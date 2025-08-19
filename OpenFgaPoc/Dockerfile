FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5001
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OpenFgaPoc/OpenFgaPoc.csproj", "OpenFgaPoc/"]
RUN dotnet restore "./OpenFgaPoc/OpenFgaPoc.csproj"
COPY . .
WORKDIR "/src/OpenFgaPoc"
RUN dotnet build "./OpenFgaPoc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OpenFgaPoc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenFgaPoc.dll"]