# Evaluación ATAM (Architecture Tradeoff Analysis Method)

Este documento detalla la evaluación de la arquitectura de **SkillVault** utilizando los principios de ATAM. Se identifican y analizan un riesgo, un trade-off (compromiso) y un punto de sensibilidad.

## 1. Riesgo (Risk)

**Descripción del Riesgo:** 
Manejo de secretos e infraestructura estática expuesta (Identificado en ADR-07). Actualmente, la cadena de conexión de la base de datos (PostgreSQL) y la llave secreta para la firma de tokens JWT (`SecretKey`) están escritas directamente ("hardcodeadas") en el archivo `appsettings.json` sin cifrado y rastreadas en el control de versiones.

**Justificación Arquitectónica:**
Esta decisión (o falta de) se tomó inicialmente en favor del **Time-to-Market** y la facilidad de probar el prototipo local (ADR-07). Sin embargo, impacta negativamente el atributo de calidad de **Seguridad (Security)**. Si el repositorio es comprometido, toda la base de datos y la capacidad de emitir tokens administrativos quedan comprometidas. La arquitectura actual carece de una estrategia de inyección de configuración segura por entornos.

## 2. Trade-off (Compromiso)

**Descripción del Trade-off:**
Despliegue Monolítico en Elastic Beanstalk (sacrificando HTTPS y certificados SSL) vs Microservicios con Load Balancer.

**Justificación Arquitectónica:**
Para la entrega final, se decidió empaquetar tanto el frontend (Nginx) como el backend (.NET) dentro de un único entorno Docker (Docker Compose) alojado en una sola máquina virtual de AWS Elastic Beanstalk, con una base de datos Amazon RDS independiente (ADR-08). 
*   **A favor:** Esto maximiza la **Mantenibilidad**, **Simplicidad** de desarrollo y permite mantener todo el ecosistema dentro de la **Capa Gratuita (Free Tier)** de AWS, lo cual es crítico para este proyecto académico.
*   **En contra:** Sacrifica la **Seguridad (HTTPS)** y la **Escalabilidad Independiente**. Al no usar un Application Load Balancer de AWS (el cual tiene costo), no es posible asociar un certificado SSL/TLS gratuito de AWS Certificate Manager, por lo que la aplicación opera bajo HTTP puro. 
Este compromiso fue aceptado ya que la complejidad operativa y los costos de mantener múltiples pipelines, gateways y balanceadores de carga para un solo estudiante no justificaban los beneficios de seguridad a esta escala de prototipo.

## 3. Punto de Sensibilidad (Sensitivity Point)

**Descripción del Punto de Sensibilidad:**
La confiabilidad del sistema de autenticación (`AuthUseCase`) es altamente sensible al estado del servidor debido a la persistencia en memoria (Identificado en ADR-07).

**Justificación Arquitectónica:**
Actualmente, el módulo de autenticación emplea un `ConcurrentDictionary` para guardar los usuarios. El atributo de calidad de **Confiabilidad (Reliability)** y **Tolerancia a fallos** se vuelve extremadamente sensible a los reinicios del sistema operativo o de la aplicación. Cualquier caída del servidor resulta en la pérdida irreversible de los datos de inicio de sesión de todos los usuarios registrados recientemente, rompiendo la integridad relacional de la base de datos subyacente de PostgreSQL donde sí se guardan las Skills de esos usuarios.
