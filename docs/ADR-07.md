# ADR 07: Registro de Deuda Técnica (Parte 1)

## Estado
Aceptado

## Contexto
Como parte de la auditoría continua del proyecto, se ha identificado deuda técnica crítica que requiere ser documentada y planificada para su eventual refactorización y resolución. 

## Deuda Técnica 1: Configuración e Infraestructura (Hardcoding de Secretos)

**Qué es:** 
Los secretos críticos de la aplicación, como la cadena de conexión a la base de datos de PostgreSQL y la clave secreta del JWT (`SecretKey`), están quemados ("hardcodeados") en texto plano dentro del archivo `appsettings.json` y expuestos en el control de versiones (Git).

**Por qué existe:**
Fue una decisión consciente durante las etapas tempranas del desarrollo para acelerar las pruebas locales y cumplir con los plazos de entrega iniciales. No se invirtió tiempo en configurar gestores de secretos ni en ofuscar las llaves temporales del JWT porque la prioridad era demostrar el flujo de autenticación funcionando.

**Costo de no pagarla:**
Si esta deuda crece o llega a producción, representa un **riesgo de seguridad crítico (Security Vulnerability)**. Cualquier actor con acceso de lectura al repositorio de GitHub obtiene acceso completo e irrestricto a la base de datos de los usuarios, así como la capacidad matemática para forjar y firmar tokens JWT maliciosos para suplantar identidades de administrador. Además, empeora drásticamente la escalabilidad y despliegue del software, pues cambiar los parámetros entre entornos (Desarrollo, Pruebas, Producción) requiere modificar el código fuente en lugar de solo actualizar las variables del servidor.

**Propuesta de solución:**
1. **Refactorización de Entornos (Options Pattern):** Extraer la información confidencial de `appsettings.json` y apoyarse en **Environment Variables** en el host de producción.
2. **Secret Manager local:** Para el desarrollo local, activar y obligar el uso de la herramienta `dotnet user-secrets` para que la cadena de conexión viva en la máquina del desarrollador y no en los archivos versionados.
3. **Rotación de Credenciales:** Generar una nueva `SecretKey` para JWT y forzar su inyección exclusiva a través del entorno de ejecución.
