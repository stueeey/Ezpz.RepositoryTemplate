# Documentation Index

This directory contains all platform documentation.

## Getting Started

- **[Process Flow Guide](process-flow.md)** - How humans and AI collaborate on different types of work
- **[Shaping Process](shaping/)** - For implementing features
- **[RFC Process](rfc/)** - For technology research and vendor selection

## Process Overview

```
Have a task?
    ↓
Choosing technology? → RFC Process → Then Shaping
    ↓ NO
Building feature? → Shaping Process
    ↓ NO
Simple fix? → Just do it
```

## Key Documents

### Process Guides

- `process-flow.md` - Complete interaction flows
- `rfc/process.md` - RFC process details
- `shaping/methodology.md` - Shaping process theory

### Templates

- `rfc/template.md` - RFC template
- `shaping/brief-template-simple.md` - Brief template

### Examples

- `rfc/example-auth-provider/` - Choosing an auth vendor
- `rfc/example-message-queue/` - Comparing message queues
- `shaping/example-rate-limiting/` - Implementing rate limiting

## Quick Reference

### When to Use Each Process

**Direct Implementation**:

- Bug fixes
- Typos
- Following existing patterns exactly

**Shaping Process**:

- New features
- Refactoring
- Anything needing design decisions

**RFC Process**:

- Vendor selection
- Technology choices
- Build vs buy decisions
- Major architectural patterns