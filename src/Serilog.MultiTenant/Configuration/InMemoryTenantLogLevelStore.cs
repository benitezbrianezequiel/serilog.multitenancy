using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Options;
using Serilog.Events;
using Serilog.MultiTenant.Abstractions;
using Serilog.MultiTenant.Options;

namespace Serilog.MultiTenant.Configuration;

public sealed class InMemoryTenantLogLevelStore : ITenantLogLevelStore, IDisposable
{
    private const string MeterName = "Serilog.MultiTenant";
    private const string StoreExpirationsMetricName = "store_expirations";
    private const string StoreSizeMetricName = "store_size";
    private const string StoreInternalErrorsMetricName = "store_internal_errors";

    private static readonly Meter Meter = new(MeterName);

    private readonly ConcurrentDictionary<string, TenantLogLevelEntry> _tenantLevels = new(StringComparer.OrdinalIgnoreCase);
    private readonly TenantLogLevelStoreOptions _options;
    private readonly Counter<long> _expiredEntriesCounter;
    private readonly IDisposable _storeSizeGauge;
    private readonly Timer? _cleanupTimer;
    private readonly Counter<long> _internalErrorsCounter;

    public InMemoryTenantLogLevelStore()
        : this(Options.Create(new TenantLogLevelStoreOptions()))
    {
    }

    public InMemoryTenantLogLevelStore(IOptions<TenantLogLevelStoreOptions>? options)
    {
        _options = SanitizeOptions(options);

        _expiredEntriesCounter = Meter.CreateCounter<long>(StoreExpirationsMetricName);
        _storeSizeGauge = Meter.CreateObservableGauge(StoreSizeMetricName, ObserveStoreSize);
        _internalErrorsCounter = Meter.CreateCounter<long>(StoreInternalErrorsMetricName);

        if (_options.CleanupInterval > TimeSpan.Zero)
        {
            _cleanupTimer = new Timer(
                static state => ((InMemoryTenantLogLevelStore)state!).CleanupExpiredEntriesSafely(),
                this,
                _options.CleanupInterval,
                _options.CleanupInterval);
        }
    }

    public bool TryGet(string tenantId, out LogEventLevel level)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            level = default;
            return false;
        }

        if (!_tenantLevels.TryGetValue(tenantId, out var entry))
        {
            level = default;
            return false;
        }

        var now = DateTimeOffset.UtcNow;
        if (entry.ExpiresAtUtc <= now)
        {
            RemoveExpired(tenantId);
            level = default;
            return false;
        }

        var updatedEntry = entry with
        {
            LastAccessedUtc = now,
            ExpiresAtUtc = now + _options.EntryTtl
        };

        _tenantLevels.TryUpdate(tenantId, updatedEntry, entry);

        level = entry.Level;
        return true;
    }

    public void Set(string tenantId, LogEventLevel level)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("Tenant id cannot be null or whitespace.", nameof(tenantId));
        }

        var now = DateTimeOffset.UtcNow;
        var expiration = now + _options.EntryTtl;

        _tenantLevels.AddOrUpdate(
            tenantId,
            _ => new TenantLogLevelEntry(level, now, expiration),
            (_, existing) => existing with
            {
                Level = level,
                LastAccessedUtc = now,
                ExpiresAtUtc = expiration
            });

        EnsureMaxEntries();
    }

    public bool Remove(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return false;
        }

        return _tenantLevels.TryRemove(tenantId, out _);
    }

    public IReadOnlyDictionary<string, LogEventLevel> Snapshot()
    {
        CleanupExpiredEntries();

        return _tenantLevels.ToDictionary(
            static pair => pair.Key,
            static pair => pair.Value.Level,
            StringComparer.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _storeSizeGauge.Dispose();
    }

    private IEnumerable<Measurement<long>> ObserveStoreSize()
    {
        yield return new Measurement<long>(_tenantLevels.Count);
    }

    private void CleanupExpiredEntriesSafely()
    {
        try
        {
            CleanupExpiredEntries();
        }
        catch (Exception ex)
        {
            _internalErrorsCounter.Add(1,
                new KeyValuePair<string, object?>("operation", "cleanup"),
                new KeyValuePair<string, object?>("error_type", ex.GetType().Name));
        }
    }

    private void CleanupExpiredEntries()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var pair in _tenantLevels)
        {
            if (pair.Value.ExpiresAtUtc <= now)
            {
                RemoveExpired(pair.Key);
            }
        }
    }

    private void RemoveExpired(string tenantId)
    {
        if (_tenantLevels.TryGetValue(tenantId, out var existing)
            && existing.ExpiresAtUtc <= DateTimeOffset.UtcNow
            && _tenantLevels.TryRemove(new KeyValuePair<string, TenantLogLevelEntry>(tenantId, existing)))
        {
            _expiredEntriesCounter.Add(1);
        }
    }

    private void EnsureMaxEntries()
    {
        const int maxPasses = 3;
        const int maxRemovalsPerPass = 8;

        for (var pass = 0; pass < maxPasses; pass++)
        {
            var excess = _tenantLevels.Count - _options.MaxEntries;
            if (excess <= 0)
            {
                return;
            }

            var removals = Math.Min(excess, maxRemovalsPerPass);
            for (var i = 0; i < removals; i++)
            {
                if (!TrySelectEvictionCandidate(out var candidate))
                {
                    return;
                }

                _tenantLevels.TryRemove(candidate, out _);
            }
        }
    }

    private bool TrySelectEvictionCandidate(out string tenantId)
    {
        const int sampleSize = 64;

        tenantId = string.Empty;
        var oldestAccess = DateTimeOffset.MaxValue;
        var sampled = 0;

        foreach (var pair in _tenantLevels)
        {
            if (pair.Value.LastAccessedUtc < oldestAccess)
            {
                oldestAccess = pair.Value.LastAccessedUtc;
                tenantId = pair.Key;
            }

            sampled++;
            if (sampled >= sampleSize)
            {
                break;
            }
        }

        return sampled > 0;
    }

    private static TenantLogLevelStoreOptions SanitizeOptions(IOptions<TenantLogLevelStoreOptions>? options)
    {
        var defaults = new TenantLogLevelStoreOptions();
        var value = options?.Value ?? defaults;

        return new TenantLogLevelStoreOptions
        {
            MaxEntries = value.MaxEntries > 0 ? value.MaxEntries : defaults.MaxEntries,
            EntryTtl = value.EntryTtl > TimeSpan.Zero ? value.EntryTtl : defaults.EntryTtl,
            CleanupInterval = value.CleanupInterval >= TimeSpan.Zero ? value.CleanupInterval : defaults.CleanupInterval
        };
    }

}
