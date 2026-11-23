# DiscordRoleBridge

Лёгкий сервис привязки Discord ↔ SS14 на своём VDS. Занимает ~50–100 МБ RAM, не требует Redis/Orleans.

## Что делает

1. Игрок жмёт «Привязать Discord» в игре — открывается страница сервиса.
2. На странице жмёт «Привязать Discord» — его кидает в Discord OAuth2.
3. После подтверждения сервис получает Discord ID и список ролей участника и сохраняет связку с GUID игрока.
4. Игровой сервер раз в несколько минут запрашивает роли по GUID и подставляет их в систему доната.

## Настройка Discord

1. Создать приложение на https://discord.com/developers/applications.
2. В OAuth2 → Redirects добавить `https://bridge.example.com/callback`.
3. В OAuth2 → URL Generator включить scope `identify`, `guilds`, `guilds.members.read`.
4. Скопировать Client ID и Client Secret.
5. Бот не обязателен: сервис читает роли через OAuth-токен пользователя (`/users/@me/guilds/{guild}/member`), поэтому пользователь должен состоять в указанном гильдии.

## Запуск

```bash
dotnet publish DiscordRoleBridge.csproj -c Release -o /opt/bridge
cp appsettings.example.json /opt/bridge/appsettings.json
nano /opt/bridge/appsettings.json
cd /opt/bridge && dotnet DiscordRoleBridge.dll
```

Порт 8080 по умолчанию. Перед сервисом рекомендуется nginx с HTTPS — Discord требует HTTPS для redirect URI.

## Связка с игровым сервером

В `serverconfig.toml`:

```toml
[discord_bridge]
enabled = true
url = "https://bridge.example.com"
token = "СЛУЧАЙНЫЙ_ТОКЕН_ИЗ_appsettings.json"
refresh_seconds = 300

[discord]
secret = "СЮДА_ТОТ_ЖЕ_СЕКРЕТ_ЧТО_И_SharedSecret"
```

`discord.secret` используется для подписи state — он должен совпадать с `SharedSecret` в `appsettings.json`.

## API

- `GET /link?state=<guid>.<hmac>` — страница привязки
- `GET /oauth?state=...` — редирект в Discord
- `GET /callback?code=...&state=...` — OAuth callback
- `GET /api/roles/{guid}` с заголовком `X-Api-Token` — `{ linked, discordId, roles }`
- `GET /api/link/{guid}` с заголовком `X-Api-Token` — `{ linked, discordId }`
- `GET /health` — проверка живости

Данные хранятся в `bridge-data.json` ( токены доступа, ID и роли ). Файл следует защитить правами 600.
