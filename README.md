<div align="center">

<img src="Resources/Textures/Logo/logo.png" alt="Daiquiri-SS14" width="820" />

# Daiquiri-SS14

### Русскоязычный форк [Starlight](https://github.com/ss14Starlight/space-station-14) для [Space Station 14](https://github.com/space-wizards/space-station-14)

[![Space Station 14](https://img.shields.io/badge/Space_Station_14-Space_Wizards-blue?style=for-the-badge)](https://github.com/space-wizards/space-station-14)
[![Starlight](https://img.shields.io/badge/upstream-Starlight-8a2be2?style=for-the-badge)](https://github.com/ss14Starlight/space-station-14)
[![Goob Station](https://img.shields.io/badge/content-Goob_Station-e8710a?style=for-the-badge)](https://github.com/Goob-Station/Goob-Station)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-AGPL--3.0--or--later-1f6feb?style=for-the-badge)](LICENSE-Daiquiri.md)
[![Language](https://img.shields.io/badge/language-Russian-d52b1e?style=for-the-badge)](#)

Русскоязычный контент-пак на базе Starlight: собственный перевод, ксеноморфы,
антагонист «Инфицированный», контент из Goob Station и множество доработок
повседневных механик.

[**Возможности**](#-возможности) · [**В работе**](#-в-работе) · [**Сборка**](#-сборка-и-запуск) · [**Лицензии**](#-структура-и-лицензии) · [**Участие**](#-участие) · [**Лицензия**](#-лицензия)

</div>

---

## ✨ Возможности

| | Механика | Описание |
|:--:|---|---|
| 🌍 | **Русский перевод** | Полная локализация игрового контента на русский язык |
| 🧬 | **Ксеноморфы** | Улей, королева с яйцекладом, личинки, собственные спрайты и звуки |
| 👑 | **«Инфицированный»** | Новый антагонист: раундстарт и мидраунд, тепловизор, гибель тела и превращение в королеву |
| 🌡 | **Холод** | Гуманоид трясётся от холода и получает случайные сообщения об этом |
| 🤸 | **Эмоции** | Сальто, кувырок, прыжок и танец — перенесены из Goob Station |
| 🎛 | **Лобби и настройки** | Обновлённое лобби; отключение пластинки убирает и кнопку её смены |
| 🗣 | **TTS** | Автоматическое сканирование голосов, без новых протоколов |
| 📠 | **Цели станции** | Цели выдаются на факс в начале смены |
| 🖼 | **Спрайты Goob Station** | Эмиттеры, генераторы сдерживающего поля, сингулярность |
| 🏅 | **Титулы админов** | Титул активного администратора отображается в OOC перед ником |
| ⚡ | **Производительность** | Оптимизация потребления оперативной памяти |

---

## 🚧 В работе

| | Направление | Описание |
|:--:|---|---|
| 🌋 | **Лаваленд** | Перенос контента Goob Station (структуры, боссы и мобы) на собственную карту |

---

## 🖥 Сборка и запуск

### Требования

- [.NET SDK **10.0.100**](https://dotnet.microsoft.com/download) (см. `global.json`)
- [Python 3](https://www.python.org/) — для инициализации сабмодуля
- [Git](https://git-scm.com/)

### Шаги

```bash
# 1. Клонировать репозиторий с сабмодулем движка
git clone --recursive https://github.com/Necropolis-ss14/Daiquiri-SS14.git
cd Daiquiri-SS14

# 2. Инициализировать сабмодуль RobustToolbox (если не использовали --recursive)
python RUN_THIS.py

# 3. Собрать проект
dotnet build
```

### Запуск

```bash
# Сервер
runserver.bat          # Windows
./runserver.sh         # Linux / macOS

# Клиент
runclient.bat          # Windows
./runclient.sh         # Linux / macOS

# Сервер и клиент одной командой
runclientserver-Tools.bat
```

---

## 🗂 Структура и лицензии

<details>
<summary><b>Нажмите, чтобы развернуть карту лицензий</b></summary>

| Путь | Источник | Лицензия |
|---|---|---|
| `RobustToolbox/` (сабмодуль) | Space Wizards Federation | MIT (код движка) |
| `Content.*/**/*.cs` — **код** SS14 | Space Station 14 | MIT |
| `Content.*/_Starlight/**/*.cs` — **код** | Starlight | MIT; для вкладов 2024-11-04 — 2026-02-28 действует Starlight Fork License |
| `Content.*/_Goobstation/**/*.cs`, `Content.*/_Lavaland/**/*.cs` — **код** | Goob Station | AGPL-3.0-or-later |
| `Content.*/_Adventure/**/*.cs` — **код** | Adventure Space | MIT |
| `Content.*/_Daiquiri/**/*.cs` — **код** | Daiquiri-SS14 | AGPL-3.0-or-later |
| `Resources/**` — **контент** SS14/Starlight: прототипы, карты, спрайты, звуки, шрифты, локализация | Space Station 14 / Starlight | **CC BY-SA 3.0** (часть — CC BY-NC-SA 3.0), если в `meta.json`/`.license` не указано иное. **Не MIT и не AGPL.** |
| `Resources/**/_Goobstation`, `Resources/**/_Lavaland` | Goob Station | Лицензия из SPDX-заголовка/`.license`: код-прототипы — AGPL-3.0-or-later, медиа — CC BY-SA 3.0, если не указано иное |
| `Resources/**/_Adventure` | Adventure Space | Медиа — CC BY-SA 3.0, если не указано иное; код — MIT |
| `Resources/**/_Daiquiri` | Daiquiri-SS14 | Свои медиа — CC BY-SA 3.0; свои прототипы — AGPL-3.0-or-later |

> **Код и контент лицензируются раздельно.** Файлы `.cs` — MIT или AGPL по
> происхождению. Контент в `Resources/` (включая YAML-прототипы, карты и спрайты)
> лицензируется по CC BY-SA 3.0 (или иной лицензии, указанной в метаданных ассета).
> Мы не перелицензируем контент Space Station 14/Starlight под MIT/AGPL и не
> заявляем двойное лицензирование одного и того же файла.

Полная карта лицензий и атрибуции — в [`LICENSE-Daiquiri.md`](LICENSE-Daiquiri.md).

</details>

---

## 🤝 Участие

> **Об ИИ-вкладе.** Проект не принимает полностью сгенерированные ИИ правки.
> Ответственность за качество, проверку и доработку лежит на авторе правки.
> Использование инструментов-ассистентов (автодополнение в IDE, отладчики,
> разбор логов и т.п.) допустимо.

---

## ⚖️ Лицензия

> [!IMPORTANT]
> **Код и контент лицензируются отдельно.** Daiquiri-SS14 — производная работа,
> объединяющая наработки **Space Station 14** (Space Wizards Federation),
> **Starlight**, **Goob Station** и **Adventure Space**. **Код** (файлы `.cs`) —
> MIT у SS14/Starlight/Adventure и AGPL-3.0-or-later у Goob/Daiquiri. **Контент**
> (`Resources/`: прототипы, карты, спрайты, звуки, шрифты, локализация) —
> **CC BY-SA 3.0** (или иная, указанная в `meta.json`/`.license`), а не MIT/AGPL:
> мы не перелицензируем контент upstream и не заявляем двойное лицензирование
> одного файла. Полная карта и атрибуция — в [`LICENSE-Daiquiri.md`](LICENSE-Daiquiri.md).

> [!NOTE]
> **Идёт перелицензирование.** Лицензия Starlight (`LICENSE-Starlight.TXT`) действовала для
> вкладов Starlight с **2024-11-04** (коммит `84205e38`) по **2026-02-28** (коммит `01eff0f7`).
> Для вкладов этого периода она **остаётся в силе**, пока авторы явно не согласятся на
> перелицензирование. После получения согласия такие вклады переходят под MIT.
> Все вклады вне этого диапазона — под MIT (`LICENSE.TXT`).
> Запросы на перелицензирование отслеживаются в
> [issue #3499](https://github.com/ss14Starlight/space-station-14/issues/3499).

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licence-MIT-green?style=for-the-badge" alt="MIT license"></a></summary>

> Часть файлов распространяется по [лицензии MIT](https://opensource.org/license/MIT) —
> это код Space Wizards Federation, Starlight и Adventure Space. Оригинальные тексты
> лицензий сохранены в [`LICENSE.TXT`](LICENSE.TXT) и [`LICENSE-Starlight.TXT`](LICENSE-Starlight.TXT).
> MIT совместим с AGPL, поэтому такие файлы могут входить в состав AGPL-работы без
> отзыва исходного разрешения.
</details>

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licence-AGPL--3.0--or--later-1f6feb?style=for-the-badge" alt="AGPL license"></a></summary>

> Код, а также прототипы с заголовком `SPDX-License-Identifier: AGPL-3.0-or-later`,
> перенесённые из Goob Station и Daiquiri-SS14, распространяются по
> [GNU AGPL v3.0 или новее](LICENSES/AGPL-3.0-or-later.txt).
> Медиа-ассеты Goob/Daiquiri без AGPL-заголовка лицензируются по CC BY-SA 3.0 (см. ниже).
> При использовании модифицированной сборки как сетевого сервиса необходимо
> предоставить пользователям соответствующий исходный код (раздел 13 AGPL).
</details>

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licence-CC_3.0_BY--SA-lightblue?style=for-the-badge" alt="Creative Commons 3.0 BY-SA"></a></summary>

> Контент лицензируется отдельно: **прототипы (YAML), карты, спрайты, звуки,
> шрифты, локализация** — обычно
> [Creative Commons 3.0 BY-SA](https://creativecommons.org/licenses/by-sa/3.0/),
> если в метаданных ассета/`.license` не указано иное. Часть ассетов
> распространяется под некоммерческими лицензиями (например, CC BY-NC-SA) и должна
> быть удалена при коммерческом использовании. Контент SS14/Starlight лицензирован
> upstream именно под CC, поэтому для него не указываются MIT/AGPL.
> Отдельные аудиоассеты Starlight (`.../Experiment/*.ogg`, автор Orsoniks)
> распространяются по собственной «Cargo License» — см.
> [`LICENSE-Sawians.md`](LICENSE-Sawians.md).
</details>

---

<div align="center">

### Сделано на базе

<a href="https://github.com/space-wizards/space-station-14">
  <img src="https://raw.githubusercontent.com/space-wizards/space-station-14/master/Resources/Textures/Logo/logo.png" alt="Space Station 14" width="360" />
</a>

<a href="https://github.com/ss14Starlight/space-station-14">
  <img src="Resources/Textures/_Starlight/Logo/logo.png" alt="Starlight" width="300" />
</a>

<a href="https://github.com/Goob-Station/Goob-Station">
  <img src="https://raw.githubusercontent.com/Goob-Station/Goob-Station/master/Resources/Textures/Logo/logo.png" alt="Goob Station" width="360" />
</a>

[Space Station 14](https://github.com/space-wizards/space-station-14) ·
[Starlight](https://github.com/ss14Starlight/space-station-14) ·
[Goob Station](https://github.com/Goob-Station/Goob-Station)

<sub>Документация движка: <a href="https://docs.spacestation14.io/">docs.spacestation14.io</a></sub>

</div>
