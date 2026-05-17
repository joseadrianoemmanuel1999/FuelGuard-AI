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
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FuelGuard.Api.dll"]
