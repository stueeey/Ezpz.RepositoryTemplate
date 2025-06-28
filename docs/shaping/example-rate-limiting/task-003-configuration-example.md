# Task 003: Add Configuration Support

[← Back to Task Summary](./task-summary-example.md)

**Size**: Small (2 files) - 🎯 Very Low Risk, 💪 Trivial Effort  
**Dependencies**: None (can work in parallel with tasks 001 & 002)

## Objective

Add configuration options for rate limiting that can be adjusted per environment without code changes.

## Implementation Steps

1. Create `Configuration/RateLimitOptions.cs`
   ```csharp
   public class RateLimitOptions
   {
       public Dictionary<string, int> PerTier { get; set; } = new();
       public Dictionary<string, int> PerEndpoint { get; set; } = new();
       public int DefaultRequestsPerMinute { get; set; } = 100;
   }
   ```

2. Update `appsettings.json`
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

3. Register configuration in `Program.cs`
   ```csharp
   builder.Services.Configure<RateLimitOptions>(
       builder.Configuration.GetSection("RateLimit"));
   ```

## Validation Criteria

- [ ] Configuration loads correctly from appsettings.json
- [ ] Can override settings in appsettings.Development.json
- [ ] Missing configuration uses sensible defaults
- [ ] Configuration is available via DI in services

## Notes

- Consider adding configuration validation on startup
- Document all available options in appsettings comments