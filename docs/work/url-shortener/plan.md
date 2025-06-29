# URL Shortener Service - Implementation Plan

## Service Structure
Following the repository patterns, we'll create:
```
src/services/UrlShortener/
├── Company.Platform.UrlShortener.App/          # REST API application
├── Company.Platform.UrlShortener.Contracts/    # Shared contracts/DTOs
├── Company.Platform.UrlShortener.Client/       # HTTP client library
├── Company.Platform.UrlShortener.Migrator.App/ # Database migrations
├── Company.Platform.UrlShortener.App.Tests/    # Unit tests (TUnit)
├── Company.Platform.UrlShortener.ApiTests/     # Integration tests (TUnit)
└── Company.Platform.UrlShortener.E2ETests/     # E2E tests (TUnit + Playwright)
```

## Core Components

### 1. Database Schema (SQLite)
```sql
CREATE TABLE ShortUrls (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShortCode TEXT NOT NULL UNIQUE,
    OriginalUrl TEXT NOT NULL,
    ClickCount INTEGER DEFAULT 0,
    CreatedAt TEXT NOT NULL,
    ExpiresAt TEXT
);

CREATE INDEX IX_ShortUrls_ShortCode ON ShortUrls(ShortCode);
```

### 2. API Endpoints
- `POST /api/shorten` - Create short URL
- `GET /{shortCode}` - Redirect to original URL
- `GET /api/urls/{shortCode}` - Get URL details
- `GET /api/urls/{shortCode}/stats` - Get click statistics

### 3. Core Services
- `IUrlShortenerService` - Business logic
- `IShortCodeGenerator` - Generate unique short codes
- `IUrlRepository` - Database operations

### 4. DTOs (in Contracts)
- `CreateShortUrlRequest`
- `ShortUrlResponse`
- `UrlStatsResponse`

## Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite with EF Core
- **Caching**: IMemoryCache (built-in)
- **Validation**: FluentValidation
- **Testing**: TUnit, FakeItEasy, AwesomeAssertions

## Integration Points
1. Register with Aspire orchestrator
2. Add to Directory.Packages.props for dependencies
3. Follow existing logging patterns
4. Use standard health checks

## Key Design Decisions
1. **Short Code Generation**: Sequential ID + Base62 encoding
2. **URL Validation**: Basic format check, no external validation
3. **Caching**: Cache popular URLs in memory (LRU)
4. **Expiration**: Optional expiration date support
5. **Rate Limiting**: Use existing repository patterns