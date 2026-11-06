# ── 1. restore ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS restore
WORKDIR /src

COPY EPM.TKT.slnx .
COPY EPM.TKT.App/EPM.TKT.App.csproj                   EPM.TKT.App/
COPY EPM.TKT.Domain/EPM.TKT.Domain.csproj             EPM.TKT.Domain/
COPY EPM.TKT.Infrastructure/EPM.TKT.Infrastructure.csproj EPM.TKT.Infrastructure/
COPY EPM.TKT.External/EPM.TKT.External.csproj         EPM.TKT.External/
COPY EPM.TKT.Mediator/EPM.TKT.Mediator.csproj         EPM.TKT.Mediator/
COPY EPM.TKT.WebApi/EPM.TKT.WebApi.csproj             EPM.TKT.WebApi/
COPY EPM.TKT.Tests/EPM.TKT.Tests.csproj               EPM.TKT.Tests/
COPY TKTDatabase/TKTDatabase.sqlproj                   TKTDatabase/

RUN dotnet restore EPM.TKT.slnx

# ── 2. build ─────────────────────────────────────────────────────────────────
FROM restore AS build
COPY . .
RUN dotnet build EPM.TKT.WebApi/EPM.TKT.WebApi.csproj --no-restore -c Release && \
    dotnet build EPM.TKT.Tests/EPM.TKT.Tests.csproj --no-restore -c Release

# ── 3. test ──────────────────────────────────────────────────────────────────
FROM build AS test
RUN dotnet test EPM.TKT.Tests/EPM.TKT.Tests.csproj --no-build -c Release \
    --logger "console;verbosity=minimal"

# ── 4. publish ───────────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish EPM.TKT.WebApi/EPM.TKT.WebApi.csproj \
    --no-build -c Release -o /app/publish

# ── 5. final runtime image ───────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine-extra AS final
WORKDIR /app

RUN apk update \
    && apk add --no-cache ca-certificates \
    && update-ca-certificates

COPY --from=publish /app/publish .

USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "EPM.TKT.WebApi.dll"]
