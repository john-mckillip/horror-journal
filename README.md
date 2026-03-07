# 🎬 Horror Film Journal

A .NET 10 minimal API microservice for tracking horror films — watched, wishlisted, and abandoned. Built as a hands-on MongoDB learning project exploring aggregation pipelines, text indexes, change streams, and the official C# driver.

---

## Features

- Track films by status: **Watched**, **Wishlist**, **Abandoned**
- Rate and review films with free-text notes
- Tag films with subgenres (Slasher, Folk Horror, Cosmic Horror, etc.)
- Full-text search across titles, directors, and notes
- Aggregated stats: ratings, top subgenres, films per year, top directors
- Real-time change stream logging via a background service

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## Getting Started

### 1. Start MongoDB locally

```bash
docker compose up -d
```

This starts:

- **MongoDB** on `localhost:27017`
- **mongo-express** (web UI) on `http://localhost:8081`

### 2. Run the API

```bash
dotnet run --project HorrorJournal.Api
```

The API will be available at `https://localhost:5001`. On first run in Development mode, the database will be seeded with sample films automatically.

### 3. Explore the API

Swagger UI is available at:

```
https://localhost:5001/swagger
```

---

## API Endpoints

### Films

| Method   | Endpoint                 | Description                                              |
| -------- | ------------------------ | -------------------------------------------------------- |
| `GET`    | `/films`                 | List films (filter by `status`, `subgenre`, `minRating`) |
| `GET`    | `/films/{id}`            | Get a single film                                        |
| `GET`    | `/films/search?q={text}` | Full-text search                                         |
| `POST`   | `/films`                 | Add a new film                                           |
| `PUT`    | `/films/{id}`            | Update a film (partial)                                  |
| `DELETE` | `/films/{id}`            | Delete a film                                            |

### Stats

| Method | Endpoint       | Description                         |
| ------ | -------------- | ----------------------------------- |
| `GET`  | `/films/stats` | Aggregated stats across the journal |

### Example: Add a Film

```http
POST /films
Content-Type: application/json

{
  "title": "Hereditary",
  "year": 2018,
  "director": "Ari Aster",
  "subgenres": ["Supernatural", "Folk Horror", "Psychological"],
  "status": "Watched",
  "rating": 9,
  "watchedOn": "2024-10-31",
  "notes": "One of the most unsettling films I've ever seen. The attic scene."
}
```

### Example: Filter by Subgenre

```http
GET /films?subgenre=Folk+Horror&minRating=7
```

---

## Configuration

Settings are managed via `appsettings.json` and `appsettings.Development.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "horror_journal"
  }
}
```

For MongoDB Atlas, replace the connection string with your Atlas URI.

---

## Running Tests

```bash
dotnet test
```

---

## Project Structure

```
HorrorJournal/
├── HorrorJournal.Api/
│   ├── Models/              # Domain models and DTOs
│   ├── Repositories/        # MongoDB repository layer
│   ├── Services/            # Index initializer, change stream service
│   ├── Endpoints/           # Minimal API endpoint definitions
│   ├── Infrastructure/      # Data seeder
│   └── Program.cs
├── HorrorJournal.Tests/     # xUnit + Moq unit tests
├── docker-compose.yml
└── CLAUDE.md                # Claude Code context file
```

---

## MongoDB Concepts Covered

This project is structured to demonstrate the following MongoDB features:

| Concept                  | Where                            |
| ------------------------ | -------------------------------- |
| Typed document mapping   | `Film.cs` + BSON attributes      |
| Filter / Update builders | `FilmRepository.cs`              |
| Text indexes             | `IndexInitializer.cs`            |
| Compound indexes         | `IndexInitializer.cs`            |
| TTL indexes              | `IndexInitializer.cs`            |
| Aggregation pipeline     | `FilmRepository.GetStatsAsync()` |
| Change streams           | `FilmChangeStreamService.cs`     |
| Singleton client pattern | `Program.cs`                     |

---

## Local Dev Tools

| Tool          | URL                            |
| ------------- | ------------------------------ |
| API (Swagger) | https://localhost:5001/swagger |
| mongo-express | http://localhost:8081          |

---

## Stretch Goals

- [ ] Atlas Search for fuzzy matching and autocomplete
- [ ] Recommendations endpoint (based on top subgenres/directors)
- [ ] TTL-based wishlist auto-expiry
- [ ] Blazor or React front end
- [ ] MCP server tool for querying the journal from Claude
