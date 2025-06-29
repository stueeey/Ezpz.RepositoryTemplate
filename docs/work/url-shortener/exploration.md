# URL Shortener Service - Implementation Exploration

## Approach Options

### Option 1: REST API with SQLite Database
**Pros:**
- Simple HTTP API for creating/redirecting URLs
- SQLite requires no server setup (file-based)
- Perfect for demos and small-scale deployments
- Easy to integrate with web frontends
- Follows common URL shortener patterns

**Cons:**
- Limited concurrent write performance
- May need caching layer for high traffic

**Implementation:**
- ASP.NET Core Web API
- Entity Framework Core with SQLite
- Redis for caching frequent lookups (optional)

### Option 2: gRPC Service with NoSQL
**Pros:**
- Follows existing Domain service pattern
- Better for service-to-service communication
- NoSQL (e.g., CosmosDB) scales well for key-value lookups

**Cons:**
- gRPC less natural for URL redirects
- Requires HTTP gateway for browser access

### Option 3: Hybrid - REST Frontend, gRPC Backend
**Pros:**
- REST API for public-facing operations
- gRPC for internal service communication
- Separation of concerns

**Cons:**
- More complex architecture
- Two APIs to maintain

### Option 4: Event-Driven with CQRS
**Pros:**
- Separates read (redirect) from write (create)
- Can scale reads independently
- Event sourcing provides audit trail

**Cons:**
- Over-engineered for simple URL shortening
- Complex for a demonstration

### Option 5: Reframe - Use Existing CDN/Edge Service
**Pros:**
- No custom service needed
- Leverage existing infrastructure
- Better performance at edge

**Cons:**
- May not demonstrate repository patterns
- Less control over features

## Recommendation: Option 1 - REST API with SQLite Database

This approach:
- Demonstrates clear repository patterns
- Shows database migrations via Migrator project
- Uses familiar REST patterns for easy understanding
- SQLite makes it easy to run locally without dependencies
- Can add Redis caching later if needed
- Follows the structure seen in documentation examples

## Technical Decisions
1. **API Style**: REST (more natural for URL operations)
2. **Database**: SQLite with EF Core
3. **Caching**: In-memory cache initially (can add Redis later)
4. **ID Generation**: Base62 encoding of sequential IDs
5. **URL Validation**: Basic format checking
6. **Metrics**: Track click count in database