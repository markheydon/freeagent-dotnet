---
title: Pagination
parent: How-to
nav_order: 1
---

# Pagination

FreeAgent list endpoints return one page at a time. The SDK follows the Stripe.NET pattern: **`ListAsync`** for one page and **`ListAutoPagingAsync`** to iterate every page without manual `page` management.

## Single-page access

Use when you need explicit control over page number and page size (`per_page` maximum is 100).

```csharp
using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

var page = await client.Contacts.ListAsync(
    page: 1,
    perPage: 25,
    view: ContactViews.Active);

Console.WriteLine($"Page {page.Page}: {page.Items.Count} of ~{page.Total} contacts");
Console.WriteLine($"Has next page: {page.HasNextPage}");
```

## Auto-pagination

Use when you want every item without managing page numbers:

```csharp
await foreach (var contact in client.Contacts.ListAutoPagingAsync(perPage: 50))
{
    Console.WriteLine(contact.DisplayName);
}
```

Cancellation is honoured during pagination loops - pass a `CancellationToken` to stop early.

## List filters (Contacts)

The Contacts list supports `view`, `sort`, and `updated_since` query parameters. See the [Contacts reference](../reference/contacts.md) for supported values.

```csharp
using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

var page = await client.Contacts.ListAsync(
    page: 1,
    perPage: 25,
    view: ContactViews.Clients,
    sort: "-updated_at",
    updatedSince: DateTimeOffset.Parse("2025-03-15T09:00:00.000Z"));
```
