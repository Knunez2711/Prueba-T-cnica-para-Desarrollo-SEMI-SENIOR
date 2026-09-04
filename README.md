# Hospital API

API REST para gestion de pacientes de un hospital, desarrollada con .NET y PostgreSQL.

## Requisitos

- .NET 10 SDK o superior
- PostgreSQL 16
- Base de datos `Hospital`

## Conexion

La cadena de conexion esta configurada en `HospitalApi/appsettings.Development.json`:

```json
"HospitalConnection": "Host=localhost;Port=5432;Database=Hospital;Username=knunez"
```

Si PostgreSQL requiere password, agregarlo asi:

```json
"HospitalConnection": "Host=localhost;Port=5432;Database=Hospital;Username=knunez;Password=TU_PASSWORD"
```

## Ejecutar

```bash
dotnet run --project HospitalApi/HospitalApi.csproj --launch-profile http
```

Swagger:

```text
http://localhost:5031/swagger
```

## Endpoints

- `POST /api/Pacientes`
- `GET /api/Pacientes`
- `GET /api/Pacientes/{id}`
- `PUT /api/Pacientes/{id}`
- `DELETE /api/Pacientes/{id}`

## Base de datos

El script de referencia esta en `HospitalApi/Data/schema.sql`.

La API usa consultas SQL parametrizadas con `Npgsql`, sin Entity Framework.
