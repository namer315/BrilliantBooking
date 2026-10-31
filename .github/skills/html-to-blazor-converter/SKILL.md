---
name: html-to-blazor-converter
description: Converts HTML, CSS, and JavaScript web pages into modular Blazor components (.NET 10, C# 14) with strict code-behind separation. Use when a user asks to convert, port, or migrate a static or JavaScript-driven page into a Blazor component.
---

# HTML/CSS/JS to Blazor Component Converter

## Skill Overview & Purpose

This skill converts a web page (HTML markup, CSS styles, and JavaScript behavior) into a Blazor component for .NET 10 and C# 14. Each component produces four files with strict separation of concerns:

| File | Purpose |
|---|---|
| `ComponentName.razor` | Markup and Blazor directives only |
| `ComponentName.razor.cs` | Partial class with all converted logic |
| `ComponentName.razor.css` | Scoped CSS for styles that Tailwind utilities cannot express |
| `ComponentName.Models.cs` | Converted models, DTOs, and enums |

Target project: `C:\BrilliantBooking\`. Page components live in `BrilliantBooking\Components\Pages\`.

## System Role & Behavioral Constraints

**Role:** Senior .NET architecture specialist who converts front-end code into idiomatic C# Blazor components.

**Hard constraints (never violate):**

1. The `.razor` file contains no `@code` block, no C# member declarations, and no `@inject` directive. Allowed directives are `@attribute`, `@inherits`, `@using`, and `@implements`.
2. All state, parameters, injections, lifecycle methods, and handlers live in `ComponentName.razor.cs`.
3. JavaScript is transpiled to C#. `IJSRuntime` is used only as a documented fallback (see the mapping table).
4. Every output file includes its complete `using` directives and its `namespace`. Do not rely on implicit imports.
5. Do not invent APIs, component names, or NuGet packages. If something cannot be determined, emit `// TODO(convert): <original snippet>` and list it in the manifest's `openQuestions`.
6. Preserve the original visual output. Keep Tailwind classes in markup. Move hand-written CSS to `.razor.css`.
7. Never emit a `<script>` tag or raw inline JavaScript in the output.

## JavaScript-to-C# Transpilation Mapping

| JavaScript construct | C# equivalent | Notes |
|---|---|---|
| `let x = 0` / `const x = 'a'` | `private int _x = 0;` / `private const string X = "a";` | Use `_camelCase` private fields. Use `const` only for true constants. |
| `function f(a) {...}` | `private void F(int a) {...}` | PascalCase methods. |
| `async function f()` / `await` | `private async Task FAsync()` / `await` | Suffix with `Async`. Return `Task`, not `void`, for async work. |
| `Promise.all([...])` | `await Task.WhenAll(...)` | Use `Task<T>` results when values are needed. |
| `setTimeout(fn, ms)` | `await Task.Delay(ms, ct)` | Use a `CancellationTokenSource` and cancel it in `DisposeAsync`. |
| `setInterval(fn, ms)` | `PeriodicTimer` on a background `Task` | Dispose the timer in `DisposeAsync`. |
| `arr.map(x => ...)` | `arr.Select(x => ...)` | Call `.ToList()` when the result is stored. |
| `arr.filter(x => ...)` | `arr.Where(x => ...)` | Same as above. |
| `arr.find(x => ...)` | `arr.FirstOrDefault(x => ...)` | Check the result for `null`. |
| `arr.reduce((a, x) => ..., init)` | `arr.Aggregate(init, (a, x) => ...)` | Use `Sum`, `Count`, or `Max` when they apply. |
| `[...new Set(arr)]` | `arr.Distinct().ToList()` | |
| `arr.sort((a, b) => a - b)` | `arr.Order()` or `arr.OrderBy(x => x)` | |
| `for (const x of arr)` | `foreach (var x in arr)` | |
| `for (let i = 0; ...)` | `for (int i = 0; ...)` | |
| Object literal `{ id: 1, name: 'A' }` | `public sealed class Name` or `record` in `.Models.cs` | Type every object shape. |
| `JSON.stringify` / `JSON.parse` | `System.Text.Json.JsonSerializer` | Use `[JsonPropertyName]` when keys differ. |
| `null` / `undefined` | `null` with nullable reference types | Use `?` on nullable references. |
| `` `Hello ${name}` `` | `$"Hello {name}"` | |
| `cond ? a : b`, `a ?? b`, `a?.b` | Same syntax in C# | |
| `this.x` | `_x` (instance field) | |
| `el.addEventListener('click', fn)` | `@onclick="FnAsync"` in markup | Move the handler to `.razor.cs`. |
| `e.stopPropagation()` | `@onclick:stopPropagation="true"` | Markup directive. |
| `e.preventDefault()` | `@onclick:preventDefault="true"` | Markup directive. |
| `el.textContent = x`, `classList.toggle` | `@(...)` expressions bound to state | Recompute from state. Do not mutate the DOM. |
| `document.querySelector` / DOM mutation | Declarative binding, or `ElementReference` via `@ref` | `IJSRuntime` only if no declarative option exists. |
| `fetch(url, opts)` | `HttpClient` with `GetFromJsonAsync` or `PostAsJsonAsync` | Inject `HttpClient` in `.razor.cs`. |
| `new Date()` / `Date.now()` | `DateTimeOffset.Now` / `DateTimeOffset.UtcNow` | |
| `Math.*` | `Math.*` | Same API. |
| `console.log` | `ILogger<T>` injected in `.razor.cs` | |
| `localStorage` | `ProtectedLocalStorage` (Blazor Server) or `IJSRuntime` fallback | Use JS fallback only for raw storage. |
| `window.addEventListener('resize')` | `IJSRuntime` fallback | No C# equivalent. |
| Canvas, Web Audio, WebGL | `IJSRuntime` fallback | Documented exception. |
| Third-party JS plugin (e.g., `lucide.createIcons()`) | Check whether the project already refreshes it. If not, `IJSRuntime` in `OnAfterRenderAsync(bool firstRender)` | Guard with `firstRender` when calling once. |

**JS interop policy:** Use `IJSRuntime` only where this table lists it as a fallback. Each use needs a one-line justification comment in `.razor.cs`. The manifest sets `jsInteropUsed` to `true` and gives the reason.

## Code-Behind (.razor.cs) Implementation Rules

1. Declare `public partial class ComponentName : ComponentBase` with the same name and namespace as the `.razor` file. Add a base class only when the `.razor` file also declares it.
2. Include `using Microsoft.AspNetCore.Components;` and every other required `using` directive.
3. Every `[Parameter]` property uses `{ get; set; }`. Mark required parameters with `[EditorRequired]`. Use `EventCallback<T>` for outputs, not `Action`.
4. Every injected dependency uses `[Inject] private T Name { get; set; } = default!;`.
5. Lifecycle overrides use these exact signatures:
   - `protected override void OnInitialized()`
   - `protected override async Task OnInitializedAsync()`
   - `protected override async Task OnParametersSetAsync()`
   - `protected override async Task OnAfterRenderAsync(bool firstRender)`
   Call the base method when overriding.
6. Event handlers return `Task` when they await and `void` otherwise. The signature must match the markup binding.
7. Implement `IAsyncDisposable` or `IDisposable` when the component owns timers, subscriptions, or a `CancellationTokenSource`.
8. Copy parameters into internal state in `OnInitialized`, not `OnParametersSet`. `OnParametersSet` resets local state on every parent render.
9. Do not call `StateHasChanged()` after UI event handlers. Blazor re-renders automatically. Call it only after non-UI callbacks, such as timers.
10. Derived values are expression-bodied properties, for example `private IEnumerable<X> Visible => ...`.

## Local Model Processing Protocol

This section applies to Ollama, Llama 3, Mistral, LM Studio, and similar models with limited context.

### Recommended Inference Settings

| Setting | Value | Reason |
|---|---|---|
| Temperature | 0.1 to 0.2 | Reduces invented APIs |
| Max output tokens | At least 4096 | Prevents truncated `.razor.cs` files |
| Context window | At least 8k | Holds the system prompt, input, and output |
| Stop sequence | `<END_OF_FILE>` | Marks clean file boundaries |

### System Message Template

Send this as the system message on every request:

```plaintext
ROLE: You convert web code into Blazor components for .NET 10 / C# 14.

BOUNDARIES:
- Output ONLY the four files and the manifest described below.
- NEVER output an @code block or @inject directive in any .razor file.
- NEVER output member declarations inside .razor.
- ALWAYS include every using directive and the namespace in each file.
- NEVER invent APIs or packages. Use TODO(convert) comments for unknowns.
- Use IJSRuntime only for browser APIs that have no C# equivalent. Justify each use.

OUTPUT CONTRACT (in this order, nothing between files except the file header):
### FILE 1: <Name>.razor
### FILE 2: <Name>.razor.cs
### FILE 3: <Name>.razor.css
### FILE 4: <Name>.Models.cs
### MANIFEST
<END_OF_FILE>
```

### Manifest Schema

```json
{
  "component": "string, PascalCase",
  "namespace": "string, for example BrilliantBooking.Components.Pages",
  "files": [
	{ "name": "string", "kind": "markup | code-behind | css | models" }
  ],
  "parameters": [
	{ "name": "string", "type": "string", "required": "boolean" }
  ],
  "jsInteropUsed": "boolean",
  "jsInteropJustification": "string or null",
  "openQuestions": ["string"]
}
```

### Anti-Hallucination and Anti-Truncation Techniques

1. **Chunking:** For JavaScript over about 300 lines, convert one function group per pass. Pass the generated member signatures, not the full code, as a compact interface summary.
2. **Self-check before the manifest:** Confirm the following:
   - No `@code` or `@inject` in `.razor`.
   - Every `[Parameter]` and `[Inject]` type is in scope through a `using` or a full name.
   - Every `async` method name ends in `Async` and returns `Task`.
   - The partial class name matches the `.razor` file name.
   - The `.razor.cs` namespace matches the `.razor` file's namespace.
3. **Skipped-import prevention:** Include `using System;`, `using System.Linq;`, `using System.Collections.Generic;`, `using System.Threading.Tasks;`, and `using Microsoft.AspNetCore.Components;` in `.razor.cs` unless the file is trivially small.
4. **Fallback prevention:** If a construct is unclear, emit `// TODO(convert): <original JS snippet>` in `.razor.cs`. Never fall back to an inline `@code` block or a raw `<script>` tag.
5. **Single source of truth:** Declare models only in `.Models.cs`. Never redeclare them in `.razor.cs`.

### Validation Pass

- `.razor` has no `@code`, no `@inject`, no `<script>`, and no C# blocks beyond `@foreach` and `@if`.
- `.razor.cs` declares `public partial class` matching the file name.
- `.razor.css` contains only styles that Tailwind utilities cannot express.
- `.Models.cs` contains only type declarations.
- The manifest is valid JSON.

## Worked Example

### Input (HTML/CSS/JS)

```html
<div class="favorite-filter">
  <div class="filter-bar" id="filters"></div>
  <p class="count">Favorites: <span id="favCount">0</span></p>
  <ul class="list" id="list"></ul>
</div>
<style>
  .chip.active { background: #0a84ff; color: #fff; }
  .fav.on { color: #ff453a; }
</style>
<script>
  const restaurants = [
	{ id: 1, name: "Bistro Lune", cuisine: "French", favorite: false },
	{ id: 2, name: "Sakura House", cuisine: "Japanese", favorite: true },
	{ id: 3, name: "Le Petit", cuisine: "French", favorite: false }
  ];
  let filter = "All";

  function visible() {
	return filter === "All" ? restaurants : restaurants.filter(r => r.cuisine === filter);
  }
  function setFilter(c) { filter = c; render(); }
  async function toggleFav(id) {
	const r = restaurants.find(x => x.id === id);
	if (!r) return;
	r.favorite = !r.favorite;
	const favs = restaurants.filter(r => r.favorite).map(r => r.id);
	await fetch("/api/favorites", { method: "POST", body: JSON.stringify(favs) });
	render();
  }
  function render() { /* rebuilds DOM from state */ }
</script>
```

### FILE 1: FavoriteFilter.razor

```razor
<div class="favorite-filter">
	<div class="filter-bar">
		<button class="chip @(_filter == "All" ? "active" : "")" @onclick="() => SetFilter("All")">All</button>
		@foreach (var c in Cuisines)
		{
			<button class="chip @(_filter == c ? "active" : "")" @onclick="() => SetFilter(c)">@c</button>
		}
	</div>

	<p class="count">Favorites: @FavoriteCount</p>

	<ul class="list">
		@foreach (var r in VisibleItems)
		{
			<li class="item">
				<span>@r.Name</span>
				<button class="fav @(r.IsFavorite ? "on" : "")"
						@onclick:stopPropagation="true"
						@onclick="() => ToggleFavoriteAsync(r.Id)">♥</button>
			</li>
		}
	</ul>
</div>
```

### FILE 2: FavoriteFilter.razor.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Pages;

public partial class FavoriteFilter : ComponentBase
{
	[Parameter, EditorRequired] public IReadOnlyList<RestaurantItem> Items { get; set; } = [];
	[Parameter] public EventCallback<IReadOnlyList<int>> OnFavoritesChanged { get; set; }

	[Inject] private HttpClient Http { get; set; } = default!;

	private List<RestaurantItem> _items = [];
	private string _filter = "All";

	// Copy parameters once so local toggles survive parent re-renders.
	protected override void OnInitialized()
	{
		_items = Items.Select(i => new RestaurantItem
		{
			Id = i.Id,
			Name = i.Name,
			Cuisine = i.Cuisine,
			IsFavorite = i.IsFavorite
		}).ToList();
	}

	private IEnumerable<string> Cuisines => _items.Select(r => r.Cuisine).Distinct().Order();

	private IEnumerable<RestaurantItem> VisibleItems =>
		_filter == "All" ? _items : _items.Where(r => r.Cuisine == _filter);

	private int FavoriteCount => _items.Count(r => r.IsFavorite);

	private void SetFilter(string cuisine) => _filter = cuisine;

	private async Task ToggleFavoriteAsync(int id)
	{
		var restaurant = _items.FirstOrDefault(x => x.Id == id);
		if (restaurant is null)
		{
			return;
		}

		restaurant.IsFavorite = !restaurant.IsFavorite;

		var favorites = _items.Where(r => r.IsFavorite).Select(r => r.Id).ToArray();
		using var response = await Http.PostAsJsonAsync("/api/favorites", favorites);
		response.EnsureSuccessStatusCode();

		await OnFavoritesChanged.InvokeAsync(favorites);
	}
}
```

Conversion notes: `render()` is removed because Blazor re-renders after event handlers. `fetch` maps to `HttpClient`. The `filter` and `restaurants` globals become private instance state.

### FILE 3: FavoriteFilter.razor.css

```css
/* Only styles that Tailwind utilities cannot express. */
.favorite-filter {
	display: flex;
	flex-direction: column;
	gap: 0.75rem;
}

.chip.active {
	background: #0a84ff;
	color: #fff;
}

.fav.on {
	color: #ff453a;
}
```

### FILE 4: FavoriteFilter.Models.cs

```csharp
namespace BrilliantBooking.Components.Pages;

public sealed class RestaurantItem
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Cuisine { get; set; } = string.Empty;
	public bool IsFavorite { get; set; }
}
```

### MANIFEST

```json
{
  "component": "FavoriteFilter",
  "namespace": "BrilliantBooking.Components.Pages",
  "files": [
	{ "name": "FavoriteFilter.razor", "kind": "markup" },
	{ "name": "FavoriteFilter.razor.cs", "kind": "code-behind" },
	{ "name": "FavoriteFilter.razor.css", "kind": "css" },
	{ "name": "FavoriteFilter.Models.cs", "kind": "models" }
  ],
  "parameters": [
	{ "name": "Items", "type": "IReadOnlyList<RestaurantItem>", "required": true },
	{ "name": "OnFavoritesChanged", "type": "EventCallback<IReadOnlyList<int>>", "required": false }
  ],
  "jsInteropUsed": false,
  "jsInteropJustification": null,
  "openQuestions": [
	"Confirm that the /api/favorites endpoint exists and that an HttpClient with a base address is registered."
  ]
}
```

## Project Notes: BrilliantBooking

- Page components live in `BrilliantBooking\Components\Pages\`.
- `HomeBase` (namespace `BrilliantBooking.Components.Pages`, declared in `Home.razor.cs`) is the base for the home page components.
- Lucide icons are loaded once in `BrilliantBooking\Components\App.razor`. The `window.refreshReserveIcons` function calls `lucide.createIcons()`. `HomeBase.OnAfterRenderAsync` calls it on every render, so components that use `data-lucide` need no JS interop of their own.
- Styling uses Tailwind utility classes in markup. Use `.razor.css` only when no utility class covers the style.
- Confirm the namespace before converting a new page. The worked example uses a generic namespace.
