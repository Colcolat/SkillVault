# Índice de Decisiones Arquitectónicas (ADRs) - SkillVault

Este documento consolida el registro de todas las decisiones arquitectónicas importantes tomadas durante el desarrollo de la plataforma **SkillVault**. Cada decisión documenta el contexto, las alternativas consideradas y la decisión final aceptada.

| Número | Título | Descripción Breve | Estado |
|---|---|---|---|
| [ADR-01](ADR-01) | Elección de PostgreSQL | Se adoptó PostgreSQL como base de datos principal para el proyecto. | Aceptado |
| [ADR-02](ADR-02) | Adopción de Entity Framework Core | Uso de EF Core como ORM para la persistencia de datos. | Aceptado |
| [ADR-03](ADR-03) | Arquitectura Limpia (Clean Architecture) | Organización del código en capas de Dominio, Aplicación e Infraestructura. | Aceptado |
| [ADR-04](ADR-04) | Autenticación con JWT | Uso de JSON Web Tokens para seguridad y control de sesiones stateless. | Aceptado |
| [ADR-05](ADR-05) | Patrón Repositorio y Output Ports | Desacoplamiento de la persistencia de datos de los Casos de Uso. | Aceptado |
| [ADR-06](ADR-06.md) | Pruebas Unitarias y CI | Implementación de xUnit y un pipeline de Integración Continua en GitHub Actions. | Aceptado |
| [ADR-07](ADR-07.md) | Registro de Deuda Técnica | Documentación de deuda técnica en persistencia in-memory y hardcoding de secretos. | Aceptado |
| [ADR-08](ADR-08.md) | Migración a Arquitectura Cloud Monolítica en AWS | Uso de Elastic Beanstalk (Nginx + API) y Amazon RDS para consolidación de infraestructura. | Aceptado |

---
**Nota para la Entrega Final:** Este índice centraliza las decisiones tomadas desde la Unidad II hasta la entrega final del proyecto.
