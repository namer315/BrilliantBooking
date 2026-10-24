# ERPBooking

An ASP.NET Core Razor Pages restaurant discovery and booking site, with a C# health endpoint. Restaurant listings and availability are rendered by Razor Pages; styles are served from `wwwroot`. Self-contained publish profiles bundle the .NET runtime for Windows x64 and Linux x64, so the target host does not need the .NET runtime installed separately.

## Develop in Visual Studio

Open `ERPBooking.sln` and press **F5** to run the app. Restaurant information is seeded in `Booking/RestaurantCatalog.cs`. Local reservations are stored in the ignored `App_Data/reservations.json` file; override the path with the `Booking__DataPath` environment variable.

## Self-host with Docker Compose

Install Docker Engine (or Docker Desktop with Linux containers) and the Docker Compose v2 plugin on your server. Copy or check out this repository on the server, open a terminal in its root directory, and run:

```sh
docker compose up -d --build
```

Open `http://localhost:8080` on the server. Verify the API with:

```sh
curl --fail http://localhost:8080/api/health
```

The response is `{"status":"ok","application":"ERPBooking"}`. The multi-stage image builds with the .NET 10 SDK and runs with only the ASP.NET Core runtime as a non-root user; no .NET installation is needed on the host. Compose persists reservations in the `erpbooking-data` named volume. It restarts the service after a crash or server reboot unless you explicitly stop it, provided Docker starts on boot. Logs are rotated to limit disk usage.

### Network access and HTTPS

By default, the service binds only to the server's loopback address. For access from another machine, set `ERPBOOKING_BIND_ADDRESS` to the server's LAN address (or `0.0.0.0` for all IPv4 interfaces) and optionally set `ERPBOOKING_PORT` to a free host port before running Compose. For example, in a POSIX shell:

```sh
ERPBOOKING_BIND_ADDRESS=0.0.0.0 ERPBOOKING_PORT=8080 docker compose up -d --build
```

Then visit `http://<server-ip>:8080`. Restrict access with your firewall; the application currently has no authentication. For internet-facing hosting, keep the default loopback binding and place an HTTPS reverse proxy, such as Caddy, nginx, or IIS, on the same host in front of `http://127.0.0.1:8080`. Configure your domain, TLS certificates, and access controls at the proxy. A proxy in a separate container must instead share a Docker network with the app and connect to `http://erpbooking:8080`; its own loopback address will not reach the app.

### Manage and update

```sh
docker compose ps
docker compose logs -f erpbooking
docker compose down
```

After updating the source checkout, rebuild and recreate the service with `docker compose build --pull` followed by `docker compose up -d`. Use the same bind-address and port settings on every invocation. This also refreshes the base runtime image for security updates. The named volume preserves reservation data when the application container is replaced; `docker compose down -v` removes that data.

## Publish for hosting without Docker

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

The installer creates a restricted `erpbooking` system account, installs the app under `/opt/erpbooking`, and enables the `erpbooking` systemd service. The service stores reservations under `/var/lib/erpbooking` and listens only on `127.0.0.1:5080`; put a TLS-enabled reverse proxy in front of it for external access. Check the API locally with `curl http://127.0.0.1:5080/api/health`, inspect logs with `sudo journalctl -u erpbooking -f`, and check service state with `sudo systemctl status erpbooking`.

To deploy an update, repeat the publish, copy, and install steps. The installer stops the service before replacing files and restarts it afterward.

## API

- `GET /api/health` returns the API health status.
- `/` renders the restaurant directory with search and filters.
- `/restaurants/{slug}` shows a restaurant profile and handles availability lookup and reservation creation.

## Restaurant booking feature roadmap

The first release now includes the directory, profiles, filters, availability lookup, and reservation creation. Listings and capacities are sample data, with reservations saved as JSON for a single app instance. This prototype does not yet integrate restaurant-managed inventory or send email/SMS confirmations; use a database and secure operational setup before handling real customer bookings.

### Core experience

1. **Restaurant discovery** — implemented with sample restaurants and availability.
2. **Search and filters** — implemented for restaurant, cuisine, area, date, time, party size, and price.
3. **Restaurant profiles** — implemented with sample photos, menu summaries, opening hours, address, contact details, highlights, and sample ratings.
4. **Availability and reservations** — implemented with per-slot sample capacity and a server-validated reservation form.
5. **Booking management** — let diners view upcoming and past reservations, and cancel or change a booking subject to the restaurant's policy.
6. **Diner accounts and favourites** — support sign-in, saved diner details, and a list of favourite restaurants.
7. **Booking communications** — send confirmation and reminder messages, and notify diners when a reservation changes or is cancelled.

### Follow-on features

8. **Restaurant management** — give restaurant staff a protected area to maintain their profile, opening hours, specials, availability, and reservations.
9. **Promotions and vouchers** — let diners discover restaurant specials and purchase or redeem dining vouchers.
10. **Ratings and reviews** — collect and display guest feedback tied to completed visits.
11. **Location-aware browsing** — show nearby restaurants and availability, with area-based browsing when location access is unavailable.
12. **Responsive and accessible UI** — make discovery and booking usable on mobile, keyboard, and assistive technologies.

**Next milestone:** add diner booking management and restaurant-side reservation tools backed by a production-grade reservation database and inventory management.

**Reference:** [Dineplan restaurants in Cape Town](https://www.dineplan.com/restaurants/in/cape-town) and [Dineplan app features](https://www.dineplan.com/app).
