FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SafeDose.API/SafeDose.API.csproj", "SafeDose.API/"]
COPY ["SafeDose.Application/SafeDose.Application.csproj", "SafeDose.Application/"]
COPY ["SafeDose.Infrastructure/SafeDose.Infrastructure.csproj", "SafeDose.Infrastructure/"]
COPY ["SafeDose.Domain/SafeDose.Domain.csproj", "SafeDose.Domain/"]
COPY ["SafeDose.Identity/SafeDose.Identity.csproj", "SafeDose.Identity/"]
COPY ["SafeDose.Persistence/SafeDose.Persistence.csproj", "SafeDose.Persistence/"]

RUN dotnet restore "SafeDose.API/SafeDose.API.csproj"

COPY . .
WORKDIR "/src/SafeDose.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SafeDose.API.dll"]
