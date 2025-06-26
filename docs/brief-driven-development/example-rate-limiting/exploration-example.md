# Exploration: Add API Rate Limiting

## Current System Analysis

### Authentication Flow
Traced through the request pipeline:

1. All API requests hit `Filters/AuthorizationFilter.cs`
2. The `[Authorize]` attribute triggers JWT validation
3. User object is stored in `HttpContext.Items["User"]`
4. User has properties: Id, Email, Role, SubscriptionTier

Found in `Filters/AuthorizationFilter.cs`:
```csharp
public class AuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = ExtractToken(context.HttpContext);
        var user = ValidateToken(token);
        context.HttpContext.Items["User"] = user;
    }
}
```

### Redis Usage Patterns
Found in `Services/RedisService.cs`:
- Using StackExchange.Redis with connection multiplexing
- Existing patterns for caching with expiration
- Key naming: `{service}:{entity}:{id}`
- Fail-open pattern on Redis errors

Example from CacheService.cs:
```csharp
public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiry)
{
    try
    {
        var cached = await _redis.StringGetAsync(key);
        if (cached.HasValue)
            return JsonSerializer.Deserialize<T>(cached);
    }
    catch (RedisException ex)
    {
        _logger.LogWarning(ex, "Cache read failed, continuing without cache");
    }
    
    var result = await factory();
    
    try
    {
        await _redis.StringSetAsync(key, JsonSerializer.Serialize(result), expiry);
    }
    catch (RedisException ex)
    {
        _logger.LogWarning(ex, "Cache write failed, continuing");
    }
    
    return result;
}
```

## Three Approaches

### 1. Quick & Dirty - 🎯 Medium Risk, 💪 Easy Effort
- Fixed window counter per user
- Single global limit for all endpoints
- No gradual degradation
- Pros: Simple, fast to implement
- Cons: Burst traffic at window boundaries, no per-endpoint control

### 2. Balanced (Recommended) - 🎯 Low Risk, 💪 Medium Effort  
- Sliding window using sorted sets
- Per-endpoint limits with defaults
- Headers showing limit status
- Pros: Smooth rate limiting, good visibility
- Cons: Slightly more Redis operations

### 3. Robust - 🎯 Very Low Risk, 💪 High Effort
- Token bucket algorithm
- Per-endpoint and per-user-tier limits
- Gradual degradation with queuing
- Admin dashboard for monitoring
- Pros: Most flexible, production-ready
- Cons: Complex implementation, more testing needed

## What Could Go Wrong
- Redis connection failures (mitigated by fail-open)
- Memory pressure from tracking many users
- Clock skew in distributed environment
- Legitimate burst traffic blocked
- Performance impact on high-traffic endpoints

## My Confidence
**Medium-High** - Rate limiting is well-understood, and we have good patterns to follow. Main uncertainty is around the exact Redis performance impact.

## Recommendation
Go with **Balanced approach** - provides good protection without over-engineering, follows your existing patterns, and can be enhanced later if needed.