# BrilliantBooking

## Icon refresh lifecycle

Pages inherit `PageBase`, which refreshes Lucide icons after each render so
dynamically added icons are initialized. `SiteHeader` uses the same interop helper.
Icon refresh must not call `StateHasChanged`, as that creates a render/interop loop.

Disposing a component cancels its pending icon refreshes. Disposal cancellation
and disconnected browser circuits are logged at Debug level; unrelated timeouts
and JavaScript errors propagate to Blazor's error handling.
