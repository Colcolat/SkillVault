# Guía de Ejecución Local con Docker (A prueba de fallos)

Este proyecto está configurado para ejecutarse completamente aislado en contenedores de Docker, lo que significa que no necesitas instalar PostgreSQL localmente, ni tener conexión a internet, ni configurar variables de entorno en tu máquina. Todo está encapsulado.

## Requisitos Previos
* Tener **Docker Desktop** instalado y abierto.

## Cómo levantar el proyecto para la presentación

1. Abre una terminal en la raíz del proyecto (donde está este archivo).
2. Ejecuta el siguiente comando:
   ```bash
   docker-compose up --build
   ```
3. Docker descargará las imágenes (la primera vez), compilará tu Web API y levantará ambos contenedores (Base de datos y API).
4. El contenedor de la base de datos se iniciará primero. Una vez que esté listo ("healthy"), la API se iniciará.
5. Gracias al código en `Program.cs`, **la API ejecutará automáticamente las migraciones** de Entity Framework sobre la base de datos de PostgreSQL recién creada. No necesitas correr `dotnet ef database update`.

## Acceder a la aplicación

Una vez que veas en la terminal que la aplicación ha iniciado (usualmente dice "Application started"):
* La API estará disponible en: **http://localhost:8080**
* Puedes probar el endpoint de salud (Health Check) en: **http://localhost:8080/health**

¡Todo está listo para tu presentación!

## Cómo detener el proyecto
Cuando termines de presentar, puedes detener los contenedores usando `Ctrl+C` en la terminal donde corre, o ejecutando:
```bash
docker-compose down
```
*(Tus datos de prueba no se perderán gracias al volumen configurado en el archivo docker-compose.yml)*
