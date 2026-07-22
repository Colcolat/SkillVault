# ADR-06: Estrategia de Pruebas Unitarias e Integración Continua

## Contexto
Como parte del proceso de calidad del software y para cumplir con la Actividad #37, se requiere la implementación de pruebas unitarias automatizadas y un pipeline de Integración Continua (CI).

## Decisión
Se ha decidido utilizar **xUnit** y **Moq** para las pruebas unitarias y **GitHub Actions** para el pipeline de CI.
Las pruebas se enfocarán en probar el comportamiento de los Casos de Uso (Capa de Aplicación).

Las 3 clases seleccionadas para esta fase inicial de pruebas son:
1. `AuthUseCase`: Elegida porque maneja la autenticación y seguridad central de la aplicación. Es vital asegurar que el acceso no autorizado sea rechazado y que las credenciales correctas generen el token de acceso.
2. `CertificationUseCase`: Elegida porque actúa como un orquestador clave que recibe datos del exterior, asegura que pasen las validaciones del dominio (ej. fechas, proveedores válidos) y coordina con la persistencia.
3. `ProgressUseCase`: Elegida por contener lógica de negocio importante que involucra múltiples entidades (verificar la existencia de un certificado o curso antes de registrar progreso, validación de horas ingresadas). 

## Consecuencias
*   **Positivas:** 
    *   Ejecución automática de pruebas en cada commit a la rama principal (gracias al workflow de CI), previniendo regresiones de código.
    *   Validación independiente de infraestructura (gracias a Moq para aislar la persistencia).
*   **Negativas:**
    *   Mantenimiento extra para los *Mocks* si los contratos de la capa de persistencia cambian de forma frecuente.
