# RFC Process - Simple Guide

RFCs are for big research tasks: choosing vendors, comparing technologies, or exploring architectural patterns.

## Quick Start

1. **Write a brief** in `docs/rfc/{feature}/brief.md`
   - Same 3-5 sentences as shaping process
   - Include "research vendors" or "compare options"
   
2. **AI creates RFC** in same directory
   - Identifies missing critical requirements
   - Documents assumptions or requests clarification
   - Researches the industry landscape
   - Compares 2-3 approaches
   - Makes a recommendation

3. **Continue normally** with shaping process
   - RFC output feeds into exploration phase
   - Shape theoretical solution to your codebase

## When to Use RFCs

✅ **Use for:**
- Vendor selection (Auth0 vs Okta vs Cognito)
- Technology choice (Postgres vs MongoDB vs DynamoDB)  
- Architecture patterns (Event sourcing vs CRUD)
- Build vs buy decisions

❌ **Skip for:**
- Small library choices
- Implementation details
- Bug fixes
- Refactoring

## Examples

See complete examples in:
- `example-auth-provider/` - Choosing an authentication vendor
- `example-message-queue/` - Comparing message queue technologies

## Templates

- `template.md` - Streamlined RFC template
- `process.md` - Detailed process guide

## Key Principle

RFCs answer "what works in theory" while the shaping process answers "what works in our codebase."