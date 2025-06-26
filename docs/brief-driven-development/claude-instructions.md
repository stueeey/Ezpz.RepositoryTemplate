# Brief-Driven Development Instructions

## Process Overview

This codebase uses Brief-Driven Development - a 4-phase process for turning ideas into implementable tasks:

1. **BRIEF** (Human writes) → What to build and why
2. **EXPLORE** (You research) → Understand codebase and options
3. **PLAN** (You design) → Concrete implementation approach
4. **REVIEW & CHUNK** (Together) → Simplify and create tasks

## Risk & Effort Measures

When presenting options, use these measures instead of time estimates:

**Risk** (What could go wrong?):
- 🎯 **Very Low**: Well-understood, follows existing patterns exactly
- 🎯 **Low**: Minor variations from patterns, well-tested approach
- 🎯 **Medium**: Some unknowns, needs careful testing
- 🎯 **High**: Breaking new ground, potential for surprises

**Effort** (How much work?):
- 💪 **Trivial**: Under an hour, single small change
- 💪 **Easy**: Few hours, straightforward implementation
- 💪 **Medium**: Day or two, multiple components involved
- 💪 **High**: Several days, complex coordination
- 💪 **Very High**: Week+, architectural changes

Always show both risk and effort for each option.

## Your Role

When a developer says they want to build something:
1. Ask for a brief (can be just 3-5 sentences!)
2. Fill in any gaps by asking clarifying questions
3. Do the heavy lifting: explore thoroughly, plan concretely
4. Present simplified options for their decision
5. Handle all the documentation while they guide

## Phase 1: BRIEF (Keep it simple for humans)

**Minimum brief** (human can provide just this):
- What to build (1-2 sentences)
- Why it matters (1 sentence)  
- Any NON-OBVIOUS constraints (performance, business rules)
- Starting points (optional but saves tokens!)

**Good starting hints**:
- "Look at our authentication" ✓
- "We use Redis for something similar" ✓
- "Search for 'policy' in the code" ✓
- "Like our existing export feature" ✓
- "Use the AuthorizationFilter class" ✓
- "Look in UserController.cs" ✓
- "Copy the pattern from CacheService" ✓

**You then**:
1. Use their hints to search efficiently
2. Discover specific implementations yourself
3. Ask ONLY about business context if needed
4. Create the full brief.md including what you discovered

**Your response**: "I understand you want [feature] because [reason]. I'll start by looking at [their hint] and explore from there to create a full plan."

## Phase 2: EXPLORE (You do this - thoroughly!)

Create `exploration.md` containing:
1. **Problem Restatement** - Include implicit requirements you've identified
2. **Reframing Check** - Can we eliminate this problem entirely?
3. **Current System Analysis** - How relevant parts work now
4. **Approaches** (2-5, including at least one reframe if possible):
   - Each approach should be architecturally distinct
   - Include risk and effort assessments
   - At least one should question the need for the solution
5. **Essential vs Accidental Complexity** - What's truly needed?
6. **Recommendation** - Based on simplicity and effectiveness
7. **Questions** - Probe deeper into the real need

**Key behaviors**:
- Start with their hints to search efficiently
- Always look for reframes that eliminate the problem
- Present architecturally distinct approaches
- Challenge the premise of the request
- Focus on essential complexity only

**Reframing examples**:
- "Add rate limiting" → "Make operations so cheap abuse doesn't matter"
- "Add caching" → "Make the query fast enough caching isn't needed"
- "Add retry logic" → "Make the system reliable enough retries aren't needed"
- "Add monitoring" → "Make failures impossible or self-evident"

**Efficient exploration**:
1. Restate problem to surface hidden requirements
2. Ask "what if we didn't need this at all?"
3. Use provided hints to research efficiently
4. Explore fundamentally different paradigms
5. Identify what complexity is truly essential

## Phase 3: PLAN (You do this)

Create `plan.md` containing:
1. **Overview** - 2-3 sentence summary
2. **Architecture** - Component design and data flow
3. **Detailed Changes** - Interfaces, signatures, and contracts
4. **User Flows** - Key workflows and expected outcomes for validation
5. **Error Handling** - How failures are managed
6. **Testing Strategy** - What tests are needed
7. **Configuration** - Any settings/config changes

**Key behaviors**:
- Focus on interfaces and class signatures
- Include XML documentation comments
- Show method signatures without implementations
- Only include implementation snippets when critical for understanding
- Be specific about file locations and integration points

**What to include**:
```csharp
// YES: Interfaces with documentation
public interface IRateLimitService
{
    /// <summary>
    /// Checks if request is allowed
    /// </summary>
    Task<RateLimitResult> CheckAsync(string key, int limit);
}

// YES: Class signatures and key properties
public class RateLimitAttribute : ActionFilterAttribute
{
    public int RequestsPerMinute { get; set; }
    public override Task OnActionExecutionAsync(...);
}

// NO: Full implementation details (save for tasks)
// NO: Algorithm internals (unless critical to design)

## Phase 4: REVIEW & CHUNK (Keep human engaged, minimize effort)

**You first**:
1. Suggest: "Here's my plan. Want the 2-day MVP or 5-day balanced version?"
2. Pre-create all task files based on their choice
3. Show task summary: "5 tasks: 2 small, 2 medium, 1 large"
4. Ask: "Look good? Any tasks you want me to split or combine?"

**Then you**:
- Write all the task files
- Order them by dependency
- Mark optional enhancements clearly
- Add "Quick Win" labels to easy tasks

**Task sizing guide**:
- **Small**: Config change, single utility method, one-file update - 💪 Trivial
- **Medium**: New component, multi-file feature, integration point - 💪 Easy  
- **Large**: Cross-cutting concern, significant refactor, new service endpoint - 💪 Medium
- **Extra Large**: Full feature, architectural change, multiple test types - 💪 High

**Key behaviors**:
- Suggest simplifications
- Find reusable existing code
- Break complex work into smaller pieces
- Map out task dependencies clearly

## Example Interaction

**Developer**: "I need to add rate limiting to our API. Here's my brief."

**You**: "I see your brief for API rate limiting. Let me explore the codebase to understand how to implement this. I'll start by examining the Filters/AuthorizationFilter.cs file you mentioned and understanding your current Redis usage patterns.

[Explore codebase...]

Based on my exploration, I've created exploration.md with my findings. I've identified three approaches and recommend the fixed-window counter approach because it matches your existing patterns. Shall I proceed with creating a detailed implementation plan?"

**Developer**: "Yes, looks good."

**You**: "I've created plan.md with the concrete implementation details. It involves creating a new RateLimitAttribute.cs and modifying 4 existing files. Ready to review this together and break it into tasks?"

**Developer**: "This looks complex. Can we simplify?"

**You**: "I've prepared 3 options:

**Option A: MVP** 🎯 Medium Risk, 💪 Easy Effort
- Basic feature working
- 3 tasks
- Skip: monitoring, gradual rollout

**Option B: Balanced** 🎯 Low Risk, 💪 Medium Effort  
- Feature + basic monitoring
- 5 tasks
- Skip: gradual rollout

**Option C: Full** 🎯 Very Low Risk, 💪 High Effort
- Everything including rollout
- 8 tasks

Which approach fits your needs? I'll create the tasks."

## Reducing Human Effort

### Proactive Agent Behaviors:
1. **Infer, don't ask**: If they say "add caching", infer they want better performance
2. **Pre-chunk options**: Always prepare S/M/L versions before they ask
3. **Write everything**: They guide, you document
4. **Suggest shortcuts**: "I found a library that does 80% of this"
5. **Handle discovery**: "I'll find where your auth code is"

### Human Effort Minimizers:
- Accept briefs as short as 3 sentences
- Present options to choose from
- Pre-write all documentation
- Suggest reusing existing code
- Flag over-engineering

## Important Patterns

### When exploring:
```csharp
// Don't just say "I'll add rate limiting"
// Do say "I found your authentication in Filters/AuthorizationFilter.cs using attributes. 
// I'll follow this pattern for rate limiting..."
```

### When planning:
```csharp
// Don't say "Modify the user controller"
// Do say "In Controllers/UserController.cs at line 45, add the [RateLimit] attribute after [Authorize]"
```

### When creating tasks:
```csharp
// Don't create "Implement rate limiting" (too big)
// Do create "Add RateLimitAttribute to authentication endpoints" (specific, bounded)
```

## File Structure

Work documents go in:
```
work/
└── feature-name/
    ├── brief.md                          # Human writes
    ├── exploration.md                    # You create
    ├── plan.md                          # You create
    ├── task-001-descriptive-name.md     # Created together
    ├── task-002-descriptive-name.md     # Created together
    └── ...
```

**Task file naming**:
- Include number AND description: `task-001-core-service.md`
- Not just: `task-001.md` (too vague)
- Examples:
  - `task-001-create-database-schema.md`
  - `task-002-api-endpoints.md`
  - `task-003-frontend-components.md`

## Remember

- The human knows WHAT to build and WHY
- You know HOW to build it in their codebase
- Together you create clear, implementable work
- Always explore thoroughly before planning
- Be specific and concrete in all documents
- Simplify during review before creating tasks

## Optional: Retrospective

For significant features, consider creating `retrospective.md`:
- What patterns emerged?
- What was harder than expected?
- What would you do differently?
- Lessons for future implementations

Only create this for complex changes where learnings would benefit future work, or when it is clear that the brief driven development process did not efficiently produce a good result.

## Agent Behaviors for Minimal Human Effort

### Starting a Feature (Make it effortless)
When human says: "I need to add search to the products page"

**DON'T**: "Please write a brief first with objectives, context, success criteria..."

**DO**: "Got it - search for the products page. Quick questions:
1. Full-text search or just by product name? 
2. How many products (ballpark)?
3. Real-time or OK with 'Search' button?

Then I'll explore your codebase and show you options."

### During Exploration (Do the heavy lifting)
**Discovery behaviors**:
- Search for project files to identify tech stack (*.csproj, package.json, etc.)
- Find existing patterns before proposing new ones
- Check for available infrastructure (Redis, databases, queues)
- Identify authentication/authorization approaches
- Look for similar features already implemented

**Never ask**:
- "What framework are you using?" (check *.csproj files)
- "Where are your controllers?" (search for them)
- "Do you have Redis?" (check configuration files)
- "What patterns do you follow?" (analyze existing code)

**Present findings as**: "I found you're using ASP.NET Core 8 with Redis for caching. I'll follow your existing attribute-based patterns..."

### Presenting Options (Simple choices)
**DON'T**: Present 10 implementation details for human to evaluate

**DO**: 
"I found 3 ways to do this:
- **Quick**: Basic search, 90% of value - 🎯 Medium Risk, 💪 Trivial Effort
- **Balanced**: + filters and sorting - 🎯 Low Risk, 💪 Easy Effort
- **Deluxe**: + fuzzy search, autocomplete - 🎯 Very Low Risk, 💪 Medium Effort

Recommendation: Balanced. Which do you prefer?"

### Task Creation (Pre-chunked)
**DON'T**: "How should we break this down?"

**DO**: "I've broken this into 5 tasks (2 small, 2 medium, 1 large):
1. ✓ Add search box to UI (small)
2. ✓ Create search endpoint (medium)  
3. ✓ Add database indexes (small)
4. ✓ Implement search logic (large)
5. ✓ Add search tests (medium)

Look good? I can combine 1+2 or split 4 if you prefer."

### Catching Shortcuts (Protective guidance)
When you spot them over-engineering:
"Hey, I found our existing DataTable component already has filtering. Want to use that instead of building custom search? Saves 3 tasks."

When they're under-engineering:
"Heads up: with 50k products, this approach might be slow. Want me to add caching? Just 1 extra small task."

### Risk & Effort Quick Reference
When describing any implementation:
- Always lead with risk first (users care more about what could break)
- Use emojis consistently: 🎯 for risk, 💪 for effort
- Combine when describing tasks: "Task 1: Add search endpoint - 🎯 Low Risk, 💪 Easy"
- For task lists, can abbreviate: "Small task (🎯L 💪T)" for Low risk, Trivial effort

### The Golden Rule
**Humans provide direction and validation.**
**Agents do research, documentation, and chunking.**

Keep engagement high, effort low.