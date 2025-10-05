# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InventorySystem/InventorySystem.csproj", "InventorySystem/"]
RUN dotnet restore "InventorySystem/InventorySystem.csproj"
COPY . .
WORKDIR "/src/InventorySystem"
RUN dotnet build "InventorySystem.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "InventorySystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem.dll"]
