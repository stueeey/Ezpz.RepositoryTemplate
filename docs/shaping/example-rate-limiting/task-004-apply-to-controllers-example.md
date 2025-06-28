# Task 004: Apply Rate Limiting to Controllers

[← Back to Task Summary](./task-summary-example.md)

**Size**: Medium (4-5 files) - 🎯 Medium Risk, 💪 Easy Effort  
**Dependencies**: Tasks 002 & 003 (needs attribute and configuration)

## Objective

Apply rate limiting to existing controllers with appropriate limits based on endpoint sensitivity.

## Implementation Steps

1. Update `Controllers/AuthController.cs`
   ```csharp
   [HttpPost("login")]
   [RateLimit(RequestsPerMinute = 10)]
   public async Task<IActionResult> Login([FromBody] LoginRequest request)
   
   [HttpPost("register")]  
   [RateLimit(RequestsPerMinute = 5)]
   public async Task<IActionResult> Register([FromBody] RegisterRequest request)
   ```

2. Update `Controllers/UserController.cs`
   ```csharp
   [Authorize]
   [RateLimit(RequestsPerMinute = 200)] // Class level default
   public class UserController : ControllerBase
   ```

3. Update `Controllers/ProductController.cs`
   ```csharp
   [HttpGet("search")]
   [RateLimit(RequestsPerMinute = 300)] // Higher limit for read operations
   
   [HttpPost("bulk-import")]
   [RateLimit(RequestsPerMinute = 5)] // Low limit for expensive operations
   ```

4. Create middleware to read tier-based limits from configuration
    - Check user's subscription tier
    - Apply appropriate limit from configuration
    - Fall back to defaults if not configured

## Validation Criteria

- [ ] Authentication endpoints have strict limits
- [ ] Read operations have higher limits than writes
- [ ] Expensive operations have appropriate protection
- [ ] Tier-based limits work correctly for authenticated users
- [ ] Anonymous users get default limits

## Rollback Plan

If issues arise, remove attributes from affected controllers. The service will continue to work without rate limiting.

## Notes

- Start with conservative limits
- Monitor logs for 429 responses
- Adjust limits based on actual usage patterns
- Consider exempting health check endpoints