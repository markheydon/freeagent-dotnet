# Goals review

Run the `goals-review` skill (`.agents/skills/goals-review/SKILL.md`) against this repository.

If I provided a PR number, branch name, or `@`-mentioned paths after this command, limit the audit to that scope; otherwise perform a whole-repo health check.

Run the skill's **mandatory List API mechanical audit** against every `*Service.cs` under `src/FreeAgent.Client/Services/` - do not sample. Compare against Stripe.NET's `List` / `ListAutoPagingAsync` pattern and this repo's `ListAsync` / `ListAutoPagingAsync` convention ([adr-0012-list-api-naming.md](../../adr/adr-0012-list-api-naming.md)). Include the per-service **List API inventory** table in the report and flag any entity using legacy `Get*PageAsync`, `GetAll*Async`, or other `Get*` collection names.

Deliver the structured report from the skill. Do not change code unless I explicitly ask you to fix findings.
