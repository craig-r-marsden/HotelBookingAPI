# Hotel Booking API

## Setup Instructions

1. Ensure db connection string is set to a valid value in `appsettings.json` (server will need to be changed if SQL Server Express LocalDB is not installed)
2. Build and run app (db will be created automatically using the connection set in `appsettings.json`)
3. Access Swagger UI at `https://localhost:7096/swagger/index.html`
4. Seed initial data using `POST /api/seed` endpoint

Data can be reset data using `DELETE /api/seed` endpoint (or delete and re-seed in one step by using `POST /api/seed` with 'force' parameter set to "true").
