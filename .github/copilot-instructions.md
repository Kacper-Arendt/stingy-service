# Copilot Instructions for Stingy Service

## Project Architecture
- **Modular Monorepo**: Organized by feature modules under `src/Modules/` (e.g., `Auth`, `Teams`, `UserProfiles`). Shared code is under `src/Shared/`.
- **App Entry Point**: Main application starts in `src/Bootstrapper/Stingy.App/Program.cs`.
- **Dependency Injection**: DI setup and module loading in `src/Bootstrapper/Stingy.App/DI/ModuleLoader.cs`.
- **Service Boundaries**: Each module (e.g., `Auth`, `UserProfiles`) has its own API, Core, Domain, and Shared subprojects.
- **Shared Abstractions**: Common interfaces, events, exceptions, and value objects are in `src/Shared/Shared.Abstractions/`.

## Project-Specific Patterns
## API Approach
- **Minimal API Preferred (.NET 10)**: The project prefers using Minimal API patterns for defining endpoints and controllers, favoring concise and direct route definitions over traditional MVC controllers. The codebase targets .NET 10 features and conventions.
- **Exception Handling**: Custom exceptions in `Shared.Abstractions/Exceptions/`.
- **External Communication**: Email sending via `IEmailSender` in `Shared.Abstractions/Communication/`.
- **Database**: Migrations and entities are organized per module (e.g., `Auth.Core/Migrations/`, `UserProfiles.Domain/Entities/`).
- **HTTP**: Example requests in `Stingy.App.http`.

## Conventions
- **Namespace Structure**: Follows folder hierarchy (e.g., `Stingy.Modules.Auth.Api`).
- **File Organization**: Keep business logic in `Core`, API endpoints in `Api`, and shared contracts in `Shared`.
- **No README.md detected**: Document new patterns in this file for future agents.

## Examples
- To add a new feature module, create subfolders for `Api`, `Core`, `Domain`, and `Shared` under `src/Modules/<Feature>/`.
- To add a new event, implement `IEvent` and `IEventHandler` in `Shared.Abstractions/Events/`.

---
_If any section is unclear or missing, please provide feedback to improve these instructions._
