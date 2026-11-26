# DiscordRoleBridge

Лёгкий сервис привязки Discord ↔ SS14 на своём VDS. Занимает ~50–100 МБ RAM, не требует Redis/Orleans.

## Что делает

1. Игрок жмёт «Привязать Discord» в игре — открывается страница сервиса.
2. На странице жмёт «Привязать Discord» — его кидает в Discord OAuth2.
3. После подтверждения сервис получает Discord ID и список ролей участника и сохраняет связку с GUID игрока.
4. Игровой сервер раз в несколько минут запрашивает роли по GUID и подставляет их в систему доната.
5. Опционально: сервис раз в N секунд читает `/status` игрового сервера и обновляет сообщение
   с онлайном в Discord (см. раздел «Онлайн в Discord»).

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

## Онлайн в Discord

Сервис может держать в Discord сообщение с текущим онлайном и сам его редактировать.

1. Создать бота: Discord Developer Portal → **Bot** → **Add Bot**, скопировать **Bot Token**.
2. Пригласить бота на сервер с правом **Send Messages** (и **Read Message History** для канала).
3. Узнать ID канала (включить Developer Mode → ПКМ по каналу → Copy Channel ID).
4. В `appsettings.json` заполнить:

```json
"Discord": {
  "BotToken": "ТОКЕН_БОТА"
},
"Status": {
  "Enabled": true,
  "ServerStatusUrl": "http://127.0.0.1:1212/status",
  "ChannelId": "ID_КАНАЛА",
  "MessageId": "",
  "UpdateSeconds": 30,
  "MaxPlayers": 0,
  "ServerName": "Daiquiri-SS14",
  "InviteUrl": "",
  "StatePath": "online-status.json"
}
```

- `ServerStatusUrl` — HTTP-порт игрового сервера (`http.port` в `server_config.toml`) + `/status`.
- `MessageId` можно оставить пустым: при первом запуске сервис отправит новое сообщение
  и сохранит его ID в `online-status.json`, чтобы после перезапуска продолжать редактировать его же.
- `UpdateSeconds` — период обновления, минимум 10 секунд. `MaxPlayers: 0` — не показывать лимит.
- При удалении сообщения сервис автоматически создаст новое.

Если `Enabled: false` или не задан `BotToken`/`ChannelId`, функция выключена и сервис работает как раньше.

## API

- `GET /link?state=<guid>.<hmac>` — страница привязки
- `GET /oauth?state=...` — редирект в Discord
- `GET /callback?code=...&state=...` — OAuth callback
- `GET /api/roles/{guid}` с заголовком `X-Api-Token` — `{ linked, discordId, roles }`
- `GET /api/link/{guid}` с заголовком `X-Api-Token` — `{ linked, discordId }`
- `GET /health` — проверка живости

Данные хранятся в `bridge-data.json` ( токены доступа, ID и роли ). Файл следует защитить правами 600.
