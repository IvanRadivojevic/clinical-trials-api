# Clinical Trials API

A REST API for managing clinical trials data. Enables JSON file uploads with clinical trial data, search, and filtering capabilities.

## Technologies
- .NET 8
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI
- Serilog
- Docker

## Configuration

The project uses different connection strings for different environments:

### Local Development
{
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ClinicalTrialsDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
}

### Docker
{
"ConnectionStrings": {
"DefaultConnection": "Server=db;Database=ClinicalTrialsDb;User=sa;Password=Administrator123!;TrustServerCertificate=True"
}
}

### Production
Copy `appsettings.json` to `appsettings.Production.json` and configure your production values.

## Getting Started

### Local Development
1. Clone the repository
      git clone https://github.com/IvanRadivojevic/clinical-trials-api.git
2. Restore NuGet packages
      dotnet restore
3. Apply migrations
      otnet ef database update
4. Run the application
      dotnet run

### Docker Development
1. Build and run containers
      docker-compose up --build
2. Run existing containers
      docker-compose up

## API Endpoints
- `POST /api/FileUpload/upload` - Upload clinical trial JSON file
- `GET /api/FileUpload/{id}` - Get trial by ID
- `GET /api/FileUpload/filter` - Filter trials by status/title/trialId
- `GET /api/FileUpload/all` - Get all trials

## Sample JSON Upload File
{
"trialId": "TEST001",
"title": "Test Clinical Trial",
"startDate": "2025-02-01",
"status": "Ongoing",
"participants": 100
}

## Validation Rules
- StartDate must be in the future
- Participants must be greater than 0
- Status must be one of the valid values (Ongoing, Completed, Planned)
- All fields are required

## Logging
The application uses Serilog for logging. Logs are stored in the `logs` folder.

## Author
[Ivan Radivojevic]

## License
MIT

   
