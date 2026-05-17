# FuelGuard API — ASP.NET Core 8 (Render Docker runtime)
# Build context: repository root (dockerContext: .)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FuelGuard.sln ./
COPY src/FuelGuard.Api/FuelGuard.Api.csproj src/FuelGuard.Api/
COPY src/FuelGuard.Application/FuelGuard.Application.csproj src/FuelGuard.Application/
COPY src/FuelGuard.Domain/FuelGuard.Domain.csproj src/FuelGuard.Domain/
COPY src/FuelGuard.Infrastructure/FuelGuard.Infrastructure.csproj src/FuelGuard.Infrastructure/
COPY src/FuelGuard.Agents/FuelGuard.Agents.csproj src/FuelGuard.Agents/
COPY src/FuelGuard.Shared/FuelGuard.Shared.csproj src/FuelGuard.Shared/

RUN dotnet restore src/FuelGuard.Api/FuelGuard.Api.csproj

COPY src/ src/

RUN dotnet publish src/FuelGuard.Api/FuelGuard.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
# Render injects PORT + ASPNETCORE_URLS at runtime (see render.yaml). Do not hardcode a port here.

RUN groupadd --gid 10001 appgroup \
    && useradd --uid 10001 --gid appgroup --create-home --home-dir /app appuser \
    && chown -R appuser:appgroup /app

COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "FuelGuard.Api.dll"]
