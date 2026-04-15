# Contexto y objetivo

## Problema actual

Hoy el nivel de logs se ajusta por inquilino usando un contenedor DI por tenant y un `LoggingLevelSwitch` por tenant.
Ese enfoque funciona, pero dificulta centralizar componentes y operar con una arquitectura multi-tenant desde un contenedor raíz.

## Objetivo

Implementar una extensión de Serilog con:

- una sola configuración de logging al inicio de la aplicación;
- tenants agregados dinámicamente en runtime;
- nivel efectivo por tenant sin romper el flujo si falta contexto;
- fallback al nivel base definido en `Main`/`Program`.

## Regla funcional acordada

1. Si el tenant actual tiene override dinámico, usar ese nivel.
2. Si no hay `TenantId` o no hay override para ese tenant, usar el nivel base configurado en `Main`.
3. Nunca interrumpir el logging por ausencia de `TenantId` o por error interno de resolución.
