# CLAUDE.md — Horror Film Journal

This file provides persistent context for Claude Code working in this repository.

---

## Project Purpose

A .NET 10 minimal API microservice for tracking horror films — watched, wishlisted, and abandoned. Built primarily as a MongoDB learning project. Favor approaches that demonstrate MongoDB features (aggregation, indexes, change streams, builders) over simpler alternatives.

---

## Tech Stack

| Layer       | Technology                                   |
| ----------- | -------------------------------------------- |
| Runtime     | .NET 10                                      |
| API style   | Minimal API (no MVC controllers)             |
| Database    | MongoDB (local via Docker, Atlas-compatible) |
| Driver      | MongoDB.Driver (official C# driver)          |
| DI / Config | Microsoft.Extensions (standard .NET)         |
| Testing     | xUnit + Moq                                  |
| Local dev   | Docker Compose (mongo:7 + mongo-express)     |

---

## Project Structure

```
HorrorJournal/
├── HorrorJournal.Api/
│   ├── Models/
│   │   ├── Film.cs
│   │   ├── FilmStatus.cs
│   │   └── Requests/         # DTOs: FilmFilter, FilmUpdate
│   ├── Repositories/
│   │   ├── IFilmRepository.cs
│   │   └── FilmRepository.cs
│   ├── Services/
│   │   ├── IndexInitializer.cs
│   │   └── FilmChangeStreamService.cs
│   ├── Endpoints/
│   │   └── FilmEndpoints.cs
│   ├── Infrastructure/
│   │   └── DataSeeder.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
├── HorrorJournal.Tests/
│   └── ...
├── docker-compose.yml
└── README.md
```

---

## Conventions

### General

- All public methods must be `async` and return `Task` or `Task<T>`
- Use `CancellationToken` parameters on all async methods
- Prefer `ILogger<T>` for logging — no `Console.WriteLine`
- Use `IOptions<T>` for all configuration binding
- No static classes except for endpoint extension methods

### MongoDB Specific

- `IMongoClient` registered as **singleton**
- `IMongoDatabase` and `IMongoCollection<T>` resolved per repository — do not register them directly in DI
- Always use `Builders<T>.Filter`, `Builders<T>.Update`, `Builders<T>.Sort`, and `Builders<T>.Projection` — never construct raw `BsonDocument` queries unless there is no typed alternative
- Use lambda expressions in builders (e.g., `Builders<Film>.Filter.Eq(f => f.Status, status)`) for type safety
- Collection name is `"films"` — define as a constant in `FilmRepository`
- `CreatedAt` is set on insert only; `UpdatedAt` is always set on update
- All repository methods take an optional `CancellationToken ct = default`

### Models

- Use `[BsonId]` and `[BsonRepresentation(BsonType.ObjectId)]` on the `Id` property
- Use `[BsonElement("camelCaseName")]` to control field names in MongoDB
- Enums serialized as strings: apply `[BsonRepresentation(BsonType.String)]`
- Nullable value types (e.g., `int?`, `DateTime?`) are fine — MongoDB handles missing fields gracefully

### API

- Endpoints defined in static extension methods (`MapFilmEndpoints`) called from `Program.cs`
- Return `TypedResults` (e.g., `TypedResults.Ok(...)`, `TypedResults.NotFound()`)
- Validate input at the endpoint level before passing to the repository
- HTTP status codes: `200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`, `204 No Content` (delete)

### Error Handling

- Catch `MongoException` at the repository level and rethrow as domain exceptions or let bubble for global handling
- Do not swallow exceptions silently

---

## MongoDB Learning Goals

This project exists to learn MongoDB. When generating code, **prefer the approach that best demonstrates MongoDB capabilities**, even if a simpler alternative exists. Specifically:

- Use the **aggregation pipeline** for any multi-step data transformation or stats query
- Use **text indexes** for search (not in-memory LINQ filtering)
- Use **change streams** for real-time event handling
- Use **TTL indexes** for any auto-expiry scenario
- Use **compound indexes** where query patterns justify them
- Explain index choices in comments when non-obvious

---

## Environment

- Local MongoDB: `mongodb://localhost:27017` (Docker Compose)
- Database name: `horror_journal`
- mongo-express UI: `http://localhost:8081`
- Development seeding: runs automatically if collection is empty

---

## What Not to Do

- Do not use Entity Framework Core — this project uses the MongoDB driver directly
- Do not add MVC controllers — minimal API only
- Do not add authentication — out of scope for this learning project
- Do not use `BsonDocument` for queries when typed builders are available
- Do not register `IMongoCollection<T>` directly in DI
- Do not use `Thread.Sleep` — always `await Task.Delay` with a cancellation token
