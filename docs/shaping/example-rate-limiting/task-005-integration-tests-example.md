# Task 005: Add Integration Tests

[← Back to Task Summary](./task-summary-example.md)

**Size**: Medium (3 files) - 🎯 Low Risk, 💪 Medium Effort  
**Dependencies**: Tasks 001, 002, 004 (needs working implementation)

## Objective
Create comprehensive integration tests to verify rate limiting works correctly in various scenarios.

## Implementation Steps

1. Create `RateLimitTests/RateLimitIntegrationTests.cs`
   ```csharp
   [TestFixture]
   public class RateLimitIntegrationTests : IntegrationTestBase
   {
       [Test]
       public async Task Should_AllowRequestsUnderLimit()
       
       [Test]
       public async Task Should_Return429WhenLimitExceeded()
       
       [Test]
       public async Task Should_ResetAfterTimeWindow()
       
       [Test]
       public async Task Should_ApplyDifferentLimitsPerEndpoint()
   }
   ```

2. Create `RateLimitTests/RateLimitRedisFailureTests.cs`
   - Test fail-open behavior
   - Verify logging on Redis errors
   - Ensure requests aren't blocked

3. Create `RateLimitTests/RateLimitPerformanceTests.cs`
   - Verify < 50ms latency added
   - Test under high concurrency
   - Memory usage remains stable

## Validation Criteria
- [ ] All test scenarios pass
- [ ] Tests use TestContainers for Redis
- [ ] Performance tests confirm < 50ms overhead
- [ ] Chaos tests verify fail-open behavior
- [ ] Tests can run in CI pipeline

## Test Data
- Use test users with different subscription tiers
- Generate burst traffic patterns
- Simulate Redis connection failures

## Notes
- Reuse existing `IntegrationTestBase` infrastructure
- Consider adding load tests separately
- Document any flaky tests and mitigation