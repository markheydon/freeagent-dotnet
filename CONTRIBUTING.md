# Contributing to FreeAgent.NET

Thanks for your interest in contributing to FreeAgent.NET.

This project aims to provide a modern, strongly typed .NET SDK for the FreeAgent REST API with a predictable developer experience.

## Before You Start

- Read [GOALS.md](GOALS.md) for project intent and success criteria.
- Read [SCOPE.md](SCOPE.md) to confirm your change is in scope.
- Read [CONVENTIONS.md](CONVENTIONS.md) for architecture and naming conventions.
- Read [VERSIONING.md](VERSIONING.md) for compatibility and release-stage expectations.
- For behaviour and design constraints in the sample app, read [AGENTS.md](AGENTS.md).

## Ways to Contribute

- Report bugs using the bug report template.
- Propose new capabilities using the feature request template.
- Improve documentation, tests, and sample coverage.
- Submit fixes for confirmed defects.

For setup and usage questions, please use [GitHub Discussions](https://github.com/markheydon/freeagent-dotnet/discussions) first.

## Development Setup

See **[docs/contributing-setup.md](docs/contributing-setup.md)** for SDK installation, verifying `dotnet --list-sdks`, and building `FreeAgent.slnx`.

Requirements:

- .NET 8.0 SDK and runtime (required to build and test the SDK `net8.0` target).
- .NET 10.0 SDK and runtime (primary SDK focus; also required for the Blazor sample).

The SDK package targets both frameworks. The sample app is .NET 10.0-only.

Build and test locally:

```bash
dotnet build FreeAgent.slnx
dotnet test
```

To run tests for a single framework, use `dotnet test -f net10.0` or `dotnet test -f net8.0`.

## Branch and Pull Request Workflow

1. Fork the repository and create a feature branch from `main`.
2. Keep changes focused and small where practical.
3. Add or update tests for behavioural changes.
4. Update docs affected by your change.
5. Open a pull request using the PR template.

## Pull Request Expectations

- Follow existing coding conventions and project structure.
- Preserve backwards compatibility unless a breaking change is explicitly discussed.
- Include clear rationale and scope in the PR description.
- Ensure CI is green before requesting final review.

## Endpoint and Sample App Sync

The Blazor sample is a living reference of what the SDK implements today.

If you add, remove, or rename SDK endpoints under `src/FreeAgent.Client/Services/`, update `samples/FreeAgent.Client.Sample` in the same PR so the sample remains aligned. Follow [`docs/contributing/sample-probe-pages.md`](docs/contributing/sample-probe-pages.md) for wire-to-model probe pages (see Company and Contacts as references).

## Testing Guidance

- Prefer unit tests close to changed behaviour.
- Keep tests deterministic and focused.
- For endpoint work, add coverage for success and relevant failure pathways.

## Documentation Guidance

- Use UK English spelling in all documentation and comments.
- Keep README and plan documents aligned with implemented behaviour, not aspirational behaviour.

## Security

Do not disclose security vulnerabilities in public issues or discussions.

Use the process in [SECURITY.md](SECURITY.md).

## Code of Conduct

By participating in this project, you agree to follow [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).

## Questions and Support

See [SUPPORT.md](SUPPORT.md) for support routes and expectations.
