---
name: new-project-setup-mh
description: >
  Bootstrap a new project's standing documents from a completed Project Kickoff Spec.
  Generates GOALS.md, SCOPE.md, AGENTS.md, CONVENTIONS.md, .github/copilot-instructions.md,
  the first ADR (technology choices), and an initial wave of GitHub Issues
  for MVP scope items and user journeys.
argument-hint: Paste your completed Project Kickoff Spec, or tell me the path to the file
model: GPT-4.1
---

## When to use this prompt

- **When you have a completed Project Kickoff Spec** and a fresh (or near-empty) repo.
- **After running** `Install-ProjectBootstrap.ps1` so the standard root templates and `.github` assets are already present in the repo.
- **Before writing any code** - this sets up the context that guides everything else.
- **Time:** 10–20 minutes depending on spec detail and number of MVP items.

## What you'll get

- `GOALS.md` - project intent and success criteria
- `SCOPE.md` - in/out of scope for v1.0
- `AGENTS.md` - AI agent operating rules for this repo
- `CONVENTIONS.md` - code style and naming decisions (with .NET defaults, flagged for review)
- `.github/copilot-instructions.md` - Copilot context file for this repo
- the first ADR covering the tech stack decision
- GitHub Issues for each MVP scope item (labelled `story`, with MVP context captured in the issue body)
- GitHub Issues for each key user journey (labelled `story`, with spec follow-up captured in the issue body)

## What comes next

After this prompt completes:
1. **Review and customise** `CONVENTIONS.md` and `.github/copilot-instructions.md` - they contain .NET defaults that need project-specific detail.
2. **Review ADR-0001** and fill in any alternatives or rationale the prompt could not infer.
3. **Create a Feature Mini Spec** for each user journey issue before starting work on it.

---

## Step 0 - Gather inputs

Before starting, confirm you have everything needed. Ask for anything missing.

This prompt assumes the repo has already been bootstrapped with `Install-ProjectBootstrap.ps1`.
If the expected root template files are missing, stop and tell the user to run the bootstrap script first.

**Required:**
- A completed **Project Kickoff Spec** (sections 1–13). Paste it directly or provide a file path.
- The **GitHub repo** this project lives in (owner/repo format).
- The **tech stack**: framework, database, test library, key NuGet packages.

**Optional but helpful:**
- Preferred architecture pattern (defaults to Clean Architecture / CQRS if not specified).
- Any team or deployment constraints not captured in the spec.

Summarise back what you've received before proceeding.

---

## Step 1 - GOALS.md

Populate the existing `GOALS.md` in the repo root.

Source material:
- Section 2 (Problem Statement) → **Why This Exists** paragraph
- Section 4 (Goals) → **Goals for v1.0** bullet list (use the G1/G2/G3 labels)
- Section 11 (Success Criteria) → **Success Looks Like** section
- Section 12 (Kill Criteria) → **Kill Criteria** bullet list
- Section 5 (Non-Goals) → **What This Is NOT For** section

Set **Last updated** to today's date. Set **Revision History** with today's date and "Initial draft - project kickoff".

Confirm the file has been created before moving to Step 2.

---

## Step 2 - SCOPE.md

Populate the existing `SCOPE.md` in the repo root.

Source material:
- Section 6 (MVP Scope) → **In Scope - v1.0**
- Section 5 (Non-Goals) → **Out of Scope - v1.0**
- Any items in Section 6 or the spec notes marked as "later" or "v2" → **Possible v2 Candidates**

Set **Last updated** to today's date.

Confirm the file has been created before moving to Step 3.

---

## Step 3 - AGENTS.md

Update the existing `AGENTS.md` in the repo root.

Use the copied standard template. In the **Context Files** section, list:
- `GOALS.md`
- `SCOPE.md`
- `CONVENTIONS.md`
- `.github/copilot-instructions.md`

Confirm the file has been created before moving to Step 4.

---

## Step 4 - CONVENTIONS.md

Populate the existing `CONVENTIONS.md` in the repo root.

Use .NET Clean Architecture defaults for the Project Structure and Patterns sections.
Note any constraints from Section 8 of the kickoff spec.

Add this note at the top: `> ⚠️ Review and update this file once tech stack choices are finalised. The structure and patterns below are .NET Clean Architecture defaults.`

Set **Last updated** to today's date.

Confirm the file has been created before moving to Step 5.

---

## Step 5 - .github/copilot-instructions.md

Create `.github/copilot-instructions.md`.

Source material:
- Section 1 (Summary) → **Project Context** (2–3 sentences)
- Tech stack provided → **Tech Stack** section (framework, database, test library, key packages, with versions if known)
- Section 8 (Constraints) → note any architectural constraints under **Architecture**
- Use standard .NET Clean Architecture defaults for **Coding Conventions** and **Naming** sections
- Add a note at the top of those sections: `> ⚠️ Review and customise these defaults for your project before relying on them.`

Include a **What Not to Do** section with these standard entries:
- Do not put business logic in controllers
- Do not generate migrations automatically - flag when a migration is needed
- Do not add NuGet packages without flagging it first
- Do not change the architecture pattern without an ADR

Include a **GitHub Issues** section:
- When suggesting work to be done, format it as a GitHub Issue using the story issue template
- Link issues to GOALS.md goals where relevant

Confirm the file has been created before moving to Step 6.

---

## Step 6 - First ADR

Create the first ADR using the `create-architectural-decision-record` skill with these inputs:
- Decision title: "Core Technology Stack Selection"
- Context: from Section 8 (Constraints and Assumptions) and Section 9 (Dependencies) of the kickoff spec
- Decision: the tech stack provided
- Alternatives: if not provided in the spec, list common alternatives to each chosen technology and note they were not selected (ask the developer to fill in rejection reasons)
- Stakeholders: the project owner from the spec header

Confirm the ADR has been created before moving to Step 7.

---

## Step 7 - GitHub Issues: MVP scope items

For each item in **Section 6 (MVP Scope)** of the kickoff spec, create a GitHub Issue:

- **Title:** the scope item, written as an actionable task (e.g. "Implement user authentication")
- **Labels:** `story`
- **Body:**
  ```
  ## Description

  As a [infer user type from Section 3], I can [scope item], so that [infer benefit from GOALS.md].

  ## Acceptance Criteria and Tasks

  - [ ] [Derive 2–3 tasks from the scope item - keep them high level]

  ## Notes

  Source: Project Kickoff Spec, Section 6 (MVP Scope)
  Delivery context: This is part of the MVP scope for the first release.
  Related goal: [reference the most relevant goal from GOALS.md]

  > ⚠️ A Feature Mini Spec should be completed before work begins on this issue.
  ```

Confirm each issue has been created (with issue number) before moving to Step 8.

---

## Step 8 - GitHub Issues: key user journeys

For each item in **Section 7 (Key User Journeys)** of the kickoff spec, create a GitHub Issue:

- **Title:** "Journey: [journey name]"
- **Labels:** `story`
- **Body:**
  ```
  ## Description

  This issue tracks the user journey: **[journey name]**.

  ## What needs to happen before work starts

  - [ ] Complete a Feature Mini Spec for this journey (use `templates/FEATURE-MINI-SPEC.md`)
  - [ ] Break the journey into individual story issues
  - [ ] Update this issue once the spec is done and link to the spec file

  ## Notes

  Source: Project Kickoff Spec, Section 7 (Key User Journeys)
  ```

Confirm each issue has been created (with issue number) before completing.

---

## Step 9 - Summary

Output a completion summary in this format:

```
## New Project Setup Complete ✅

### Files Created
- GOALS.md
- SCOPE.md
- AGENTS.md
- CONVENTIONS.md
- .github/copilot-instructions.md
- [ADR file created by `create-architectural-decision-record`]

### GitHub Issues Created
- #[n] - [title] (story)
- #[n] - Journey: [name] (story)
[...]

### Items Flagged for Review
- CONVENTIONS.md - review project structure and patterns once tech stack is finalised
- .github/copilot-instructions.md - review Coding Conventions and Naming sections

### Suggested Next Step
Review the generated standing documents, then start the first scoped feature with a Feature Mini Spec:

  templates/FEATURE-MINI-SPEC.md
```
