# Task 002: Create Rate Limit Attribute

[← Back to Task Summary](./task-summary-example.md)

**Size**: Small (1 file) - 🎯 Low Risk, 💪 Trivial Effort  
**Dependencies**: Task 001 (needs IRateLimitService)

## Objective
Create an action filter attribute that can be applied to controllers and actions to enforce rate limits.

## Implementation Steps

1. Create `Attributes/RateLimitAttribute.cs`
   - Inherit from `ActionFilterAttribute`
   - Add configurable `RequestsPerMinute` property
   - Override `OnActionExecutionAsync`
   - Generate rate limit key based on user/IP
   - Return 429 with proper headers when limited

2. Headers to include:
   - `X-RateLimit-Limit`: The rate limit
   - `X-RateLimit-Remaining`: Requests remaining
   - `Retry-After`: Seconds until retry

## Validation Criteria
- [ ] Attribute can be applied at class and method level
- [ ] Method level overrides class level limits
- [ ] Returns 429 status with correct headers
- [ ] Includes helpful error message in response body
- [ ] Works with both authenticated and anonymous users

## Example Usage
```csharp
[RateLimit(RequestsPerMinute = 100)]
public class ProductController : ControllerBase
{
    [HttpPost]
    [RateLimit(RequestsPerMinute = 10)] // Override for this endpoint
    public async Task<IActionResult> Create(Product product)
    {
        // Implementation
    }
}
```