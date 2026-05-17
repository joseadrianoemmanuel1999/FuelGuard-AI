# FuelGuard Web (Angular 20)

Frontend for FuelGuard AI. API runs on Render; this app deploys to **Vercel**.

## Development

```bash
npm ci
npm start
```

Dev API proxy: `proxy.conf.json` → `https://localhost:5001` (use `/api` via `environment.ts`).

## Production build

```bash
npm run build
# Output: dist/fuelguard-web/browser
```

## Deploy

See [../docs/DEPLOY_VERCEL.md](../docs/DEPLOY_VERCEL.md).

**Vercel Root Directory:** `fuelguard-webclient`
