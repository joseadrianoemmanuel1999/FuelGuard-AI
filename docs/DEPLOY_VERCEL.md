# Deploy FuelGuard frontend on Vercel

Angular 20 app in `fuelguard-webclient/`.  
Backend API: **Render** — `https://fuelguard-ai-3bm6.onrender.com`

---

## Vercel project settings

Import the GitHub monorepo and use:

| Setting | Value |
|---------|--------|
| **Root Directory** | `fuelguard-webclient` |
| **Framework Preset** | Other (or Angular — `vercel.json` overrides output) |
| **Node.js Version** | `20` (from `.nvmrc`) |
| **Install Command** | `npm ci` |
| **Build Command** | `npm run build` |
| **Output Directory** | `dist/fuelguard-web/browser` |

These match `fuelguard-webclient/vercel.json` when Root Directory is `fuelguard-webclient`.

---

## Files in the repo

| File | Purpose |
|------|---------|
| `vercel.json` | Output path, build commands, SPA rewrites → fixes **404 on refresh** |
| `src/environments/environment.prod.ts` | Production API URL (Render) |
| `.nvmrc` | Node 20 on Vercel |

---

## SPA routing (404 NOT_FOUND)

Angular uses client-side routes (`/command-center`, `/investigations`, …).  
Without a rewrite, Vercel looks for a physical file and returns **404**.

`vercel.json` sends all routes to `index.html`:

```json
"rewrites": [{ "source": "/(.*)", "destination": "/index.html" }]
```

After deploy, test:

- `https://YOUR-APP.vercel.app/command-center`
- Refresh the page — should stay on the app, not 404.

---

## API URL (production)

`environment.prod.ts`:

```ts
apiBaseUrl: 'https://fuelguard-ai-3bm6.onrender.com/api'
```

Used via `API_BASE_URL` in `app.config.ts`. All HTTP calls go to Render over **HTTPS**.

To change the API host later, update `environment.prod.ts` or set GitHub/Vercel build override (see CI `API_BASE_URL`).

---

## Render CORS (required)

In Render → service **fuelguard-ai** → Environment:

```text
Cors__AllowedOrigins=https://YOUR-APP.vercel.app
```

Use your real Vercel URL (and custom domain if any). Multiple origins: comma-separated.

Redeploy the API after changing CORS.

---

## Environment variables (optional)

| Platform | Variable | When |
|----------|----------|------|
| Vercel | `API_BASE_URL` | Only if you use a custom build script to patch `environment.prod.ts` |
| GitHub Actions | `vars.API_BASE_URL` | CI frontend build (`.github/workflows/ci.yml`) |

Default production build uses the committed `environment.prod.ts` URL.

---

## Local validation

```powershell
cd fuelguard-webclient
npm ci
npm run build
Test-Path dist/fuelguard-web/browser/index.html   # must be True
npx --yes serve dist/fuelguard-web/browser -p 4200
```

Open `http://localhost:4200/command-center` and refresh — `serve` may not mimic Vercel rewrites; production behavior is validated on Vercel.

---

## Troubleshooting

| Symptom | Cause | Fix |
|---------|--------|-----|
| **404 NOT_FOUND** on refresh | Missing SPA rewrite or wrong output dir | `vercel.json` in `fuelguard-webclient`; output `dist/fuelguard-web/browser` |
| Blank page / 404 on `/` | Output points to `dist/fuelguard-web` (no `browser/`) | Use `dist/fuelguard-web/browser` |
| API calls to localhost | Old build or wrong env | Redeploy; check `environment.prod.ts` |
| CORS error in browser | Render `Cors__AllowedOrigins` | Add exact Vercel origin, redeploy API |
| Build fails on Vercel | Wrong root directory | Root = `fuelguard-webclient`, not repo root |
| `yarn` errors | Project uses **npm** | Install: `npm ci` |

---

## Full stack checklist

- [ ] Render API: `GET https://fuelguard-ai-3bm6.onrender.com/health` → 200
- [ ] Vercel: site loads, routes work on refresh
- [ ] Browser DevTools → Network: requests to `fuelguard-ai-3bm6.onrender.com/api/...`
- [ ] No CORS errors
- [ ] `Cors__AllowedOrigins` includes Vercel URL

See also: [DEPLOY_RENDER.md](./DEPLOY_RENDER.md)
