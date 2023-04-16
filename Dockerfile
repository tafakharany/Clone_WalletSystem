# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy the source code and restore dependencies
COPY . .
RUN dotnet restore "./Wallet.API/Wallet.API.csproj" --disable-parallel

# build the app
RUN dotnet publish "./Wallet.API/Wallet.API.csproj" -c Release -o /app --no-restore

# Server Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS server
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "Wallet.API.dll"]