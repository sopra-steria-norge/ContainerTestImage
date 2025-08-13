# ContainerTestImage - Copilot Instructions

## Project Overview
ContainerTestImage is a containerized .NET 8.0 web API application that demonstrates Azure AD (Microsoft Entra ID) authentication, database integration, health monitoring, and Docker deployment. It serves as a template for modernizing and containerizing existing .NET applications.

## Key Architecture Components

### Authentication & Authorization
- Microsoft Identity Web (MSAL) for Azure AD authentication (`Msal/AuthenticationInstaller.cs`)
- JWT Bearer token authentication with role-based access control
- Swagger UI configured with OAuth authentication flow

### Database Access
- Entity Framework Core with SQL Server integration
- Auto-migration on startup (in `Program.cs`)
- Database connection health check endpoint at `/health/database`

### API Structure
- Controllers follow RESTful patterns with health check endpoints
- Authentication requirements enforced via `[Authorize]` attributes

## Development Workflow

### Local Development
```bash
# Build and run the application locally
dotnet build src/ContainerTestImage/ContainerTestImage.csproj
dotnet run --project src/ContainerTestImage/ContainerTestImage.csproj

# Run database migrations
dotnet ef migrations add <MigrationName> --project src/ContainerTestImage/ContainerTestImage.csproj
dotnet ef database update --project src/ContainerTestImage/ContainerTestImage.csproj
```

### Docker Workflow
```bash
# Run SQL Server container
docker compose -f docker-compose.database.yml up -d

# Build and run application container
docker compose up -d

# View logs
docker compose logs -f

# Access container shell
docker compose exec containertestimage-1 /bin/bash
```

## Project Conventions

### Dependency Injection
- Automatic service registration using Scrutor (see `Program.cs`)
- Pattern: Interface implementations are auto-discovered and registered
- Example: `services.Scan(a => a.FromAssemblyOf<Program>().AddClasses(publicOnly: true).AsMatchingInterface())`

### Configuration
- Environment-specific settings in `appsettings.{Environment}.json`
- Database connections use `DefaultDatabase` connection string
- Azure AD configuration under `AzureAd` section in appsettings.json

## Diagnostics and Monitoring
- Application Insights telemetry integration
- Health check endpoints: `/`, `/health`, `/health/authorize`, `/health/database`
- SSH access enabled within container (port 2222, credentials: root/Docker!)

## CI/CD Pipeline
- Azure DevOps pipeline defined in `pipelines/master-build-deploy.yml`
- Containerized deployment with automatic database migrations
- Environment-specific configurations managed through Azure KeyVault
