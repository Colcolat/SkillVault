# ADR 08: Migración a Arquitectura Cloud Monolítica en AWS (Elastic Beanstalk + RDS)

**Fecha:** 30 de Julio de 2026
**Estado:** Aceptado

## Contexto
El proyecto inicialmente desplegaba el frontend de forma estática en GitHub Pages y la API en la plataforma Render. Sin embargo, para la entrega final del proyecto se requería consolidar la arquitectura en una infraestructura más robusta, administrar bases de datos relacionales persistentes, y garantizar un pipeline de CI/CD visible. Se identificó la necesidad de moverse a Amazon Web Services (AWS) aprovechando la Capa Gratuita (Free Tier).

## Decisiones
1. **Infraestructura Unificada (All-in-One):** En lugar de mantener el frontend y el backend separados, se decidió utilizar **AWS Elastic Beanstalk** (Plataforma Docker) para alojar todo el código de la aplicación.
2. **Uso de Proxy Inverso:** Se integró un contenedor con **Nginx** usando Docker Compose dentro de Elastic Beanstalk. Nginx se encarga de servir los archivos estáticos del frontend (HTML/CSS/JS) en el puerto 80 y actuar como Proxy Inverso para enrutar las peticiones al contenedor de la API .NET en el puerto 8080.
3. **Base de Datos Administrada:** Se aprovisionó una instancia de **Amazon RDS para PostgreSQL** (db.t4g.micro) de manera independiente al servidor EC2 para asegurar la persistencia y respaldo de los datos, configurando los Security Groups para permitir tráfico seguro desde la instancia de Elastic Beanstalk.
4. **CI/CD Automatizado:** Se refactorizó el pipeline de **GitHub Actions** para que, tras ejecutar exitosamente las pruebas unitarias (`dotnet test`), empaquete el código fuente y despliegue el archivo ZIP directamente a Elastic Beanstalk usando el AWS CLI (vía action de terceros), cerrando así el ciclo de integración y despliegue continuo.

## Consecuencias
*   **Positivas:** 
    *   Consolidación de la infraestructura en un solo ecosistema nativo de AWS.
    *   Resolución de problemas de Cross-Origin Resource Sharing (CORS) ya que el frontend y el backend viven bajo el mismo dominio de AWS (enrutamiento de `/api/` manejado por Nginx).
    *   Cumplimiento total con los requisitos de la rúbrica (Pipeline CI visible, Pruebas automáticas, despliegue real).
*   **Negativas / Trade-offs:** 
    *   El dominio proporcionado por Elastic Beanstalk no incluye un certificado SSL/HTTPS gratuito de forma nativa sin el uso de un Load Balancer (el cual genera costos). Se tuvo que sacrificar temporalmente la encriptación HTTPS para poder mantener el proyecto 100% dentro de la Capa Gratuita.
    *   Aumento en la complejidad del archivo `docker-compose.yml` y la necesidad de configurar variables de entorno para vincular los contenedores locales.
