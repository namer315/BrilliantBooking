# Copilot Instructions

## Architecture

All web-to-backend calls go through the `FDM` project.

| Project | Role | May reference |
|---|---|---|
| `BrilliantBooking` (Blazor web) | UI components and pages | `FDM`, `Data` |
| `FDM` | Thin gateway. No business logic. Each public method delegates to `Business`. | `Business`, `Data` |
| `Business` | All business logic | `Data` |
| `Data` | Queries, DAOs, models, VOs, and other simple shared types | none |

The web project must not reference `Business`.

## Naming

- Gateway classes: `<Domain>FDM` (for example, `RestaurantFDM`), placed in the `FDM` project.
- Business classes: `<Domain>Business` (for example, `RestaurantBusiness`), placed in the `Business` project.
- In Blazor components, hold the gateway in a private field named `_fdm`:

```csharp
private RestaurantFDM _fdm = new RestaurantFDM();
```

## Do

- Put business rules in `Business`.
- Put queries, DAOs, models, and VOs in `Data`.
- Call backend functionality from components only through `FDM` classes.

## Don't

- Don't put business logic in `FDM`.
- Don't reference `Business` from the web project.
- Don't write data queries directly in Blazor components.
