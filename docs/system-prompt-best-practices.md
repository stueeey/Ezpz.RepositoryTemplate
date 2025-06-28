---
title: Claude-Code Instruction Library
version: 1.1.0
audience: LLM Agents, LLM-ops engineers & developers
purpose: |
  This document defines how to create, name, and maintain instruction files that steer Claude-Code's behaviour during software-development tasks while remaining discoverable through simple pattern-based (ripgrep) searches.
---

# 1 Overview

Claude-Code can only locate help files by filename or explicit `read_file`
commands. For small repositories (< 500 docs), use **readable filenames with HTML comment keywords, a concise index
in `CLAUDE.md`, and process-organized folders** to keep both the model and humans productive.

---

# 2 File Taxonomy

| Layer           | Filename pattern         | Format    | Contents                                         | Token goal |
|-----------------|--------------------------|-----------|--------------------------------------------------|------------|
| **Seed**        | `CLAUDE.md`              | MD + YAML | Role, goal, guard-rails, index, tool cheat sheet | ≤ 1.5k     |
| **Core policy** | `POLICY_compliance.md`   | Markdown  | Legal & safety must-not rules                    | ≤ 600      |
| **Style guide** | `STYLE_testing.md`       | Markdown  | Tone, code style, testing idioms                 | ≤ 800      |
| **Process**     | `{process}/` folders     | Markdown  | RFC, Shaping, ADR processes                      | variable   |
| **Domain lore** | `DOMAIN_<topic>.md`      | Markdown  | Institutional or repository-specific hints       | variable   |
| **Examples**    | `EXAMPLE_<feature>.md`   | Markdown  | Canonical implementations                        | variable   |
| **Feedback**    | `feedback/FEEDBACK_*.md` | Markdown  | Agent-generated improvement suggestions          | ≤ 400      |

*One concept per file; avoid mixing unrelated topics.*

---

# 3 Naming & Discoverability

For repositories with < 100 docs files, use HTML comments for discoverability:

1. **Readable names** – use natural naming that matches your domain
2. **Keyword stuffing** – add HTML comment at top of each file:  
   `<!--search: cache redis ttl expiry session distributed-->`
3. **Process folders** – organize by process: `/rfc/`, `/work/`, `/adr/`
4. **Shallow tree** – keep `/docs/<files>` at depth ≤ 2

---

# 4 `CLAUDE.md` Template

```markdown
---
role: "Expert peer programmer"
goal: "Work with the user to produce working code of excellent quality, following the rules and conventions laid out in this document and the documentation index"
---

# 🛡️ Guard-rails
1. ☑️ **Never output secrets.**
2. Follow corporate policy (`POLICY_compliance.md`).

# 📚 Documentation Index
docs/process-flow.md: Development workflow and decision trees
docs/rfc/: Technology research and vendor selection process
docs/work/: Active feature development using shaping process
docs/adr/: Architecture decisions and rationale
docs/**/*.md: All other documentation
src/**/README.md: Service/package specific docs

# 🛠️ Tool Selection Cheat Sheet
[Include Memory MCP vs built-in tools guidance]

# 📝 Repository Feedback Mechanism
[Include feedback process for missing docs]
```

---

# 5 Companion File Patterns

### Additional File Types

- **EXAMPLE_*.md** - Canonical implementations showing best practices
- **docs/adr/ADR-{number}-{title}.md** - Architecture decisions with rationale
- **docs/feedback/FEEDBACK_{date}_{topic}.md** - Agent-generated improvement suggestions

### 5.1 Example: Domain Documentation

````markdown
<!--search: cache redis ttl expiry session distributed memory-->
# Redis Caching Patterns

## ☑️ Rules
- **SetAsync** with default TTL = 60 min
- ☑️ **MUST** wrap calls in `IAsyncPolicy cacheRetry`
- Never cache PII

```csharp
await redis.SetAsync(key, value, TimeSpan.FromMinutes(60));
```
````

### 5.2 Example: Feedback File

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

---

# 6 Formatting Guidelines

- YAML front-matter ≤ 50 lines
- Headings ≤ 6 words; imperative verbs
- One bullet = one rule
- Fence examples with triple back-ticks
- Wrap critical instruction blocks with ☑️ or §§ delimiters

---

# 7 Maintenance with Git

Use git to identify stale documentation:

```bash
# Find docs not updated in 6 months
git log --since="6 months ago" --name-only --pretty=format: docs/ | sort | uniq > recent.txt
find docs -name "*.md" | sort > all.txt
comm -23 all.txt recent.txt  # Shows stale files

# Alternative: Find by last modified date
find docs -name "*.md" -mtime +180 -exec ls -la {} \;
```

---

# 8 Do & Don't Checklist

| ✔️ Do                                               | ❌ Don't                                       |
|-----------------------------------------------------|-----------------------------------------------|
| Use HTML comments for keyword discoverability       | Create complex naming schemes for small repos |
| Keep documentation index current in CLAUDE.md       | Assume the model remembers newly added files  |
| Create feedback files when documentation is missing | Silently work around missing docs             |
| Provide runnable examples in technical docs         | Mix multiple domains in one doc               |
| Use git history to identify stale documentation     | Let outdated docs accumulate                  |
| Follow existing process names (RFC, Shaping, ADR)   | Invent new terminology                        |

---

# 9 Summary

For repositories with < 500 documentation files:

1. Use natural, readable filenames
2. Add HTML comments with search keywords
3. Maintain a clear index in CLAUDE.md
4. Create feedback files when docs are missing
5. Use git to prune stale documentation
6. Follow established process names and patterns

The goal is discoverability without complexity - let the repository's natural organization shine through while ensuring
Claude can find what it needs.