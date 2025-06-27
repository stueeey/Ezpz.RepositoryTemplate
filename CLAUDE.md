
## Structure & Conventions

```
.
├── CLAUDE.md                    # AI documentation (this file)
├── HUMANS.md                    # Human-focused AI usage guide
├── README.md                    # Primary human documentation
├── .editorconfig               # Code style with LLM comments
├── .gitignore                  # Version control config
├── docs/
│   ├── packages.md             # External package listing
│   ├── design/
│   │   ├── research/           # Problem definition
│   │   ├── discovery/          # Change analysis
│   │   ├── plan/              # Implementation plans
│   │   └── adr/               # Architecture decisions
│   ├── reference/             # External docs (git-ignored)
│   └── how-to/                # Task guides
├── src/
│   ├── packages/              # Internal NuGet packages
│   │   └── {domain}/          # e.g., events, i18n
│   ├── services/              # Microservices
│   │   ├── Directory.Packages.props
│   │   └── {service}/
│   │       ├── *.Contracts
│   │       ├── *.App
│   │       ├── *.Migrator.App
│   │       ├── *.Client
│   │       ├── *.App.Tests
│   │       ├── *.ApiTests
│   │       └── *.E2ETests
│   ├── orchestrator/          # Aspire local dev
│   └── specifications/        # API contracts
│       ├── openapi/
│       └── protobuf/
├── build/                     # MSBuild configuration
│   ├── *.props               # Build settings
│   └── conventions/          # Project type imports
├── artifacts/                # Build outputs
├── pipelines/               # CI/CD workflows
├── infrastructure/          # Bicep IaC
└── scripts/                # Automation tools
```

### Root Files
- **CLAUDE.md** - Main AI documentation (concise, always in context)
- **HUMANS.md** - Human-focused guidance for using AI agents
- **README.md** - Primary human documentation
- **.editorconfig** - Code style with LLM context comments
- **.gitignore** - Version control configuration

### Documentation (/docs)
**Process:** RFC → Shaping (BRIEF → EXPLORE → PLAN → CHUNK)

- **packages.md** - Internal packages from other repositories
- **rfc/** - Request for Comments (large-scale research)
    - **{feature}-rfc/** - Deep research on vendors/technologies
        - `rfc.md` - Analysis and recommendations
        - `appendix-*.md` - Optional deep-dive reports
- **work/** - Shaping workspace
    - **{feature}/** - Feature directories containing:
        - `brief.md` - Human: what/why (3-5 sentences)
        - `exploration.md` - AI: research & approaches (including reframes)
        - `plan.md` - AI: shaped implementation
        - `task-summary.md` - Together: dependencies & execution options
        - `task-XXX-{name}.md` - Descriptively named tasks
- **adr/** - Architecture Decision Records (major technology choices only)
- **reference/** - External materials (git-ignored)
- **how-to/** - Task guides with templates
- **shaping/** - Shaping methodology and examples

### Source Code (/src)
- **packages/** - Internal NuGet packages by domain
    - No central package management (target minimum versions)
    - Auto-publish as NuGet on build
    - Move to separate repo when stable (low frequency changes)
    - Example domains: events, internationalization, packaging

- **services/** - Microservices (deploy to cloud)
    - Central package management via Directory.Packages.props
    - Standard structure per service:
        - `.Contracts` - API contracts, events, metrics, manifest
        - `.App` - Main executable (publishes as Docker container)
        - `.Migrator.App` - Database migration host
        - `.Client` - Service client library
        - `.App.Tests` - Unit tests using xunit
        - `.ApiTests` - Integration tests using NUnit
        - `.E2ETests` - End-to-end tests using playwright + NUnit

- **orchestrator/** - Aspire orchestrator for local development

- **specifications/** - API contracts
    - **openapi/** - OpenAPI specifications and fragments
    - **protobuf/** - Shared protobuf schemas

### Build System (/build)
- **Analysis.props** - Code analysis and warnings
- **Tooling.props** - Build configuration
- **LanguageFeatures.props** - .NET language features
- **RepositoryInfo.props** - Repository metadata
- **conventions/** - MSBuild imports for project types

### Other Directories
- **artifacts/** - Build outputs (packages, binaries, specifications)
- **pipelines/** - CI/CD workflows (build, test, deploy)
- **infrastructure/** - IAC modules and service configs
- **scripts/** - Automation tools and setup

## Instructions

When exploring the code, consider using the MCP server named "Memory"

## Shaping Process

Elevates human-AI collaboration by distributing work based on comparative advantage:
- **Humans**: Business context, architectural judgment, critical decisions
- **AI**: Research, pattern discovery, comprehensive documentation

### 4-Phase Process:
1. **BRIEF** (human) → What/why in `docs/work/{feature}/brief.md` (3-5 sentences)
2. **EXPLORE** (AI) → Research problem & 2-5 approaches (including reframes) in `exploration.md`
3. **PLAN** (AI) → Shape chosen approach to fit codebase in `plan.md`
4. **CHUNK** (together) → S/M/L/XL tasks with dependencies in `task-XXX-{name}.md`

Task sizes: Small (1 file), Medium (2-5 files), Large (6-10 files), XL (10+ files)

**Key principle**: AI should always explore reframes - ways to eliminate the problem entirely rather than solve it. Examples:
- "Add caching" → "Make operation so fast caching isn't needed"
- "Add rate limiting" → "Make operations so cheap abuse doesn't matter"

## RFC Process (Large-Scale Research)

For larger work requiring extensive vendor/technology research:

1. **Brief** (human) → Same starting point
2. **RFC Research** (AI) → Deep dive into solutions without codebase constraints
3. **RFC Document** → Theoretical approaches and recommendations
4. **Then Shaping Process** → Shape RFC recommendations to fit codebase

Use RFCs when:
- Evaluating vendors (Auth0 vs Okta vs Cognito)
- Choosing technologies (Kafka vs RabbitMQ vs Service Bus)
- Exploring paradigms (event sourcing vs state-based)

RFC outputs focus on "what works in theory" while the shaping process determines "what works in our codebase".

## Process Flow Summary

[Full details in docs/process-flow.md]

### Starting Work
- **Inline prompt**: User describes need → You create brief
- **Written brief**: User provides `docs/work/{feature}/brief.md`
- **Research materials**: If user mentions `docs/reference/`, likely needs RFC

### Decision Tree
1. Vendor/technology choice? → RFC Process
2. Needs implementation? → Shaping Process  
3. Simple fix? → Just do it

### RFC Signals
Watch for: "research", "look into", "find out", "how should I", references to external materials

### Approval Gates
Always pause for human approval at:
- Brief review (problem framing)
- Exploration/RFC review (findings)
- Plan review (approach selection)
- Task review (before implementation)

### Creating ADRs
Document major decisions that:
- Affect entire system architecture
- Are expensive/hard to reverse
- Future devs need to understand

Remember: You do the heavy lifting (research, documentation, technical details) while humans make business/architectural decisions.