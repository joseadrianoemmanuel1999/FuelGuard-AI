# Deploy FuelGuard API on Render

Backend: **ASP.NET Core 8** (`src/FuelGuard.Api`) — Docker only.  
Frontend: **Angular** (`fuelguard-webclient`) — deploy on a **separate** host (not this Render Web Service).

---

## Root cause: exit code 127

| Code | Meaning |
|------|---------|
| **127** | `command not found` (POSIX shell) |

Render shows *"Exited with status 127 while building your code"* when the **native build shell** runs a command that does not exist, for example:

- `yarn start` / `yarn` with Node runtime and wrong root directory
- `dotnet publish` on Render's native environment (no .NET SDK installed)
- Leftover **Build Command** in the dashboard while **Environment = Docker**

The repository `Dockerfile` and `render.yaml` are correct; the failure is almost always **dashboard settings** not matching Docker, or a **Blueprint** that was never synced.

---

## Ideal Render dashboard settings

| Setting | Value |
|---------|--------|
| **Environment** | Docker |
| **Root Directory** | *(empty — repository root)* |
| **Dockerfile Path** | `./Dockerfile` |
| **Docker Context** | `.` |
| **Build Command** | *(empty)* |
| **Start Command** | *(empty)* |
| **Pre-Deploy Command** | *(empty)* |
| **Health Check Path** | `/health` |

Do **not** point this service at `fuelguard-webclient/`.

---

## Blueprint (`render.yaml`)

1. Render Dashboard → **Blueprints** → connect repo → apply `render.yaml`, **or**
2. Create a new Web Service from the blueprint and delete the old Node-based service.

After sync, confirm **Build Command** and **Start Command** are empty in the service settings.

---

## Environment variables (Render)

| Key | Example / notes |
|-----|-----------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` (set in `render.yaml`) |
| `ASPNETCORE_URLS` | `http://0.0.0.0:$PORT` (set in `render.yaml`) |
| `ConnectionStrings__FuelGuard` | Supabase PostgreSQL connection string **with password** |
| `Cors__AllowedOrigins` | `https://your-frontend.example.com` |
| `AI__ApiKey` | Hugging Face token (optional) |

Supabase example:

```text
Host=db.xxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

---

## Local validation (before push)

From repository root:

```powershell
cd "c:\Users\josea\Documents\GitHub\FuelGuard AI"

docker build -t fuelguard-api:local .

docker run --rm -p 10000:10000 `
  -e PORT=10000 `
  -e ASPNETCORE_URLS=http://0.0.0.0:10000 `
  -e ConnectionStrings__FuelGuard="Host=...;Password=..." `
  fuelguard-api:local
```

```powershell
curl http://localhost:10000/health
curl http://localhost:10000/health/ready
```

.NET only (no Docker):

```powershell
dotnet publish src/FuelGuard.Api/FuelGuard.Api.csproj -c Release -o ./artifacts/publish
dir ./artifacts/publish/FuelGuard.Api.dll
```

---

## Monorepo layout

```text
/
├── Dockerfile              ← API image (Render)
├── render.yaml             ← Blueprint
├── FuelGuard.sln
├── src/FuelGuard.Api/      ← entry assembly → FuelGuard.Api.dll
└── fuelguard-webclient/    ← Angular (separate deploy)
```

---

## Post-deploy checklist

- [ ] Build log shows `docker build` (no `yarn` / `npm` / native `dotnet`)
- [ ] `GET https://<service>.onrender.com/health` → `200` JSON
- [ ] `ConnectionStrings__FuelGuard` set in Render
- [ ] CORS includes production frontend URL
- [ ] Swagger only in Development (not exposed in Production)
- [ ] GitHub `RENDER_DEPLOY_HOOK` secret configured for `.github/workflows/deploy-render.yml`

---

## CI/CD

- **CI** (`.github/workflows/ci.yml`): `dotnet build` + `docker build` on every PR/push.
- **Deploy** (`.github/workflows/deploy-render.yml`): triggers Render deploy hook after CI on `main`.
