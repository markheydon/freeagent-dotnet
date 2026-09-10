# Turpinverse canon snapshots

Bundled demo data for the Blazor sample app. Files are **read-only snapshots** copied from the upstream [Turpinverse](https://github.com/markheydon/turpinverse) repository (`canon/` folder). This SDK repository does not modify Turpinverse; it only vendors the JSON files needed to seed FreeAgent API probes.

Refresh snapshots from upstream when canon changes:

```bash
curl -fsSL "https://raw.githubusercontent.com/markheydon/turpinverse/main/canon/<file>.json" \
  -o samples/FreeAgent.Client.BlazorSample/canon/<file>.json
```

## Bundled files

| File | Records | Used by |
|------|---------|---------|
| `organisations.json` | 10 | Contact seeding (`TurpinverseContactSeeder`) |
| `personas.json` | 25 | Primary contact lookup for organisation contacts |
| `invoices.json` | 12 | Reserved for Invoices endpoint probes (#46) |

Add further canon files here as SDK resources are implemented (for example `bills.json`, `quotes.json`). Do not bundle files for endpoints that do not yet exist in the SDK.

## Mapping rules

- **Contacts** — one FreeAgent B2B contact per organisation (`tradingName` + primary contact persona name/email/phone; `registeredOffice` for billing address). See `Services/Turpinverse/`.

Upstream canon documentation: [turpinverse/canon/README.md](https://github.com/markheydon/turpinverse/blob/main/canon/README.md).
