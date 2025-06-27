# Complete Example: Rate Limiting Feature

This example demonstrates the full brief-driven development process for implementing API rate limiting in a C# ASP.NET Core application.

## Example Files

The complete example has been broken down into separate files that mirror the actual process:

### Phase 1: Brief (Human writes)
- [`example-rate-limiting/brief-example.md`](example-rate-limiting/brief-example.md) - The initial request from the developer

### Phase 2: Exploration (AI creates)
- [`example-rate-limiting/exploration-example.md`](example-rate-limiting/exploration-example.md) - Analysis of the codebase and approach options

### Phase 3: Plan (AI creates)
- [`example-rate-limiting/plan-example.md`](example-rate-limiting/plan-example.md) - Detailed implementation plan with code snippets

### Phase 4: Tasks (Created together)
- [`task-summary-example.md`](example-rate-limiting/task-summary-example.md) - **Start here!** Dependency graph and execution options
- [`task-001-core-service-example.md`](example-rate-limiting/task-001-core-service-example.md) - Core rate limiting service
- [`task-002-rate-limit-attribute-example.md`](example-rate-limiting/task-002-rate-limit-attribute-example.md) - Rate limit attribute
- [`task-003-configuration-example.md`](example-rate-limiting/task-003-configuration-example.md) - Configuration support
- [`task-004-apply-to-controllers-example.md`](example-rate-limiting/task-004-apply-to-controllers-example.md) - Apply to controllers
- [`task-005-integration-tests-example.md`](example-rate-limiting/task-005-integration-tests-example.md) - Integration tests

## Key Takeaways

1. **Brief can be simple** - Just 3-5 sentences is enough to get started
2. **AI does the heavy lifting** - Explores codebase, identifies patterns, creates detailed plans
3. **Risk & Effort over Time** - Uses 🎯 and 💪 emojis instead of day estimates
4. **Multiple execution paths** - Tasks can be done sequentially, in parallel, or as MVP
5. **Clear dependencies** - Visual graph shows which tasks block others
6. **Validation criteria** - Each task has clear success metrics

## Process Benefits

- **Reduced cognitive load**: Human focuses on WHAT and WHY, AI handles HOW
- **Better decisions**: Three options with trade-offs prevent satisficing
- **Risk awareness**: Explicit risk ratings and mitigation strategies
- **Flexible execution**: Multiple paths to implementation based on urgency
- **Clear handoffs**: Each phase produces specific artifacts

This example shows how a simple request ("add rate limiting") becomes a well-planned, risk-assessed implementation with multiple execution options.