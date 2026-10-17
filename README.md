# ERPBooking

An ASP.NET Core Razor Pages frontend with a C# Minimal API. The Razor homepage is rendered from `Pages/Index.cshtml` and its code-behind model, while CSS and JavaScript are served from `wwwroot`. Self-contained publish profiles bundle the .NET runtime for Windows x64 and Linux x64, so the target host does not need the .NET runtime installed separately.

## Develop in Visual Studio

Open `ERPBooking.sln`. Edit the homepage markup in `Pages/Index.cshtml`, its C# code-behind in `Pages/Index.cshtml.cs`, and the shared styles and browser JavaScript under `wwwroot`. Press **F5** to run the app.

## Publish for hosting

Use the profile that matches the hosting server's operating system and CPU architecture:

```powershell
dotnet publish ERPBooking.csproj -c Release -p:PublishProfile=win-x64
```

For a Linux x64 host:

```powershell
dotnet publish ERPBooking.csproj -c Release -p:PublishProfile=linux-x64
```

Copy the complete contents of the matching `bin/Release/net10.0/<runtime-identifier>/publish` directory to the host. The output includes the application, bundled .NET runtime, and static assets. Run `ERPBooking.exe` on Windows or `./ERPBooking` on Linux. No separate .NET installation is required on the host.

This is a self-contained deployment, not a static-only website: the C# API still needs to run as a process on the host. If hosting behind IIS, the ASP.NET Core Hosting Bundle may still be needed for IIS integration; alternatively, run the app directly or behind a reverse proxy.

## API

- `GET /api/health` returns the API health status.
- `/` renders the Razor homepage from `Pages/Index.cshtml`.
