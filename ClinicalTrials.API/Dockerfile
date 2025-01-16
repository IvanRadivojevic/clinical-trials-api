FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["./ClinicalTrials.API/ClinicalTrials.API.csproj", "ClinicalTrials.API/"]
COPY ["./ClinicalTrials.Infrastructure/ClinicalTrials.Infrastructure.csproj", "ClinicalTrials.Infrastructure/"]
COPY ["./ClinicalTrials.Application/ClinicalTrials.Application.csproj", "ClinicalTrials.Application/"]
COPY ["./ClinicalTrials.Domain/ClinicalTrials.Domain.csproj", "ClinicalTrials.Domain/"]

RUN dotnet restore "./ClinicalTrials.API/ClinicalTrials.API.csproj"

# Copy all files
COPY . .
WORKDIR "/src/ClinicalTrials.API"

# Build for Linux
RUN dotnet build "ClinicalTrials.API.csproj" -c Release -o /app/build --os linux

FROM build AS publish
RUN dotnet publish "ClinicalTrials.API.csproj" -c Release -o /app/publish /p:UseAppHost=false --os linux

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "ClinicalTrials.API.dll"] 