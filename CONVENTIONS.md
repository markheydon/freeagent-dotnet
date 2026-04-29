# Conventions
> ⚠️ Review and update this file once tech stack choices are finalised. The structure and patterns below are .NET Clean Architecture defaults.

# Conventions

**Project:** FreeAgent.NET
**Last updated:** 29 April 2026

This document records decisions about how code is written in this project.
It exists so that both humans and AI produce consistent output.
When in doubt, follow what's here. To change a convention, update this file and
create an ADR if it's a significant architectural change.

---

## Project Structure

````
src/
	FreeAgent.Client/           # Main SDK implementation (models, services, HTTP, auth)
tests/
	FreeAgent.Client.Tests/     # Unit and integration tests for the SDK
plan/                         # Project planning and specs
````

---

## Patterns in Use
- **Service abstraction** for each FreeAgent API resource.
- **Strongly typed models** for requests and responses.
- **Exception hierarchy** for API and transport errors.
- **Async/await** for all I/O operations.
- **No business logic** in SDK (see SCOPE.md).

---

## Naming Quick Reference

| Thing | Convention | Example |
|---|---|---|
| Interface | `I` prefix | `IOrderRepository` |
| Command | `[Verb][Noun]Command` | `CreateOrderCommand` |
| Query | `Get[Noun][By...]Query` | `GetOrderByIdQuery` |
| Handler | `[CommandOrQuery]Handler` | `CreateOrderCommandHandler` |
| DTO (read) | `[Noun]Dto` | `OrderDto` |
| Request (write) | `[Verb][Noun]Request` | `CreateOrderRequest` |
| Response | `[Noun]Response` | `OrderResponse` |
| Test class | `[ClassName]Tests` | `CreateOrderCommandHandlerTests` |
| Test method | `Method_State_Expected` | `Handle_WithValidCommand_ReturnsSuccess` |

---

## Things We Don't Do Here
- No business logic in controllers (ever)
- No raw SQL outside of explicitly marked read-model queries
- No `.Result` or `.Wait()` on async code
- No commented-out code committed to main
- No `TODO` without a linked GitHub Issue number

---

## Revision History
| Date | Change | Reason |
|---|---|---|
| 29 April 2026 | Initial draft | Project kickoff |

