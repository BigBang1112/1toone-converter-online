# Debug phase
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Publish phase
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
ARG APPNAME=1toOneConverterOnline
RUN apt-get update && apt-get install -y nodejs npm
RUN dotnet workload install wasm-tools
WORKDIR /src
# copy csproj and restore as distinct layers
COPY $APPNAME/*.csproj $APPNAME/
COPY $APPNAME.Server/*.csproj $APPNAME.Server/
RUN dotnet restore $APPNAME.Server/$APPNAME.Server.csproj -a $TARGETARCH --no-cache
# copy and publish app and libraries
COPY $APPNAME $APPNAME
COPY $APPNAME.Server $APPNAME.Server
RUN dotnet publish $APPNAME.Server -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app -p:UseAppHost=false --no-restore


# Final phase
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "1toOneConverterOnline.Server.dll"]