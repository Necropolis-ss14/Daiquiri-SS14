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
| `RobustToolbox/` (сабмодуль) | Space Wizards Federation | MIT |
| `Content.*/` (базовые каталоги) | Space Station 14 | MIT |
| `Content.*/_Starlight`, `Resources/**/_Starlight` | Starlight | MIT + категория до 2026-02-28 |
| `Content.*/_Goobstation`, `Resources/**/_Goobstation` | Goob Station | AGPL-3.0-or-later |
| `Content.*/_Lavaland`, `Resources/**/_Lavaland` | Goob Station | AGPL-3.0-or-later |
| `Content.*/_Adventure`, `Resources/**/_Adventure` | Adventure Space | MIT |
| `Content.*/_Daiquiri`, `Resources/**/_Daiquiri` | Daiquiri-SS14 | AGPL-3.0-or-later |

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
> **Сводная лицензия.** Daiquiri-SS14 объединяет наработки трёх проектов —
> **Space Station 14** (Space Wizards Federation), **Starlight** и **Goob Station**, —
> каждая из которых распространяется по своей лицензии.
> Полная карта лицензий и атрибуция — в [`LICENSE-Daiquiri.md`](LICENSE-Daiquiri.md).
> Код и контент из Goob Station распространяются по **AGPL-3.0-or-later**.

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
> это код Space Wizards Federation и Starlight.
</details>

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licence-AGPL--3.0--or--later-1f6feb?style=for-the-badge" alt="AGPL license"></a></summary>

> Код и прототипы, перенесённые из Goob Station, распространяются по
> [GNU AGPL v3.0 или новее](LICENSES/AGPL-3.0-or-later.txt). Их заголовки содержат
> `SPDX-License-Identifier: AGPL-3.0-or-later`.
</details>

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licence-CC_3.0_BY--SA-lightblue?style=for-the-badge" alt="Creative Commons 3.0 BY-SA"></a></summary>

> Медиа-ассеты (спрайты, звуки, шрифты) лицензируются отдельно: обычно
> [Creative Commons 3.0 BY-SA](https://creativecommons.org/licenses/by-sa/3.0/),
> если в метаданных ассета или его папке не указано иное. Часть ассетов
> распространяется под некоммерческими лицензиями (например, CC BY-NC-SA) и должна
> быть удалена при коммерческом использовании.
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
