# Use the official .NET 9 runtime image for the base stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK 9.0 for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/KvizApi
RUN dotnet publish KvizApi.csproj -c Release -o /app/publish

# Use the base image and copy the published app files
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KvizApi.dll"]
