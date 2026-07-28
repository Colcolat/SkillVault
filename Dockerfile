FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SkillVault/SkillVault.csproj", "SkillVault/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "SkillVault/SkillVault.csproj"

COPY . .
WORKDIR "/src/SkillVault"
RUN dotnet build "SkillVault.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkillVault.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SkillVault.dll"]
