FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY server.csproj ./
RUN dotnet restore server.csproj
COPY . ./
RUN dotnet publish server.csproj -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "server.dll"]
