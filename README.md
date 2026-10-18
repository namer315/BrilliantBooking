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

The output in `bin/Release/net10.0/<runtime-identifier>/publish` includes the application, bundled .NET runtime, and static assets. No separate .NET installation is required on the host.

This is a self-contained deployment, not a static-only website: the C# API still needs to run as a process on the host. If hosting behind IIS, the ASP.NET Core Hosting Bundle may still be needed for IIS integration; alternatively, run the app directly or behind a reverse proxy.

## Self-host on Linux with systemd

The included installer targets a Linux x64 server running systemd. Publish the app, then create an archive of the published output on your development machine:

```powershell
dotnet publish ERPBooking.csproj -c Release -p:PublishProfile=linux-x64
tar -czf ERPBooking-linux-x64.tar.gz -C bin/Release/net10.0/linux-x64/publish .
```

Copy the archive and installer files to your Linux server:

```powershell
scp ERPBooking-linux-x64.tar.gz deploy/linux/install.sh deploy/linux/erpbooking.service user@your-server:/tmp/
```

On the server, extract and install the application:

```sh
mkdir -p /tmp/erpbooking-publish
tar -xzf /tmp/ERPBooking-linux-x64.tar.gz -C /tmp/erpbooking-publish
sudo bash /tmp/install.sh /tmp/erpbooking-publish
```

The installer creates a restricted `erpbooking` system account, installs the app under `/opt/erpbooking`, and enables the `erpbooking` systemd service. The service listens only on `127.0.0.1:5080`; put a TLS-enabled reverse proxy in front of it for external access. Check the API locally with `curl http://127.0.0.1:5080/api/health`, inspect logs with `sudo journalctl -u erpbooking -f`, and check service state with `sudo systemctl status erpbooking`.

To deploy an update, repeat the publish, copy, and install steps. The installer stops the service before replacing files and restarts it afterward.

## API

- `GET /api/health` returns the API health status.
- `/` renders the Razor homepage from `Pages/Index.cshtml`.
