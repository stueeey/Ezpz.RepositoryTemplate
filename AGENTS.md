
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
**Design Process Flow:** research → discovery → plan → ADR

- **packages.md** - Internal packages from other repositories
- **design/**
  - **research/** - Problem definition, requirements, constraints
  - **discovery/** - What changes are needed to codebase
  - **plan/** - Proposed implementation changes
  - **adr/** - Architectural Decision Records
- **reference/** - External materials (git-ignored)
- **how-to/** - Task guides with templates

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
- **infrastructure/** - Bicep modules and service configs
- **scripts/** - Automation tools and setup