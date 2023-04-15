FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ITMO.Dev.ASAP/ITMO.Dev.ASAP.csproj", "src/ITMO.Dev.ASAP/"]
COPY ["src/core/Presentation/ITMO.Dev.ASAP.WebApi.Sdk/ITMO.Dev.ASAP.WebApi.Sdk.csproj", "src/core/Presentation/ITMO.Dev.ASAP.WebApi.Sdk/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Dto/ITMO.Dev.ASAP.Application.Dto.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Dto/"]
COPY ["src/github/Application/ITMO.Dev.ASAP.Github.Application.Dto/ITMO.Dev.ASAP.Github.Application.Dto.csproj", "src/github/Application/ITMO.Dev.ASAP.Github.Application.Dto/"]
COPY ["src/core/Presentation/ITMO.Dev.ASAP.WebApi.Abstractions/ITMO.Dev.ASAP.WebApi.Abstractions.csproj", "src/core/Presentation/ITMO.Dev.ASAP.WebApi.Abstractions/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Contracts/ITMO.Dev.ASAP.Application.Contracts.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Contracts/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Google/ITMO.Dev.ASAP.Application.Google.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Google/"]
COPY ["src/core/Domain/ITMO.Dev.ASAP.Common/ITMO.Dev.ASAP.Common.csproj", "src/core/Domain/ITMO.Dev.ASAP.Common/"]
COPY ["src/core/Infrastructure/Integration/ITMO.Dev.ASAP.Integration.Google/ITMO.Dev.ASAP.Integration.Google.csproj", "src/core/Infrastructure/Integration/ITMO.Dev.ASAP.Integration.Google/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Abstractions/ITMO.Dev.ASAP.Application.Abstractions.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Abstractions/"]
COPY ["src/core/Domain/ITMO.Dev.ASAP.Domain/ITMO.Dev.ASAP.Domain.csproj", "src/core/Domain/ITMO.Dev.ASAP.Domain/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.DataAccess.Abstractions/ITMO.Dev.ASAP.DataAccess.Abstractions.csproj", "src/core/Application/ITMO.Dev.ASAP.DataAccess.Abstractions/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Handlers/ITMO.Dev.ASAP.Application.Handlers.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Handlers/"]
COPY ["src/core/Infrastructure/ITMO.Dev.ASAP.Mapping/ITMO.Dev.ASAP.Mapping.csproj", "src/core/Infrastructure/ITMO.Dev.ASAP.Mapping/"]
COPY ["src/core/Infrastructure/ITMO.Dev.ASAP.Identity/ITMO.Dev.ASAP.Identity.csproj", "src/core/Infrastructure/ITMO.Dev.ASAP.Identity/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Common/ITMO.Dev.ASAP.Application.Common.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Common/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application/ITMO.Dev.ASAP.Application.csproj", "src/core/Application/ITMO.Dev.ASAP.Application/"]
COPY ["src/github/Presentation/ITMO.Dev.ASAP.Github.Presentation.Contracts/ITMO.Dev.ASAP.Github.Presentation.Contracts.csproj", "src/github/Presentation/ITMO.Dev.ASAP.Github.Presentation.Contracts/"]
COPY ["src/core/Application/ITMO.Dev.ASAP.Application.Specifications/ITMO.Dev.ASAP.Application.Specifications.csproj", "src/core/Application/ITMO.Dev.ASAP.Application.Specifications/"]
COPY ["src/core/Infrastructure/ITMO.Dev.ASAP.DataAccess/ITMO.Dev.ASAP.DataAccess.csproj", "src/core/Infrastructure/ITMO.Dev.ASAP.DataAccess/"]
COPY ["src/core/Presentation/ITMO.Dev.ASAP.Controllers/ITMO.Dev.ASAP.Controllers.csproj", "src/core/Presentation/ITMO.Dev.ASAP.Controllers/"]
COPY ["src/core/DeveloperEnvironment/ITMO.Dev.ASAP.DeveloperEnvironment/ITMO.Dev.ASAP.DeveloperEnvironment.csproj", "src/core/DeveloperEnvironment/ITMO.Dev.ASAP.DeveloperEnvironment/"]
COPY ["src/core/Infrastructure/ITMO.Dev.ASAP.Seeding/ITMO.Dev.ASAP.Seeding.csproj", "src/core/Infrastructure/ITMO.Dev.ASAP.Seeding/"]
COPY ["src/core/Presentation/ITMO.Dev.ASAP.Presentation.Services/ITMO.Dev.ASAP.Presentation.Services.csproj", "src/core/Presentation/ITMO.Dev.ASAP.Presentation.Services/"]
COPY ["src/core/Presentation/ITMO.Dev.ASAP.Presentation.Contracts/ITMO.Dev.ASAP.Presentation.Contracts.csproj", "src/core/Presentation/ITMO.Dev.ASAP.Presentation.Contracts/"]
COPY ["src/github/Application/ITMO.Dev.ASAP.Github.Application.Handlers/ITMO.Dev.ASAP.Github.Application.Handlers.csproj", "src/github/Application/ITMO.Dev.ASAP.Github.Application.Handlers/"]
COPY ["src/github/Presentation/ITMO.Dev.ASAP.Github.Presentation.Webhooks/ITMO.Dev.ASAP.Github.Presentation.Webhooks.csproj", "src/github/Presentation/ITMO.Dev.ASAP.Github.Presentation.Webhooks/"]
COPY ["src/core/Presentation/ITMO.Dev.ASAP.Commands/ITMO.Dev.ASAP.Commands.csproj", "src/core/Presentation/ITMO.Dev.ASAP.Commands/"]
COPY ["src/github/Application/Abstractions/ITMO.Dev.ASAP.Github.Application.Octokit/ITMO.Dev.ASAP.Github.Application.Octokit.csproj", "src/github/Application/Abstractions/ITMO.Dev.ASAP.Github.Application.Octokit/"]
COPY ["src/github/Application/ITMO.Dev.ASAP.Github.Application.Contracts/ITMO.Dev.ASAP.Github.Application.Contracts.csproj", "src/github/Application/ITMO.Dev.ASAP.Github.Application.Contracts/"]
COPY ["src/github/Application/Abstractions/ITMO.Dev.ASAP.Github.Application.DataAccess/ITMO.Dev.ASAP.Github.Application.DataAccess.csproj", "src/github/Application/Abstractions/ITMO.Dev.ASAP.Github.Application.DataAccess/"]
COPY ["src/github/ITMO.Dev.ASAP.Github.Common/ITMO.Dev.ASAP.Github.Common.csproj", "src/github/ITMO.Dev.ASAP.Github.Common/"]
COPY ["src/github/ITMO.Dev.ASAP.Github.Domain/ITMO.Dev.ASAP.Github.Domain.csproj", "src/github/ITMO.Dev.ASAP.Github.Domain/"]
COPY ["src/github/Application/ITMO.Dev.ASAP.Github.Application.Mapping/ITMO.Dev.ASAP.Github.Application.Mapping.csproj", "src/github/Application/ITMO.Dev.ASAP.Github.Application.Mapping/"]
COPY ["src/github/Application/ITMO.Dev.ASAP.Github.Application.Specifications/ITMO.Dev.ASAP.Github.Application.Specifications.csproj", "src/github/Application/ITMO.Dev.ASAP.Github.Application.Specifications/"]
COPY ["src/github/Application/ITMO.Dev.ASAP.Github.Application/ITMO.Dev.ASAP.Github.Application.csproj", "src/github/Application/ITMO.Dev.ASAP.Github.Application/"]
COPY ["src/github/Infrastructure/ITMO.Dev.ASAP.Github.Octokit/ITMO.Dev.ASAP.Github.Octokit.csproj", "src/github/Infrastructure/ITMO.Dev.ASAP.Github.Octokit/"]
COPY ["src/github/Infrastructure/ITMO.Dev.ASAP.Github.DataAccess/ITMO.Dev.ASAP.Github.DataAccess.csproj", "src/github/Infrastructure/ITMO.Dev.ASAP.Github.DataAccess/"]
COPY ["src/github/Presentation/ITMO.Dev.ASAP.Github.Presentation.Services/ITMO.Dev.ASAP.Github.Presentation.Services.csproj", "src/github/Presentation/ITMO.Dev.ASAP.Github.Presentation.Services/"]
COPY ["src/frontend/ITMO.Dev.ASAP.WebUI.AdminPanel/ITMO.Dev.ASAP.WebUI.AdminPanel.csproj", "src/frontend/ITMO.Dev.ASAP.WebUI.AdminPanel/"]
RUN dotnet restore "src/ITMO.Dev.ASAP/ITMO.Dev.ASAP.csproj"
COPY . .
WORKDIR "/src/src/ITMO.Dev.ASAP"
RUN dotnet build "ITMO.Dev.ASAP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ITMO.Dev.ASAP.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5069 \
    ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ITMO.Dev.ASAP.dll"]
