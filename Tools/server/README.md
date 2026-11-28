# Ограничение памяти сервера

`compose.memory.yml` — override-файл, который ставит лимит 8 ГБ ОЗУ на сервис игрового сервера.
Использование вместе с основным compose-файлом:

```bash
docker compose -f docker-compose.yml -f compose.memory.yml up -d
```

Если сервис в основном compose-файле называется иначе, замени `ss14-server` в `compose.memory.yml`
на имя своего сервиса. Проверить лимит:

```bash
docker stats
docker inspect <container> --format '{{.HostConfig.Memory}}'
```

`mem_limit` работает с `docker compose up` (не требуется swarm). Для swarm используй
`deploy.resources.limits.memory: 8g`.
