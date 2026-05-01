---
description: 'Documentation authoring guidance for Markdown files'
applyTo: '**/*.md'
---

# Markdown Documentation Instructions

## Primary Authoring Agent
- Use the `Tech Writer` agent as the default authoring path for Markdown documentation content.
- Apply Diataxis-aligned writing style using project context from `GOALS.md`, `SCOPE.md`, `CONVENTIONS.md`, and `.github/copilot-instructions.md`.

## Scope
- This rule applies to project documentation, guides, references, plans, and other non-ADR Markdown files.
- Prioritise consistency in terminology, tone, and document structure across repository docs.

## ADR Exception
- For Architectural Decision Records under `adr/*.md`, do not use `Tech Writer` as the primary path.
- Use the `create-architectural-decision-record` skill and follow repository ADR location/pattern policy.
