# Этап 1: сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MessageProject.csproj", "./"]
RUN dotnet restore "./MessageProject.csproj"
COPY . .
RUN dotnet publish "MessageProject.csproj" -c Release -o /app/publish

# Этап 2: запуск приложения
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "MessageProject.dll"]
