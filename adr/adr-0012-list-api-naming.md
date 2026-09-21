---
title: "ADR-0012: List API Naming (Stripe.NET Parity)"
status: "Accepted"
date: "2026-09-21"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "public-api", "developer-experience", "pagination"]
supersedes: ""
superseded_by: ""
---

# ADR-0012: List API Naming (Stripe.NET Parity)

## Status

**Accepted**

## Context

**G2** in [GOALS.md](../GOALS.md) sets Stripe.NET-quality developer experience as a floor. The SDK initially used `Get*` for all read operations, including collections. Paginated resources then gained asymmetric names (`GetContactsPageAsync`, `GetAllContactsAsync`) to disambiguate single-page access from auto-pagination.

Stripe.NET uses a clearer split:

- `Get` / `Retrieve` for a single resource
- `List` / `ListAsync` for one page (or one response) of a collection
- `ListAutoPaging` / `ListAutoPagingAsync` for automatic iteration across pages

The prior `Get*Page` / `GetAll*` pairing worked but was less discoverable in IntelliSense and did not mirror the library consumers already know from Stripe.NET.

## Decision

Adopt Stripe.NET-aligned list naming on resource services:

- **DEC-001**: Collection reads use `ListAsync` on the resource service (for example `client.Contacts.ListAsync`). The service name supplies resource context; the method name does not repeat the resource plural.
- **DEC-002**: Paginated resources also expose `ListAutoPagingAsync`, implemented on top of `ListAsync` (no duplicate HTTP logic).
- **DEC-003**: Single-resource reads remain `Get*Async` (for example `GetContactAsync`, `GetProjectAsync`).
- **DEC-004**: When one service exposes multiple distinct collection endpoints, use descriptive `List*` method names (for example `CompanyService.ListBusinessCategoriesAsync`, `CompanyService.ListTaxTimelineAsync`).

## Consequences

### Positive

- **POS-001**: Improves discoverability - `ListAsync` and `ListAutoPagingAsync` appear together and match Stripe.NET mental models.
- **POS-002**: Clarifies the read verb split: `Get*` = one resource, `List*` = collection.
- **POS-003**: Reduces naming ceremony (`GetContactsPageAsync` → `ListAsync` on `ContactService`).

### Negative

- **NEG-001**: Breaking change for existing alpha consumers (acceptable pre-1.0).
- **NEG-002**: `ListAsync` overload resolution depends on service type (`client.Contacts.ListAsync` vs `client.Projects.ListAsync`) - same as Stripe.

## Alternatives Considered

### Keep `Get*PageAsync` / `GetAll*Async`

- **ALT-001**: **Description**: Retain the emergent `Get*`-everywhere convention with `Page` and `All` suffixes.
- **ALT-002**: **Rejection Reason**: Less obvious pairing; weaker alignment with G2 Stripe.NET-quality intent.

### Use `ListContactsAsync` / `ListContactsAutoPagingAsync`

- **ALT-003**: **Description**: Repeat the resource name in the method (for example `ListContactsAsync`).
- **ALT-004**: **Rejection Reason**: Verbose; Stripe uses `List` on the typed service without repeating the resource noun.

## Implementation Notes

- **IMP-001**: Rename Contacts and Projects paginated methods; rename non-paginated collection methods (`Users`, `Categories`, `EmailAddresses`) to `ListAsync`.
- **IMP-002**: Update tests, sample app, docs, `CONVENTIONS.md`, and `implement-endpoint` / `goals-review` skills in the same change set.
- **IMP-003**: Document the migration in [docs/how-to/upgrading.md](../docs/how-to/upgrading.md).

## References

- [GOALS.md](../GOALS.md) - G2, G3
- [CONVENTIONS.md](../CONVENTIONS.md) - list method naming
- [Stripe.NET CustomerService.List / ListAutoPaging](https://github.com/stripe/stripe-dotnet/blob/master/src/Stripe.net/Services/Customers/CustomerService.cs)
