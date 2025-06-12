
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["UserService.sln", "."]
COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["PostgresInfrastructure/PostgresInfrastructure.csproj", "PostgresInfrastructure/"]


RUN dotnet restore "UserService.sln"


COPY . .
WORKDIR "/src/API"


RUN dotnet publish "API.csproj" -c Release -o /app/publish --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app


COPY --from=build /app/publish .

EXPOSE 8080


ENTRYPOINT ["dotnet", "API.dll"]