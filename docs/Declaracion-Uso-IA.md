# Declaración de Uso de IA

## Herramientas Utilizadas
Durante el desarrollo de **SkillVault**, se utilizaron herramientas de Inteligencia Artificial como asistentes de codificación (ej. Antigravity/Claude/Gemini).

## Propósito del Uso
Las herramientas de IA fueron utilizadas exclusivamente como apoyo bajo los siguientes escenarios:
1. **Generación de código repetitivo (Boilerplate):** Para acelerar la creación de modelos, interfaces y controladores estándar en .NET Core.
2. **Refactorización y Arquitectura Limpia:** Para ayudar en la separación de responsabilidades entre la capa de Dominio, Casos de Uso e Infraestructura (Puertos y Adaptadores).
3. **Resolución de Conflictos y Debugging:** Para identificar errores de compilación, advertencias de nulabilidad (null-safety) y resolución de conflictos de ramas en Git.
4. **Documentación:** Apoyo en la redacción de los Architectural Decision Records (ADRs), diagramas C4 en formato Mermaid y la evaluación ATAM.
5. **Configuración de CI/CD y Docker:** Para orquestar los contenedores locales y establecer el pipeline automatizado de pruebas (GitHub Actions).

## Responsabilidad y Toma de Decisiones
Todas las decisiones arquitectónicas (como la elección de PostgreSQL, el uso de Entity Framework Core, y la arquitectura hexagonal) fueron guiadas y validadas por el autor del proyecto. La IA funcionó estrictamente como un pair-programmer para materializar el diseño, garantizando que el entendimiento técnico y el control del repositorio siempre se mantuvieran del lado del desarrollador humano.
