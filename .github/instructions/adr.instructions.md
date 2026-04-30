---
description: 'ADR-specific authoring guidance'
applyTo: 'adr/*.md'
---

# ADR Instructions

## Primary Authoring Path
- Use the `create-architectural-decision-record` skill for ADR creation and major ADR updates.
- Keep ADR content precise, decision-focused, and explicit about alternatives and consequences.

## Location and Naming
- Store ADR files in repository-root `adr/`.
- Use filename pattern `adr-NNNN-[title-slug].md` with a 4-digit sequence.

## Consistency Rules
- Preserve decision history and avoid rewriting accepted decisions without documenting status changes.
- Keep references and cross-links accurate when ADR files are moved or renamed.
