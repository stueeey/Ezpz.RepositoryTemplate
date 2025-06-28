# Shaping Workspace

This directory contains active and completed feature development work using the shaping process.

## Process Overview

The shaping process optimizes human-AI collaboration:

- **Humans provide**: Business context, architectural judgment, and critical design decisions
- **AI provides**: Deep technical research, pattern discovery, and comprehensive documentation

## Four-Phase Process

### 1. BRIEF (Human writes)

Create a `brief.md` file with 3-5 sentences covering:

- What to build (1-2 sentences)
- Why it matters (1 sentence)
- Non-obvious constraints
- Starting hints (optional)

### 2. EXPLORE (AI creates)

AI produces `exploration.md` containing:

- Problem restatement (including implicit requirements)
- 2-5 meaningful solution approaches
- At least one reframe that might eliminate the problem
- Risk and effort assessments

### 3. PLAN (AI creates)

AI produces `plan.md` that:

- Shapes the chosen approach to fit existing codebase patterns
- Documents contracts and interfaces
- Validates the solution still solves the original problem

### 4. CHUNK (Together)

Create task files:

- `task-summary.md` - Dependency graph and execution options
- `task-001-{description}.md` - Individual tasks with clear names
- Task sizes: Small (1 file), Medium (2-5 files), Large (6-10 files), XL (10+ files)

## Directory Structure

```
work/
├── feature-name/
│   ├── brief.md
│   ├── exploration.md
│   ├── plan.md
│   ├── task-summary.md
│   └── task-001-description.md
└── another-feature/
    └── ...
```

## Getting Started

1. Create a new directory for your feature
2. Write a brief.md file (see [brief template](../shaping/brief-template-simple.md))
3. Have AI create exploration and plan
4. Review and create tasks together

For detailed methodology, see [Shaping Methodology](../shaping/methodology.md).