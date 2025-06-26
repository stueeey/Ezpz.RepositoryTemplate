# Brief-Driven Development Methodology

## Core Philosophy

Brief-Driven Development optimizes the human-AI collaboration between expert software engineers and AI agents. Rather than dumbing down the process, it elevates the conversation by having AI handle the tedious research and documentation work, freeing humans to focus on nuanced architectural decisions and trade-offs.

The methodology distributes work based on comparative advantage:

- **Humans provide**: Business context, architectural judgment, and critical design decisions
- **AI provides**: Deep technical research, pattern discovery, and comprehensive documentation

## Key Design Principles

### 1. Minimize Tedious Work, Maximize Design Thinking
- Briefs can be just 3-5 sentences to get started
- Humans focus on architectural decisions and trade-offs
- AI handles comprehensive research and documentation
- Options presented show meaningful architectural differences

### 2. Progressive Disclosure
- Start with minimal brief
- AI explores and discovers technical details
- Complexity revealed gradually through phases
- Each phase produces concrete artifacts

### 3. Risk & Effort Over Time
- Use abstract measures (🎯 Risk, 💪 Effort) instead of time estimates
- Risk levels: Very Low, Low, Medium, High
- Effort levels: Trivial, Easy, Medium, High, Very High
- Focuses on complexity and uncertainty rather than duration

### 4. Deep Problem Exploration
- **Research First**: Understand the problem space before jumping to solutions
- **Essential vs Accidental Complexity**: Distinguish what's truly necessary
- **Multiple Paradigms**: Explore fundamentally different approaches
- **Shape to Fit**: Adapt theoretical solutions to actual codebase patterns

## The Four-Phase Process

### Phase 1: BRIEF (Human writes)
**Purpose**: Capture the "what" and "why" with minimal effort

**Contents**:
- What to build (1-2 sentences)
- Why it matters (1 sentence)
- Non-obvious constraints (performance, business rules)
- Starting hints (vague keywords to guide exploration)

**What NOT to include**:
- Technical stack details (AI will discover)
- File locations (AI will find)
- Implementation specifics (AI will determine)

### Phase 2: EXPLORE (AI creates)
**Purpose**: Deep research into the problem space and meaningful solution alternatives

**Research Focus**:
1. **Problem Understanding**:
   - Restate the problem including implicit requirements
   - Identify what's being asked vs what's actually needed
   - Surface hidden constraints and assumptions

2. **Solution Exploration** (2-5 approaches):
   - Research existing solutions and patterns
   - Explore fundamentally different architectures
   - Understand essential vs accidental complexity
   - Each approach should represent a different paradigm, not just scope

**Examples of Meaningful Differences**:
- Event-driven vs request-response
- Centralized vs distributed
- Push vs pull mechanisms
- Synchronous vs asynchronous processing
- Different data modeling approaches

**Key behaviors**:
- Start with hints but expand research broadly
- Question assumptions in the brief
- Focus on architectural trade-offs, not just effort
- Recommend based on simplicity AND effectiveness

### Phase 3: PLAN (AI creates)
**Purpose**: Shape the chosen approach to fit elegantly with the existing codebase

**Solution Shaping**:
1. **Pattern Matching**:
   - How does the codebase handle similar problems?
   - What conventions and idioms should we follow?
   - Where does this naturally fit in the architecture?

2. **Contract Design**:
   - Well-documented interfaces and signatures
   - Integration points with existing systems
   - Configuration and extension points

3. **Simplification Pass**:
   - What could be removed without compromising functionality?
   - Are there simpler alternatives we dismissed too quickly?
   - Document rejected simplifications and why

**Key behaviors**:
- Adapt the theoretical solution to actual code patterns
- Focus on how it fits, not just what it does
- Validate it still solves the original problem

### Phase 4: REVIEW & CHUNK (Together)
**Purpose**: Simplify and create executable work items

**Process**:
1. AI presents pre-chunked options
2. Human chooses approach (MVP/Balanced/Full)
3. AI creates all task files
4. Tasks include dependencies and validation criteria

**Task naming**:
- Descriptive names: `task-001-core-service.md`
- Not just numbers: ~~`task-001.md`~~

## Critical Success Factors

### 1. Problem-First Thinking
- Always restate and validate the problem before exploring solutions
- Question whether we're solving the real problem or just the stated one
- Look for the simplest thing that could possibly work

### 2. Meaningful Architectural Choices
- Options should represent different paradigms, not just different scopes
- Each approach should have distinct trade-offs worth discussing
- Focus on essential complexity, eliminate accidental complexity

### 3. Starting Hints Save Tokens
Both general and specific hints are valuable:
- ✅ "Look at our authentication"
- ✅ "We use Redis for something similar"
- ✅ "Use the AuthorizationFilter class"
- ✅ "Check UserController.cs"

### 4. Shape Solutions to Fit
- Great solutions adapt to existing patterns
- Don't force foreign paradigms into the codebase
- Validate that shaped solutions still solve the original problem

### 5. Clear Dependencies
Visual task dependency graphs show:
- What can be done in parallel
- What blocks other work
- Multiple valid execution orders

## Open Questions for Future Iteration

### Context Management
Should we kill Claude Code between phases?

**Arguments for fresh context**:
- Enforces complete documentation
- Prevents assumption carryover
- Each phase truly standalone

**Arguments for continuous context**:
- Maintains discovered nuances
- More efficient subsequent phases
- Natural knowledge building

**Current recommendation**: Experiment with both approaches and measure outcomes.

## Measuring Success

A successful brief-driven development process should result in:
1. Deep understanding of the problem space
2. Architecturally distinct options explored
3. Solutions that fit naturally with existing patterns
4. Clear identification of essential vs accidental complexity
5. High-quality technical decisions with clear rationale
6. Executable task lists requiring minimal clarification

## Evolution

This methodology should evolve based on:
- Patterns in successful vs failed implementations
- Feedback on cognitive load
- Task completion metrics
- Team satisfaction with the process

## The Research → Shaping Flow

This methodology incorporates a two-stage design process:

### Research Phase (Part of EXPLORE)
When exploring approaches, the AI should:
1. Restate the problem to confirm understanding (including implicit requirements)
2. Research existing solutions, patterns, and technologies
3. Explore 2-4 meaningfully different approaches
4. Understand essential vs accidental complexity
5. Recommend based on simplicity and effectiveness

### Shaping Phase (Part of PLAN)
When planning the chosen approach, the AI should:
1. Understand what the approach needs to accomplish
2. Explore how similar things are done in this codebase
3. Shape the approach to fit naturally with existing patterns
4. Document what needs to be built or modified
5. Capture key contracts and interfaces
6. Validate the shaped solution still solves the original problem

Throughout both phases, the AI should ask:
- Am I solving the real problem or just the stated problem?
- What's the simplest thing that could possibly work?
- What complexity is truly necessary?

The key is elevating the technical conversation: AI handles the research and documentation heavy-lifting, while humans make nuanced architectural decisions based on comprehensive analysis.