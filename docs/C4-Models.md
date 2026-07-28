# Modelos C4 - SkillVault

A continuación se presentan los diagramas de arquitectura basados en el modelo C4 para el proyecto **SkillVault**.

## Nivel 1: Contexto del Sistema (System Context)

Muestra el sistema de software que estamos construyendo y cómo encaja en el mundo en términos de las personas que lo usan y los otros sistemas con los que interactúa.

```mermaid
C4Context
  title Diagrama de Contexto de Sistema (Nivel 1) para SkillVault

  Person(user, "Usuario / Estudiante", "Un usuario que desea gestionar, validar y registrar sus habilidades y certificaciones.")
  System(skillvault, "SkillVault", "Permite a los usuarios registrarse, iniciar sesión, y administrar su perfil de habilidades, certificaciones y progresos.")
  
  Rel(user, skillvault, "Visualiza y gestiona sus habilidades usando")
```

## Nivel 2: Contenedores (Containers)

Hace zoom dentro del sistema para mostrar los contenedores (aplicaciones, bases de datos, etc.) que componen el sistema de software.

```mermaid
C4Container
  title Diagrama de Contenedores (Nivel 2) para SkillVault

  Person(user, "Usuario / Estudiante", "Un usuario que interactúa con la plataforma.")

  System_Boundary(c1, "SkillVault System") {
    Container(spa, "Single Page Application", "JavaScript, HTML, CSS", "Proporciona la interfaz de usuario en el navegador.")
    Container(api, "Web API", ".NET 8 / C#", "Provee la lógica de negocio, autenticación vía JWT y endpoints RESTful.")
    ContainerDb(db, "Base de Datos", "PostgreSQL", "Almacena información de usuarios, certificaciones, habilidades y progresos.")
  }

  Rel(user, spa, "Interactúa con", "HTTPS")
  Rel(spa, api, "Realiza peticiones a", "JSON/HTTPS")
  Rel(api, db, "Lee y escribe datos en", "Entity Framework Core / TCP")
```

## Nivel 3: Componentes (Components)

Hace zoom dentro del contenedor principal (la Web API) para mostrar cómo está estructurado por dentro, basándose en la Arquitectura Limpia (Clean Architecture).

```mermaid
C4Component
  title Diagrama de Componentes (Nivel 3) para la Web API de SkillVault

  Container_Boundary(api, "Web API (.NET 8)") {
    
    Component(controllers, "Controladores (API Layer)", "ASP.NET Core MVC", "Expone las rutas de red y maneja las peticiones HTTP.")
    Component(usecases, "Casos de Uso (Application Layer)", "C# Interfaces/Classes", "Orquesta la lógica de negocio y aplica las reglas del dominio.")
    Component(domain, "Modelo de Dominio (Domain Layer)", "C# POCOs", "Contiene las entidades Core (User, Skill, Certification).")
    Component(repositories, "Repositorios (Infrastructure Layer)", "EF Core", "Implementa los puertos de acceso a datos de la capa de aplicación.")
    Component(auth, "Servicio JWT (Infrastructure Layer)", "C#", "Genera y valida los tokens de autenticación.")
  }
  
  ContainerDb(db, "Base de Datos", "PostgreSQL", "Almacena los datos del sistema.")

  Rel(controllers, usecases, "Invoca")
  Rel(usecases, domain, "Usa entidades de")
  Rel(usecases, repositories, "Usa abstracciones (Puertos)")
  Rel(usecases, auth, "Solicita generación de tokens")
  Rel(repositories, db, "Realiza consultas/comandos SQL a")
```
