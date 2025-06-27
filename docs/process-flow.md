# Development Process Flow Guide

This guide shows how humans and AI agents collaborate through different complexity levels of work, from simple tasks to major architectural decisions.

## Starting Points

Work can begin in two ways:

### 1. Inline Prompt
User provides request directly to agent:
```
"We need to add search functionality to help users find products faster"
```
Agent creates brief from the conversation.

### 2. Written Brief
User provides location of existing brief:
```
"Please implement the brief at docs/work/search/brief.md"
"I've put some research docs in docs/reference/search-vendors/"
```
Agent starts from documented requirements.

## Quick Decision Tree

```
User has a task
    ↓
Is it exploring vendors/technologies? → YES → Start RFC Process
    ↓ NO
Does it need implementation? → YES → Start Shaping Process
    ↓ NO
Just do it directly (bug fix, simple refactor)
```

### RFC Signals
These phrases/patterns indicate RFC may be needed:
- "Research options for..."
- "Look into different approaches..."
- "Find out which technology..."
- "How should I handle..."
- Brief references external research materials
- Comparing vendors or technologies
- Build vs buy decisions

## Process Flows by Complexity

### 1. Simple Task (Direct Implementation)

**Example**: "Fix the typo in the login button"

```
Human: Points out issue
   ↓
Agent: Makes the change directly
   ↓
Done
```

No process needed - just fix it.

### 2. Small Feature (Shaping Process)

**Example**: "Add pagination to the products list to improve page load times"

```
Human: "I need pagination on the products page to improve load times 
        when we have 1000+ products"
   ↓
Agent: "Quick questions: How many items per page? Style preference?"
   ↓
Human: "20 items, simple numbered pages"
   ↓
Agent: Creates brief from conversation
   ↓
APPROVAL GATE: Human reviews brief
   ↓
Agent: Explores codebase → Creates exploration.md
   ↓
APPROVAL GATE: Human reviews findings
   ↓
Agent: Plans implementation → Creates plan.md
   ↓
APPROVAL GATE: Human chooses approach (MVP/Balanced/Full)
   ↓
Agent: Creates task files
   ↓
Human: Implements tasks
```

**Key files created**:
- `docs/work/pagination/brief.md`
- `docs/work/pagination/exploration.md`
- `docs/work/pagination/plan.md`
- `docs/work/pagination/task-*.md`

### 3. Large Feature (Shaping Process with Research)

**Example**: "Implement real-time notifications to improve user engagement with order updates"

```
Human: "We need real-time notifications for order updates to reduce 
        support calls about order status"
   ↓
Agent: "Let me understand the requirements..."
   - Asks about scale, delivery guarantees, client types
   ↓
Human: Provides context
   ↓
Agent: Creates comprehensive brief
   ↓
APPROVAL GATE: Human reviews and refines brief
   ↓
Agent: Explores deeply:
   - Researches WebSockets vs SSE vs polling
   - Analyzes existing patterns
   - Considers infrastructure
   ↓
APPROVAL GATE: Human reviews exploration
   ↓
Agent: Plans with multiple approaches
   ↓
APPROVAL GATE: Human makes architectural decisions
   ↓
Agent: Creates detailed task breakdown
   ↓
Implementation proceeds in phases
```

### 4. Technology Selection (RFC → Shaping Process)

**Example**: "We need a search solution to help users find products in our growing catalog"

```
Human: "We need search for 1M+ products with facets to improve 
        conversion rates. Research options. I've saved some vendor 
        comparisons in docs/reference/search/"
   ↓
Agent: Recognizes RFC signals → Creates RFC brief
   ↓
Agent: Researches industry solutions + provided materials
   ↓
APPROVAL GATE: Human reviews RFC brief
   ↓
Agent: Produces RFC comparing Elasticsearch vs Algolia vs Azure Search
   - Documents assumptions
   - Highlights gaps needing clarification
   ↓
APPROVAL GATE: Human reviews RFC, makes vendor decision
   ↓
Human: "Let's implement Elasticsearch" (new brief)
   ↓
Agent: Uses RFC as input for shaping process
   ↓
Normal shaping flow continues (with approval gates)
```

**Key files created**:
- `docs/rfc/search-solution/brief.md`
- `docs/rfc/search-solution/rfc.md`
- `docs/work/elasticsearch-integration/brief.md`
- `docs/work/elasticsearch-integration/exploration.md`
- etc.

## Communication Patterns

### Human Initiates Loosely
```
Human: "I'm thinking about adding caching"
Agent: "What specific performance issue are you solving? 
        Is this for database queries, API responses, or something else?"
Human: "Product API is slow"
Agent: "I'll analyze the current performance and explore caching options"
```

### Agent Identifies Missing Context
```
Agent: "I see you need message queuing. Your requirements could lead to 
        very different solutions. Key question: Do you need message replay?"
Human: "No, just fire and forget"
Agent: "That eliminates Kafka and simplifies to RabbitMQ vs Azure Service Bus"
```

### Agent Surfaces Hidden Complexity
```
Human: "Add user avatars"
Agent: "I'll explore this. Note: this involves storage strategy, CDN, 
        image processing, and GDPR compliance for user data."
Human: "Oh, I didn't think about GDPR. Keep it simple - just URLs for now"
```

## Key Interaction Principles

### 1. Progressive Detail
- Start with minimal input from human
- Agent asks only clarifying questions that change the approach
- Details emerge through exploration, not interrogation

### 2. Agent Does Heavy Lifting
- Human provides intent and constraints
- Agent handles research, documentation, technical details
- Human makes business/architectural decisions

### 3. Clear Handoff Points
Each phase produces concrete artifacts:
- **Brief**: Captures intent
- **Exploration/RFC**: Provides options
- **Plan**: Shows concrete approach
- **Tasks**: Enable implementation

### 4. Flexible Formality
- 3-sentence brief for simple features
- Detailed RFC for vendor selection
- Process scales with complexity

## Approval Gates

The process includes natural stopping points where human approval is needed:

### Shaping Process Gates
1. **Brief Approval**: Confirm the problem/solution framing is correct
2. **Exploration Approval**: Review findings before planning
3. **Plan Approval**: Choose approach (MVP/Balanced/Full)
4. **Task Approval**: Confirm task breakdown before implementation

### RFC Process Gates
1. **RFC Brief Approval**: Confirm research scope
2. **RFC Approval**: Make technology/vendor decision
3. Then normal shaping gates for implementation

### Why Gates Matter
- Prevent wasted effort on wrong direction
- Ensure human judgment at critical decisions
- Create natural checkpoints for course correction
- Document decision rationale at each stage

### Streamlining Gates
For trusted agents or routine work:
- Combine brief + exploration review
- Auto-approve MVP approach for experiments
- Batch approve related tasks

## When Each Process Shines

### Use Direct Implementation When:
- Fix is obvious and isolated
- No architectural decisions needed
- Following existing patterns exactly

### Use Shaping Process When:
- Building new features
- Need to explore codebase patterns
- Multiple valid approaches exist

### Use RFC Process When:
- Evaluating vendors or technologies
- Making build vs buy decisions
- Exploring industry solutions
- Decision has long-term impact

## Architecture Decision Records (ADRs)

ADRs document major technology or architectural decisions that affect the entire system. Create an ADR when:

- Choosing a fundamental technology (database engine, messaging system)
- Changing architectural patterns (monolith → microservices)
- Making irreversible or expensive-to-change decisions
- Decision affects multiple teams or services
- Future developers need to understand "why" not just "what"

### ADR Process
```
Major decision identified (during RFC or implementation)
   ↓
Agent or Human drafts ADR
   ↓
APPROVAL GATE: Architecture review
   ↓
ADR saved to docs/adr/
   ↓
Future RFCs/briefs reference the ADR
```

Example: After choosing PostgreSQL over MongoDB via RFC, create an ADR documenting why this decision was made for the entire platform.

## Common Scenarios

### Scenario: "Make it faster"
```
Human: "The dashboard is slow, customers are complaining"
Agent: "I'll profile the current implementation. What's your performance target?"
Human: "Under 2 seconds"
Agent: Explores → Finds N+1 queries → Plans optimization
```

### Scenario: "Add standard feature"
```
Human: "We need password reset to reduce support tickets"
Agent: "I'll check your current auth setup and email infrastructure..."
→ Follows shaping process
```

### Scenario: "Major architectural change"
```
Human: "We need to go multi-region to support our EU expansion"
Agent: "This requires database, caching, and CDN strategy. Should I research solutions?"
Human: "Yes, I've put the latency requirements in docs/reference/"
→ RFC process → ADR for architecture → Multiple shaping implementations
```

## Tips for Humans

1. **Start simple**: "I need X" is enough to begin
2. **Provide context when it matters**: Budget, scale, constraints
3. **Trust the process**: Let agent explore before deciding
4. **Make decisions at the right level**: Architecture vs implementation details

## Tips for Agents

1. **Minimize questions**: Explore first, ask only when critical
2. **Show your work**: "I found you're using X, so I'll follow that pattern"
3. **Flag hidden complexity**: Surface non-obvious implications
4. **Recommend process**: Suggest RFC when appropriate
5. **Document assumptions**: Make decision basis transparent

## Process Combinations

Large projects often combine processes:

```
Multi-region Architecture:
├── RFC: Database Strategy
├── RFC: CDN Selection
├── Brief: Implement database sharding
├── Brief: Add CDN support
└── Brief: Update deployment pipeline
```

Each piece uses the appropriate process for its complexity and scope.