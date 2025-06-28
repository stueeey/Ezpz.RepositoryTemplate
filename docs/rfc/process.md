# RFC Process Guide

## When to Use RFCs

Use an RFC when the brief requires:

- Vendor evaluation (SaaS, PaaS, tools)
- Technology selection (databases, message queues, frameworks)
- Paradigm choices (architectures, patterns)
- Industry research (standards, best practices)

## How to Start

1. **Write a brief** in `docs/rfc/{feature}/brief.md`
    - Same format as shaping process
    - Include hint: "research vendors" or "explore options"

2. **AI creates RFC** in `docs/rfc/{feature}/rfc.md`
    - First identifies any missing critical requirements
    - Either requests clarification OR documents assumptions
    - Conducts web research
    - Compares approaches theoretically
    - Makes recommendations

3. **Continue with shaping process**
    - Create `docs/work/{feature}/brief.md` (can reference RFC)
    - AI uses RFC as input for exploration
    - Shape theoretical solution to fit codebase

## Critical Requirements Identification

The spirit of RFC research is to surface hidden requirements that would fundamentally change the recommendation. Think
like a consultant: what questions would make the client say "Oh, I didn't think of that, but yes that's critical!"

### The Right Mindset

Ask yourself:

- What unstated requirement could make my top recommendation completely wrong?
- What assumption am I making that might not be true?
- What question would reveal a deal-breaker for certain approaches?

### Examples of Game-Changing Discoveries

**Message Queue**: "Do you need message replay?" → If yes, Kafka becomes compelling
**Database**: "Will this be multi-region?" → Changes everything about consistency models
**Auth**: "Do you have B2B customers who need their own SSO?" → Eliminates simple solutions
**Search**: "Do you need real-time indexing?" → Rules out many batch-oriented solutions

### How to Handle Gaps

When you identify a critical gap:

1. **If it flips the recommendation**: Stop and ask for clarification
2. **If it affects scoring**: Document your assumption and note how different answers change things
3. **If it's nice-to-have**: Note it in "would benefit from clarification"

The goal is intellectual honesty - don't recommend based on incomplete information when that information would change
everything.

## What Makes a Good RFC Brief

### Example 1: Clear vendor research

```
We need a search solution that can handle 1M+ products with faceted search, 
autocomplete, and typo tolerance. 

Cloud-hosted preferred. Budget ~$1000/month.

Research both vendors and open source options.
```

### Example 2: Technology comparison

```
We need to choose a primary database for our new microservices. 
Must support ACID transactions, horizontal scaling, and have good .NET support.

Compare both SQL and NoSQL options.
```

### Example 3: Architecture pattern

```
We're building a system that needs to handle 100K concurrent users with 
real-time updates. 

Research architectures for real-time systems at scale.
```

## Key Differences from Shaping Process

| Aspect      | RFC                             | Shaping Process            |
|-------------|---------------------------------|----------------------------|
| Focus       | What works in theory            | What works in our codebase |
| Research    | Industry-wide                   | Codebase-specific          |
| Output      | Technology recommendations      | Implementation plan        |
| Constraints | Business/technical requirements | Existing patterns/code     |

## Tips for Effective RFCs

1. **Be specific about constraints** - Budget, scale, technical requirements
2. **Request research explicitly** - "Research vendors", "Compare options"
3. **Mention special concerns** - Compliance, vendor lock-in, operational complexity
4. **State preferences** - Cloud vs on-prem, build vs buy, OSS vs commercial

## Anti-Patterns

❌ **Don't use RFCs for**:

- Simple implementation questions
- Codebase-specific patterns
- Small library choices
- Bug fixes or refactoring

✅ **Do use RFCs for**:

- Major technology decisions
- Vendor evaluations
- Architecture patterns
- Industry best practices