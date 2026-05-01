# Security Policy

## Reporting a Vulnerability

Please do not report security vulnerabilities via public GitHub issues or discussions.

Use GitHub's private vulnerability reporting flow for this repository:

1. Go to the repository Security tab.
2. Select Report a vulnerability.
3. Submit a private report with reproduction details, impact, and any suggested remediation.

If you cannot use GitHub private reporting, open a discussion first only for non-sensitive questions and avoid sharing exploit details.

## What to Include

Please include as much of the following as possible:

- A clear description of the vulnerability.
- Affected package and version.
- Reproduction steps or proof of concept.
- Potential impact and attack surface.
- Any suggested fix or mitigation.

## Response Expectations

Best-effort targets:

- Initial acknowledgement: within 5 working days.
- Triage outcome: within 10 working days when reproducible details are provided.
- Ongoing updates: provided at key milestones until resolution.

Timelines may vary depending on severity and maintainer availability.

## Disclosure Process

- Reports are reviewed privately.
- Remediation is prepared and validated.
- A fix is released.
- Public disclosure follows coordinated timing once users can reasonably patch.

## Supported Versions

This project is currently in prerelease stages. Security fixes are applied on a best-effort basis to the latest prerelease line.

For release-stage policy and compatibility expectations, see [VERSIONING.md](VERSIONING.md).

## Scope

This policy covers:

- The FreeAgent .NET SDK code in this repository.
- Published NuGet packages produced from this repository.

Third-party services, upstream API behaviour, and local deployment issues outside this repository are out of scope.
