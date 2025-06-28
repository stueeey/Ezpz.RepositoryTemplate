# Working with AI Agents - Quick Start 🚀

This guide helps you collaborate effectively with AI agents on development tasks.

## Getting Started 💬

Just describe what you need - the agent will guide you through the right process:

```
"We need search functionality to help users find products"
"Fix the slow dashboard - customers are complaining"
"Research options for handling real-time notifications"
```

## The Right Process for Your Task 🎯

### Just Ask!

The agent will automatically choose the appropriate process:

- 🔧 **Simple fixes** → Direct implementation
- ✨ **New features** → Shaping process with exploration and planning
- 🔍 **Technology choices** → RFC research followed by shaping

### What Happens Next

1. **Agent asks clarifying questions** - Only what affects the approach
2. **You review and approve** at natural checkpoints:
    - Problem understanding (brief)
    - Technical findings (exploration)
    - Implementation approach (plan)
    - Task breakdown
3. **Implementation proceeds** with clear tasks

## Tips for Success 💡

### Start Simple

✅ Good: "Add user avatars to reduce support confusion about who said what"  
❌ Bad: "Add user avatars with S3 storage, CDN, 100x100 size, JPEG format..."

### Include the Why

✅ Good: "Add caching to fix slow product pages"  
❌ Bad: "Add caching"

### Trust the Process

- 🔍 Let the agent explore your codebase
- 🎯 Don't prescribe technical solutions
- 💼 Focus on business needs

### Provide Context When It Matters

- 💰 Budget constraints
- 📋 Compliance requirements
- ⚡ Performance targets
- ⏰ Timeline pressures

## Common Patterns 📚

### Feature Request ✨

```
You: "We need password reset to reduce support tickets"
Agent: Creates brief → Explores your auth setup → Plans implementation
You: Choose approach (MVP vs Full) → Get tasks
```

### Performance Problem ⚡

```
You: "The API is timing out for large customers"
Agent: Profiles → Identifies bottleneck → Suggests fixes
You: Approve approach → Implementation begins
```

### Technology Research 🔍

```
You: "We need a message queue for our microservices. Research options."
Agent: Creates RFC → Compares technologies → Makes recommendation
You: Choose technology → Agent plans integration
```

## Working with Artifacts 📄

The agent creates documents you can review:

- 📝 **Brief** - Problem and goals
- 🔍 **Exploration** - What the agent discovered
- 📐 **Plan** - Proposed solution
- ✅ **Tasks** - Step-by-step implementation

You can also provide your own:

```
"Implement the brief at docs/work/feature/brief.md"
"I've put research materials in docs/reference/"
```

## Advanced Usage 🎓

### Signals for Research (RFC)

These phrases trigger deeper research:

- "Research options for..."
- "How should we handle..."
- "Compare vendors..."
- "Look into different approaches..."

### When You'll See ADRs

Major architectural decisions get documented for future reference:

- 🗄️ Database technology selection
- 🏗️ Architecture pattern changes
- 🔒 Irreversible technical choices

### Streamlining Approval

For routine work, tell the agent:

- "Go with MVP approach"
- "Skip to implementation"
- "Assume standard patterns"

## Quick Reference 📋

**Full guide**: See [docs/process-flow.md](docs/process-flow.md) for detailed flows

❓ **Problem?** Just describe it  
💭 **Need the why?** Agent will ask  
🤔 **Too many questions?** Agent will explore instead  
🚦 **Wrong direction?** Correct at checkpoints  
🎮 **Want control?** Provide specific constraints

Remember: You provide business context and make decisions. The agent handles research, technical details, and
documentation.