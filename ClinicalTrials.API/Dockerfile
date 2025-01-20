FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClinicalTrials.API/ClinicalTrials.API.csproj", "ClinicalTrials.API/"]
COPY ["ClinicalTrials.Infrastructure/ClinicalTrials.Infrastructure.csproj", "ClinicalTrials.Infrastructure/"]
COPY ["ClinicalTrials.Domain/ClinicalTrials.Domain.csproj", "ClinicalTrials.Domain/"]
COPY ["ClinicalTrials.Application/ClinicalTrials.Application.csproj", "ClinicalTrials.Application/"]
RUN dotnet restore "ClinicalTrials.API/ClinicalTrials.API.csproj"
COPY . .
WORKDIR "/src/ClinicalTrials.API"
RUN dotnet build "ClinicalTrials.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClinicalTrials.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClinicalTrials.API.dll"] 