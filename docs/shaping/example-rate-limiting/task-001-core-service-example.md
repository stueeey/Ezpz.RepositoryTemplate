# Task 001: Create Core Rate Limiting Service

[← Back to Task Summary](./task-summary-example.md)

**Size**: Medium (2 files) - 🎯 Low Risk, 💪 Easy Effort  
**Dependencies**: None (can start immediately)

## Objective

Create the core rate limiting service that handles Redis operations and rate limit logic.

## Implementation Steps

1. Create `Services/RateLimitService.cs`
    - Define `IRateLimitService` interface
    - Implement sliding window algorithm using Redis sorted sets
    - Add fail-open behavior for Redis errors
    - Include proper logging

2. Create `Models/RateLimitResult.cs`
   ```csharp
   public class RateLimitResult
   {
       public bool Allowed { get; set; }
       public int Remaining { get; set; }
       public int RetryAfter { get; set; }
   }
   ```

3. Register service in `Program.cs`
   ```csharp
   builder.Services.AddSingleton<IRateLimitService, RateLimitService>();
   ```

## Validation Criteria

- [ ] Service correctly tracks requests in 1-minute sliding window
- [ ] Old entries are cleaned up automatically
- [ ] Returns correct retry-after time when limit exceeded
- [ ] Fails open when Redis is unavailable
- [ ] Unit tests pass with mocked Redis

## Notes

- Use the existing `IConnectionMultiplexer` from DI
- Follow the error handling pattern from `CacheService.cs`
- Keys should expire after 2 minutes to prevent memory bloat