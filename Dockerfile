# ── 1. restore ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS restore
WORKDIR /src

COPY Core/EPM.TKT.App/EPM.TKT.App.csproj                                     Core/EPM.TKT.App/
COPY Core/EPM.TKT.Domain/EPM.TKT.Domain.csproj                               Core/EPM.TKT.Domain/
COPY Core/EPM.TKT.Mediator/EPM.TKT.Mediator.csproj                           Core/EPM.TKT.Mediator/
COPY Infrastructure/EPM.TKT.External/EPM.TKT.External.csproj                 Infrastructure/EPM.TKT.External/
COPY Infrastructure/EPM.TKT.Infrastructure/EPM.TKT.Infrastructure.csproj     Infrastructure/EPM.TKT.Infrastructure/
COPY Presentation/EPM.TKT.WebApi/EPM.TKT.WebApi.csproj                       Presentation/EPM.TKT.WebApi/
COPY Tests/EPM.TKT.AppTests/EPM.TKT.AppTests.csproj                          Tests/EPM.TKT.AppTests/
COPY Tests/EPM.TKT.InfrastructureTests/EPM.TKT.InfrastructureTests.csproj    Tests/EPM.TKT.InfrastructureTests/
COPY Tests/EPM.TKT.ArchitectureTests/EPM.TKT.ArchitectureTests.csproj        Tests/EPM.TKT.ArchitectureTests/
COPY Tests/EPM.TKT.ExternalTests/EPM.TKT.ExternalTests.csproj                Tests/EPM.TKT.ExternalTests/
COPY Tests/EPM.TKT.MediatorTests/EPM.TKT.MediatorTests.csproj                Tests/EPM.TKT.MediatorTests/
COPY Tests/EPM.TKT.WebAPITests/EPM.TKT.WebAPITests.csproj                    Tests/EPM.TKT.WebAPITests/

RUN dotnet restore Presentation/EPM.TKT.WebApi/EPM.TKT.WebApi.csproj && \
    dotnet restore Tests/EPM.TKT.AppTests/EPM.TKT.AppTests.csproj && \
    dotnet restore Tests/EPM.TKT.InfrastructureTests/EPM.TKT.InfrastructureTests.csproj && \
    dotnet restore Tests/EPM.TKT.ArchitectureTests/EPM.TKT.ArchitectureTests.csproj && \
    dotnet restore Tests/EPM.TKT.ExternalTests/EPM.TKT.ExternalTests.csproj && \
    dotnet restore Tests/EPM.TKT.MediatorTests/EPM.TKT.MediatorTests.csproj && \
    dotnet restore Tests/EPM.TKT.WebAPITests/EPM.TKT.WebAPITests.csproj

# ── 2. build ─────────────────────────────────────────────────────────────────
FROM restore AS build
COPY . .
RUN dotnet build Presentation/EPM.TKT.WebApi/EPM.TKT.WebApi.csproj                      --no-restore -c Release && \
    dotnet build Tests/EPM.TKT.AppTests/EPM.TKT.AppTests.csproj                         --no-restore -c Release && \
    dotnet build Tests/EPM.TKT.InfrastructureTests/EPM.TKT.InfrastructureTests.csproj   --no-restore -c Release && \
    dotnet build Tests/EPM.TKT.ArchitectureTests/EPM.TKT.ArchitectureTests.csproj       --no-restore -c Release && \
    dotnet build Tests/EPM.TKT.ExternalTests/EPM.TKT.ExternalTests.csproj               --no-restore -c Release && \
    dotnet build Tests/EPM.TKT.MediatorTests/EPM.TKT.MediatorTests.csproj               --no-restore -c Release && \
    dotnet build Tests/EPM.TKT.WebAPITests/EPM.TKT.WebAPITests.csproj                   --no-restore -c Release
        
# ── 3. test ──────────────────────────────────────────────────────────────────
FROM build AS test
RUN dotnet test Tests/EPM.TKT.AppTests/EPM.TKT.AppTests.csproj                          --no-build -c Release --logger "console;verbosity=minimal" && \
    dotnet test Tests/EPM.TKT.InfrastructureTests/EPM.TKT.InfrastructureTests.csproj    --no-build -c Release --logger "console;verbosity=minimal" && \
    dotnet test Tests/EPM.TKT.ArchitectureTests/EPM.TKT.ArchitectureTests.csproj        --no-build -c Release --logger "console;verbosity=minimal" && \
    dotnet test Tests/EPM.TKT.ExternalTests/EPM.TKT.ExternalTests.csproj                --no-build -c Release --logger "console;verbosity=minimal" && \
    dotnet test Tests/EPM.TKT.MediatorTests/EPM.TKT.MediatorTests.csproj                --no-build -c Release --logger "console;verbosity=minimal" && \
    dotnet test Tests/EPM.TKT.WebAPITests/EPM.TKT.WebAPITests.csproj                    --no-build -c Release --logger "console;verbosity=minimal"

# ── 4. publish ───────────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish Presentation/EPM.TKT.WebApi/EPM.TKT.WebApi.csproj \
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
