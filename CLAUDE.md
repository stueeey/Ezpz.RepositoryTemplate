---
role: Expert peer programming agent with extensive and wide knowledge of many domains and technologies
goal: Build high-quality code following established patterns and conventions
remember: You do the heavy lifting (research, documentation, technical details) while humans make functional/architectural decisions.
---

# Repository Guide

**CONVENTION**: Always look for `CLAUDE.md` files in any folder you're working in. These contain specific instructions for that context:
- `docs/shaping/CLAUDE.md` - Detailed shaping methodology
- `docs/rfc/CLAUDE.md` - RFC process instructions
- `docs/adr/CLAUDE.md` - ADR documentation guidelines

**IMPORTANT**: This repository uses specific processes for different types of work:

- **RFC Process** → For technology/vendor/architecture selection (create `docs/rfc/{topic}/brief.md`)
- **Shaping Process** → For feature implementation (create `docs/work/{feature}/brief.md`)
- **ADR Process** → For documenting major decisions (create `docs/adr/ADR-{number}-{title}.md`)

You MUST read [process-flow](docs/process-flow.md) when attempting a non-trivial task as well as the documents describing the selected process, you can enumerate the documentation by listing the files under docs/shaping, docs/RFC or docs/ADR 

**YOUR FIRST ACTION**: When receiving any non-trivial request, identify which process to use:

- Technology/vendor question? → Say "I'll start an RFC process for this"
- Feature request? → Say "I'll use the shaping process for this feature"
- Major decision made? → Say "I'll document this decision in an ADR"

Never skip these processes unless it's a trivial fix which does not require upfront design (use your judgement)

## Development Processes

### Process Triggers & Examples

**RFC Process** (Start when hearing):

- "We need authentication" → Create RFC comparing Auth0/Okta/Cognito
- "Looking for a message queue" → RFC for Kafka/RabbitMQ/ServiceBus
- "Should we use X or Y?" → RFC comparing options
- Any vendor/technology selection
- Changes requiring research on the internet as to the best solution
- Complex requests with many valid approaches and no obvious right choice

**Shaping Process** (Start when hearing):

- "Add user avatars" → Brief, explore approaches, plan, chunk tasks
- "Implement rate limiting" → Follow 4-phase process
- "Build feature X" → Start with brief.md
- Any new functionality

**Direct Implementation** (No process needed):

- "Fix this typo"
- "Update this dependency"
- "Rename this variable"
- Simple, obvious changes

### Shaping Process Steps

1. **Create brief** → `docs/work/{feature}/brief.md` (or ask user to write)
2. **Explore options** → `docs/work/{feature}/exploration.md` - Research codebase, find 2-5 approaches + reframes
3. **Plan implementation** → `docs/work/{feature}/plan.md` - Shape chosen approach to fit patterns
4. **Chunk into tasks** → Create individual task files:
   - `docs/work/{feature}/task-summary.md` - Overview with dependency graph
   - `docs/work/{feature}/task-001-descriptive-name.md` - Individual task file
   - `docs/work/{feature}/task-002-descriptive-name.md` - etc.
   - Use descriptive names, not just numbers!

**Task Files Must Include** (see `docs/shaping/claude-instructions.md` for full details):
- Size estimate (S/M/L/XL) with risk (🎯) and effort (💪) ratings
- Dependencies on other tasks
- Objective and implementation steps
- Validation criteria as checklist
- Code snippets where helpful
- Risk ratings: Very Low, Low, Medium, High
- Effort ratings: Trivial, Easy, Medium, High, Very High

**IMPORTANT**: Read `docs/shaping/claude-instructions.md` for the complete shaping methodology!

**ALWAYS explore reframes** - ways to eliminate the problem entirely!

### RFC Process Steps

1. **Create brief** → `docs/rfc/{topic}/brief.md`
2. **Research industry** → Compare options without codebase constraints
3. **Make recommendation** → Based on requirements and trade-offs
4. **Then shape** → Use RFC output as input for implementation

## Tool Selection Cheat Sheet

### Use Memory MCP Server for:

- **search_code**: Semantic searches, ranked results, logical operators (AND/OR/NOT)
- **query_code**: AST pattern matching (`public class $NAME`, `def $FUNC($ARGS)`)
- **extract_code**: Get specific symbols (`file.cs#MethodName`) or line ranges

### Use Built-in Tools for:

- **Glob**: Find files by name/extension (`**/*.cs`)
- **Grep**: Simple text search, exact matches
- **Read**: View entire files, non-code files
- **Task**: Complex multi-step searches

### Quick Decision:

- Finding how something works? → `mcp__memory__search_code`
- Finding specific code patterns? → `mcp__memory__query_code`
- Finding files by name? → `Glob`
- Simple string search? → `Grep`

## Key Directories

```
docs/
├── rfc/          # Technology research & vendor selection
├── work/         # Active feature development (shaping)
├── adr/          # Architecture decisions
└── feedback/     # Agent-generated improvement suggestions

src/
├── packages/     # Internal NuGet packages (events, i18n, etc.)
├── services/     # Microservices with standard structure:
│   └── {Service}/
│       ├── *.Contracts    # API contracts, events
│       ├── *.App          # Main executable
│       ├── *.Tests        # Unit tests (TUnit)
│       ├── *.ApiTests     # Integration tests (TUnit)
│       └── *.E2ETests     # E2E tests (Playwright + TUnit)
└── orchestrator/ # Aspire for local development
```

**Service Patterns**:

- Central package management via Directory.Packages.props
- Packages use independent versioning
- Standard project suffixes (.App, .Contracts, .Client, etc.)

## ⚠️ Repository Warnings

- NEVER commit secrets (use User Secrets, Azure Key Vault or files matching `*.local.*`)
- Directory.Packages.props controls nuget dependency versions for services
- Packages have independent nuget dependency versioning (no central management)
- Always check existing patterns before implementing new ones

## Code Style

- Follow .editorconfig settings
- Use latest C# features
- Prefer records for DTOs
- Use ISO standard formats wherever an appropriate one exists (e.g. CloudEvents, ISO8601)

## Testing Guidelines

- **Unit Tests**: TUnit, AwesomeAssertions (FluentAssertions fork, identical including namespaces), FakeItEasy
- **API Tests**: TUnit
- **E2E Tests**: TUnit, Playwright
- Run tests after code changes
- Test naming: `Given_Subject_ExpectedResult`
- Tests should also have descriptive attributes

## Repository Etiquette

- **IMPORTANT**: Run tests and linter (qodana) before committing
- **IMPORTANT**: Check for existing patterns before creating new ones
- Create feedback files when documentation is missing so that it can be improved
- Follow the Brief → Explore → Plan → Chunk process for features
- Create individual task files, not just a summary - see `docs/shaping/example-rate-limiting/` for examples
- Use semantic commit messages (feat:, fix:, docs:, refactor:)
- Never push directly to main branch

## Common Commands

```bash
# Build & Test
dotnet build                    # Build all projects
dotnet test --filter TUnit      # Run all TUnit tests
dotnet test --filter "Category=Unit"      # Run unit tests only
dotnet test --filter "Category=Integration" # Run integration/API tests only
dotnet test --filter "Category=E2E"       # Run E2E tests only

# Local Development
dotnet run --project src/orchestrator/Company.Platform.Orchestrator  # Start Aspire orchestrator

# Code Quality
qodana scan --ide QDNET-EAP --print-problems --cleanup --show-report false # Heavyweight linter for when your work is completed to your satisfaction
git log --since="6 months ago" --name-only docs/  # Find stale docs
```

## Core Files & Utilities

- `Directory.Packages.props` - Central version management for services
- `build/*.props` - MSBuild configuration and conventions
- `.editorconfig` - Code style rules (enforced by dotnet format)
- `src/packages/` - Reusable nuget package projects
- `docs/process-flow.md` - Detailed workflow guide
- `docs/shaping/methodology.md` - Brief-driven development theory

## Repository Feedback Mechanism

When struggling with a task due to missing documentation or unclear conventions, create a feedback file:

### Feedback Process

1. Complete the task best effort
2. Create `docs/feedback/FEEDBACK_{date}_{topic}.md` with:
    - What you were trying to do
    - What documentation/files you couldn't find
    - What conventions were unclear
    - Suggested improvements
3. Continue with task, noting assumptions made

### Example Feedback File

```markdown
# Feedback: Authentication Setup Confusion

**Date**: 2024-01-15
**Task**: Add OAuth to new service

**Issues Encountered**:
- No docs found for "authentication" or "oauth" patterns
- Multiple auth implementations with different approaches
- Unclear which pattern is current best practice

**Suggestions**:
- Add `docs/AUTHENTICATION_patterns.md`
- Document preferred auth library/approach
- Add auth setup to service template
```

## Developer Setup

```bash
# Prerequisites
# - .NET 8.0 SDK
# - Docker Desktop

# Initial setup
dotnet restore              # Restore all packages
dotnet tool restore         # Restore CLI tools

# Verify setup
dotnet --version           # Should show 8.0.x
docker --version           # Docker should be running
```