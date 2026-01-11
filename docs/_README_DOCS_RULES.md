# Documentation Rules - READ FIRST

**This document defines the rules and structure for documentation in this project.**
**AI (Claude Code) MUST read this file before creating or modifying any documentation.**

---

## Document Lifecycle

Documents follow a clear lifecycle based on their directory location:

```
planning/  →  reference/  →  archived/
(proposed)    (current)      (historical)
```

| Directory | Status | Description |
|-----------|--------|-------------|
| `planning/` | Proposed/In-Progress | Not yet implemented. Plans, proposals, designs under review. |
| `reference/` | Current/Authoritative | Implemented and up-to-date. The source of truth. |
| `guides/` | Current/Actionable | How-to guides that can be executed. |
| `status/` | Live/Frequently Updated | Current state information (updated regularly). |
| `status/issues/` | Active Issues | Discovered issues to be addressed separately. |
| `archived/` | Historical | Completed plans, old specs. Reference only, not authoritative. |
| `archived/resolved_issues/` | Resolved Issues | Issues that have been resolved. |

---

## Directory Structure

```
docs/
├── _README_DOCS_RULES.md   # THIS FILE - Read first
├── INDEX.md                # Document index - Navigation hub
│
├── reference/              # Current specifications and designs
│   ├── architecture-overview.md
│   ├── api-specification.md
│   └── ...
│
├── guides/                 # How-to guides and tutorials
│   ├── development-setup.md
│   ├── deployment.md
│   └── ...
│
├── status/                 # Current state (frequently updated)
│   ├── feature-status.md
│   ├── known-limitations.md
│   └── issues/             # Discovered issues to be addressed separately
│
├── planning/               # Proposed/in-progress work
│   ├── proposals/          # Feature proposals
│   └── designs/            # Design documents under review
│
└── archived/               # Historical reference
    ├── completed-plans/    # Plans that have been implemented
    ├── legacy-specs/       # Old specifications (superseded)
    └── resolved_issues/    # Issues that have been resolved
```

---

## Rules for AI Documentation

### Rule 1: Check INDEX.md First

Before creating a new document, check `INDEX.md` to see if a relevant document already exists. Update existing documents rather than creating new ones.

### Rule 2: Place Documents in Correct Directory

- New specifications → `reference/`
- Setup/deployment guides → `guides/`
- Status reports → `status/`
- Discovered issues (unrelated to current task) → `status/issues/`
- Proposals/plans → `planning/`
- Completed plans → `archived/`
- Resolved issues → `archived/resolved_issues/`

### Rule 3: Update INDEX.md When Adding/Moving Documents

Every document must be listed in `INDEX.md` with:
- Document name and path
- Status indicator
- Brief description

### Rule 4: Move Documents Through Lifecycle

When a plan is implemented:
1. Move the plan from `planning/` to `archived/`
2. Create or update the corresponding document in `reference/`
3. Update `INDEX.md` to reflect the changes

### Rule 5: No Duplicate Documents

If the same topic exists in multiple files:
1. Consolidate into a single authoritative document
2. Place in the appropriate directory
3. Delete duplicates

### Rule 6: Language Requirements

- All documentation must be written in **English**
- Code comments and examples in English
- Exception: User-facing responses may be in the user's preferred language

---

## Status Indicators for INDEX.md

Use these indicators in INDEX.md:

| Indicator | Meaning |
|-----------|---------|
| `[CURRENT]` | Up-to-date and authoritative |
| `[LIVE]` | Frequently updated status information |
| `[DRAFT]` | Work in progress |
| `[REVIEW]` | Pending review/approval |
| `[ARCHIVED]` | Historical reference only |

---

## Quick Reference

**Before writing documentation, ask:**

1. Does a document for this topic already exist? → Check INDEX.md
2. What is the document's lifecycle stage? → Choose correct directory
3. Is this a guide, spec, status, or plan? → Match to directory type
4. Have I updated INDEX.md? → Required for all changes

**After writing documentation:**

1. Add/update entry in INDEX.md
2. Verify document is in correct directory
3. Remove any duplicate content elsewhere
