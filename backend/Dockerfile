FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# copy csproj first for layer caching
COPY backend.csproj .
RUN dotnet restore

# copy source and publish
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "backend.dll"]
