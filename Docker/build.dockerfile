FROM mcr.microsoft.com/dotnet/aspnet:7.0.5 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0.203 AS build
WORKDIR /source
COPY ./src ./src
COPY ./*.sln .
COPY ./*.props ./
COPY ./.editorconfig .

RUN dotnet restore "src/ITMO.Dev.ASAP/ITMO.Dev.ASAP.csproj"

FROM build AS publish
WORKDIR "/source/src/ITMO.Dev.ASAP"
RUN dotnet publish "ITMO.Dev.ASAP.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5069 \
    ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ITMO.Dev.ASAP.dll"]
