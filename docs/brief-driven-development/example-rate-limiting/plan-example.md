# Plan: Add API Rate Limiting

## Overview
Implement sliding window rate limiting using Redis sorted sets, with per-endpoint configuration and proper HTTP headers for client visibility.

## Architecture

```
[Request] → [RateLimitAttribute] → [RateLimitService] → [Redis]
                ↓                         ↓
         [429 Response]          [Continue to Controller]
```

## Detailed Changes

### 1. Create Rate Limit Attribute and Policy Enum
**File**: `Attributes/RateLimitPolicy.cs` (new)
```csharp
/// <summary>
/// Defines how rate limits are applied
/// </summary>
public enum RateLimitPolicy
{
    /// <summary>
    /// Rate limit per authenticated user (default)
    /// </summary>
    PerUser,
    
    /// <summary>
    /// Rate limit per IP address
    /// </summary>
    PerIP,
    
    /// <summary>
    /// Rate limit per endpoint (shared across all callers)
    /// </summary>
    PerEndpoint,
    
    /// <summary>
    /// Rate limit per user and endpoint combination
    /// </summary>
    PerUserAndEndpoint
}
```

**File**: `Attributes/RateLimitAttribute.cs` (new)
```csharp
/// <summary>
/// Applies rate limiting to controllers or actions based on configurable policies.
/// When rate limit is exceeded, returns 429 status with headers:
/// - X-RateLimit-Limit: The rate limit
/// - X-RateLimit-Remaining: Requests remaining in current window
/// - Retry-After: Seconds until retry allowed
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RateLimitAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Maximum requests allowed per minute (default: 100)
    /// </summary>
    public int RequestsPerMinute { get; set; } = 100;
    
    /// <summary>
    /// Rate limit policy (default: PerUser)
    /// </summary>
    public RateLimitPolicy Policy { get; set; } = RateLimitPolicy.PerUser;
    
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next);
}
```

### 2. Create Rate Limit Service
**File**: `Services/RateLimitService.cs` (new)
```csharp
/// <summary>
/// Service for checking and enforcing rate limits using Redis
/// </summary>
public interface IRateLimitService
{
    /// <summary>
    /// Checks if a request is allowed under the rate limit
    /// </summary>
    /// <param name="key">Unique identifier for the rate limit bucket</param>
    /// <param name="limit">Maximum requests allowed per minute</param>
    /// <returns>Result indicating if request is allowed and retry information</returns>
    Task<RateLimitResult> CheckRateLimitAsync(string key, int limit);
}

/// <summary>
/// Result of a rate limit check
/// </summary>
public class RateLimitResult
{
    /// <summary>
    /// Whether the request is allowed
    /// </summary>
    public bool Allowed { get; set; }
    
    /// <summary>
    /// Number of requests remaining in the current window
    /// </summary>
    public int Remaining { get; set; }
    
    /// <summary>
    /// Seconds until the client should retry (when Allowed is false)
    /// </summary>
    public int RetryAfter { get; set; }
}

/// <summary>
/// Redis-based implementation using sliding window algorithm
/// </summary>
public class RateLimitService : IRateLimitService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RateLimitService> _logger;
    
    public RateLimitService(IConnectionMultiplexer redis, ILogger<RateLimitService> logger);
    
    public Task<RateLimitResult> CheckRateLimitAsync(string key, int limit);
}
```

### 3. Update Startup Configuration
**File**: `Program.cs` (line 47, after Redis configuration)
```csharp
// Add rate limiting
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();
builder.Services.Configure<RateLimitOptions>(
    builder.Configuration.GetSection("RateLimit"));
```

### 4. Add Configuration
**File**: `Configuration/RateLimitOptions.cs` (new)
```csharp
/// <summary>
/// Configuration options for rate limiting
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Default requests per minute if not specified
    /// </summary>
    public int DefaultRequestsPerMinute { get; set; } = 100;
    
    /// <summary>
    /// Rate limits per subscription tier
    /// </summary>
    public Dictionary<string, int> PerTier { get; set; } = new();
    
    /// <summary>
    /// Rate limits per specific endpoint
    /// </summary>
    public Dictionary<string, int> PerEndpoint { get; set; } = new();
}
```

**File**: `appsettings.json` (line 23, after Redis section)
```json
"RateLimit": {
  "DefaultRequestsPerMinute": 100,
  "PerTier": {
    "Free": 100,
    "Pro": 500,
    "Enterprise": 2000
  },
  "PerEndpoint": {
    "POST /api/auth/login": 10,
    "POST /api/auth/register": 5
  }
}
```

### 5. Apply to Controllers
**File**: `Controllers/UserController.cs` (line 12)
```csharp
[Authorize]
[RateLimit(RequestsPerMinute = 200)] // Class level
public class UserController : ControllerBase
{
    [HttpPost("bulk-import")]
    [RateLimit(RequestsPerMinute = 10)] // Method level override
    public async Task<IActionResult> BulkImport([FromBody] BulkImportRequest request)
    {
        // Existing code...
    }
}
```

## User Flows

### Flow 1: Normal Request
1. User makes API request
2. RateLimitAttribute checks Redis
3. Request count is under limit
4. Request proceeds normally
5. Headers show remaining requests

### Flow 2: Rate Limited
1. User exceeds rate limit
2. RateLimitAttribute returns 429
3. Response includes Retry-After header
4. Client waits and retries

### Flow 3: Redis Down
1. Redis connection fails
2. Service logs error
3. Request proceeds (fail open)
4. No rate limit applied

## Error Handling
- Redis failures: Log and fail open
- Missing user context: Use IP-based limiting
- Invalid configuration: Use defaults

## Testing Strategy
1. Unit tests for RateLimitService logic
2. Integration tests with Redis test container
3. Load tests to verify performance impact
4. Chaos tests for Redis failure scenarios

## Configuration
All limits configurable in appsettings.json without code changes. Can be overridden per environment.