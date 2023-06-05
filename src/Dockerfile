# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app

# copy the source code and restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /Wallet
COPY . .
RUN dotnet restore "./Wallet.API/Wallet.API.csproj" --disable-parallel

# build the app
RUN dotnet build "./Wallet.API/Wallet.API.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "./Wallet.API/Wallet.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# final Stage
EXPOSE 4430
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.API.dll"]