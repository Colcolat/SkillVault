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

## Deuda Técnica 2: Arquitectura de Persistencia (In-Memory Auth)

**Qué es:**
El caso de uso `AuthUseCase` y la gestión de inicio de sesión de los usuarios (`Users`) actualmente dependen de un `ConcurrentDictionary` estático en memoria, en lugar de utilizar una base de datos real a través de un repositorio dedicado (`IUserRepository`).

**Por qué existe:**
Al igual que los secretos hardcodeados, la implementación de persistencia en memoria se hizo como un *mock* temporal para un prototipo rápido de validación de JWT, y se dejó olvidado como descuido no detectado a tiempo al avanzar hacia otras capas de la arquitectura (como Certifications y Skills, que sí usan la base de datos).

**Costo de no pagarla:**
Si la aplicación se reinicia, todos los nuevos usuarios registrados se pierden permanentemente. Esto arruina la integridad de los datos relacionales (ej. un usuario crea un progreso, pero al reiniciar el servidor el usuario desaparece). Además, no escala en entornos modernos: si se despliegan múltiples instancias de la API detrás de un balanceador de carga, los usuarios creados en un nodo no existirán en el otro. Finalmente, las contraseñas se manejan en texto plano, multiplicando la gravedad de la deuda técnica anterior.

**Propuesta de solución:**
1. **Patrón de Repositorio (Repository Pattern):** Crear el puerto de salida `IUserRepository` y su adaptador concreto `PostgresUserRepository` utilizando Entity Framework Core.
2. **Entidad de Dominio:** Promover al `User` a una verdadera entidad de dominio con campos como `PasswordHash`.
3. **Hashing Criptográfico:** Integrar una librería de hashing como `BCrypt.Net` en el registro e inicio de sesión para nunca persistir, ni comparar, las contraseñas en texto plano.
