# Evaluación ATAM (Architecture Tradeoff Analysis Method)

Este documento detalla la evaluación de la arquitectura de **SkillVault** utilizando los principios de ATAM. Se identifican y analizan un riesgo, un trade-off (compromiso) y un punto de sensibilidad.

## 1. Riesgo (Risk)

**Descripción del Riesgo:** 
Manejo de secretos e infraestructura estática expuesta (Identificado en ADR-07). Actualmente, la cadena de conexión de la base de datos (PostgreSQL) y la llave secreta para la firma de tokens JWT (`SecretKey`) están escritas directamente ("hardcodeadas") en el archivo `appsettings.json` sin cifrado y rastreadas en el control de versiones.

**Justificación Arquitectónica:**
Esta decisión (o falta de) se tomó inicialmente en favor del **Time-to-Market** y la facilidad de probar el prototipo local (ADR-07). Sin embargo, impacta negativamente el atributo de calidad de **Seguridad (Security)**. Si el repositorio es comprometido, toda la base de datos y la capacidad de emitir tokens administrativos quedan comprometidas. La arquitectura actual carece de una estrategia de inyección de configuración segura por entornos.

## 2. Trade-off (Compromiso)

**Descripción del Trade-off:**
Elección de un Monolito Modular (Clean Architecture) en lugar de una arquitectura de Microservicios.

**Justificación Arquitectónica:**
Al diseñar el backend (.NET Core Web API), se optó por estructurarlo en capas lógicas estrictas (Controladores, Casos de Uso, Dominio e Infraestructura) pero corriendo bajo un único proceso (Monolito). 
*   **A favor:** Esto maximiza la **Mantenibilidad**, **Simplicidad** de desarrollo y **Facilidad de despliegue**.
*   **En contra:** Sacrifica la **Escalabilidad Independiente** (ej. si el módulo de "Certifications" tiene mucha carga, no se puede escalar sin escalar también "Auth").
Este compromiso fue aceptado ya que la complejidad operativa de mantener múltiples pipelines, gateways y orquestación (Kubernetes) para un equipo pequeño no justificaba los beneficios a esta escala.

## 3. Punto de Sensibilidad (Sensitivity Point)

**Descripción del Punto de Sensibilidad:**
La confiabilidad del sistema de autenticación (`AuthUseCase`) es altamente sensible al estado del servidor debido a la persistencia en memoria (Identificado en ADR-07).

**Justificación Arquitectónica:**
Actualmente, el módulo de autenticación emplea un `ConcurrentDictionary` para guardar los usuarios. El atributo de calidad de **Confiabilidad (Reliability)** y **Tolerancia a fallos** se vuelve extremadamente sensible a los reinicios del sistema operativo o de la aplicación. Cualquier caída del servidor resulta en la pérdida irreversible de los datos de inicio de sesión de todos los usuarios registrados recientemente, rompiendo la integridad relacional de la base de datos subyacente de PostgreSQL donde sí se guardan las Skills de esos usuarios.
