# Upgrading

FreeAgent.NET follows [Semantic Versioning](https://semver.org/). Until MVP completion, all releases are **prerelease** (`0.x.y-alpha.n`).

## Prerelease expectations

- Public APIs may change between alpha releases.
- Pin an exact version in your project file:

```xml
<PackageReference Include="FreeAgent.Client" Version="0.1.0-alpha.1" />
```

## Upgrade checklist

1. Read the [GitHub Release notes](https://github.com/markheydon/freeagent-dotnet/releases) for breaking changes.
2. Update the package reference and restore.
3. Run your test suite — model mapping changes (for example `string` to enum, `DateTime` to `DateOnly`) are the most common breaks during alpha.
4. Check [API coverage](../reference/api-coverage.md) for newly implemented endpoints.

## Stable releases

Stable `1.0.0` will follow the criteria in [VERSIONING.md](../../VERSIONING.md). Until then, treat every upgrade as potentially breaking.
