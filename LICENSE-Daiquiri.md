# Daiquiri-SS14 — Licensing and Attribution · Лицензирование и атрибуция

**[English](#english) · [Русский](#русский)**

---

# English

## Daiquiri-SS14 — Licensing and Attribution

This document describes how the **Daiquiri-SS14** repository is licensed and how the
work of every upstream project it is built on is attributed.

Daiquiri-SS14 is a community content pack for Space Station 14. It is a fork, and it
redistributes and modifies code, prototypes, localization and assets that originate
from five separate upstream projects:

| # | Upstream project | Repository | Role |
|---|---|---|---|
| 1 | **Space Station 14** — Space Wizards Federation | <https://github.com/space-wizards/space-station-14> | Base game and engine content |
| 2 | **Starlight** | <https://github.com/ss14Starlight/space-station-14> | The direct upstream fork this repository is derived from |
| 3 | **Goob Station** | <https://github.com/Goob-Station/Goob-Station> | Content (Lavaland, xenomorphs, emotes, sprites, and more) ported into this fork |
| 4 | **Adventure Space** (Adventure Space Nova) | <https://github.com/AdventureSpaceTeam/adventure_space_nova> | Bluespace artillery and related content ported into this fork |
| 5 | **Trauma Station** | <https://github.com/Trauma-Station/Trauma-Station> | Genetics system (DNA scanner, sequencer, mutations) and related content ported into this fork |

The original license of each upstream project continues to apply to the files that
originate from it. Nothing in this repository claims ownership over, or relicenses
away, the rights of those projects and their contributors.

### 1. License of the combined work

The combined Daiquiri-SS14 distribution is released under the **GNU Affero General
Public License, version 3.0 or later** (`AGPL-3.0-or-later`), because it includes
copyleft code and content ported from Goob Station.

Content (prototypes, maps, sprites, sounds, fonts, localization) is **not** relicensed
under AGPL: it stays under CC BY-SA 3.0 (or the per-asset license) as stated by the
project it originates from. Only `.cs` code follows the code license of its origin.

The full text of the AGPL is available in [`LICENSES/AGPL-3.0-or-later.txt`](LICENSES/AGPL-3.0-or-later.txt).

Files that originate from Space Station 14 and Starlight remain available to you
**under their original licenses** (see section 3). Because MIT permits sublicensing,
those files may also be redistributed as part of this AGPL-licensed combined work,
but the original permissive grant is not withdrawn and you may still take those
files under their original terms.

If you reuse only a part of this repository, check the license of that part:

- Files carrying a `SPDX-License-Identifier` header are licensed as stated in the header.
- Files with no header follow the license map in section 2.
- Media assets follow section 4.

### 2. License map

| Path / area | Origin | License |
|---|---|---|
| `RobustToolbox/` (submodule) | Space Wizards Federation | MIT (engine code) |
| `Content.*/**/*.cs` (base folders without an `_`-prefix fork folder) | Space Station 14 (Space Wizards Federation) | MIT (code) |
| `Content.Shared/_Starlight`, `Content.Server/_Starlight`, `Content.Client/_Starlight` | Starlight | MIT (code); legacy Starlight license applies to 2024-11-04 ... 2026-02-28 contributions (see section 3.2) |
| `Content.*/_Goobstation`, `Content.*/_Lavaland` (C# files) | Goob Station | AGPL-3.0-or-later (code) |
| `Content.*/_Adventure` (C# files) | Adventure Space | MIT (code) |
| `Content.*/_Daiquiri/Genetics` (C# files) | Trauma Station | AGPL-3.0-or-later (code) |
| `Content.*/_Daiquiri` (C# files) | Daiquiri-SS14 (this fork) | AGPL-3.0-or-later (code) |
| `Resources/**` (prototypes, maps, sprites, sounds, fonts, locale) | Space Station 14 (Space Wizards Federation) | **Content: CC BY-SA 3.0** (some CC BY-NC-SA 3.0); not MIT |
| `Resources/**/_Starlight` (prototypes, maps, sprites, sounds, locale) | Starlight | **Content: CC BY-SA 3.0**; not MIT/AGPL |
| `Resources/**/_Goobstation`, `Resources/**/_Lavaland` | Goob Station | Per-file: prototypes carrying `SPDX-License-Identifier: AGPL-3.0-or-later` are AGPL; media otherwise CC BY-SA 3.0 unless stated otherwise |
| `Resources/**/_Adventure` | Adventure Space | Media: CC BY-SA 3.0 unless stated otherwise |
| `Resources/**/_Daiquiri/Genetics` | Trauma Station | Prototypes: AGPL-3.0-or-later where marked; media: CC BY-SA 3.0 unless stated otherwise |
| `Resources/**/_Trauma` | Trauma Station | Media: CC BY-SA 3.0 unless stated otherwise (per-asset license in `meta.json` / `attributions.yml`) |
| `Resources/**/_Daiquiri` | Daiquiri-SS14 (this fork) | Own prototypes: AGPL-3.0-or-later; own media: CC BY-SA 3.0, unless stated otherwise |
| `Resources/Audio/_Starlight/Effects/Experiment/*.ogg`, `Resources/Audio/_Starlight/Voice/Experiment/*.ogg` | Orsoniks ("Cargo") | Custom "Cargo License" — see [`LICENSE-Sawians.md`](LICENSE-Sawians.md) |

Code and content are licensed separately: `.cs` files follow the code license of their
origin, while everything under `Resources/` (including YAML prototypes and maps) is
**content** and follows CC BY-SA 3.0 or the per-file license. Content from Space
Station 14 and Starlight is not relicensed under MIT/AGPL, and no file is claimed to be
dual-licensed.

`_Daiquiri` files that were ported from Goob Station (for example emotion, xenomorph,
Harpy and Lavaland-related content) are derivative of AGPL-3.0-or-later code and
remain AGPL-3.0-or-later.

`_Daiquiri/Genetics` files and prototypes ported from Trauma Station are derivative of
AGPL-3.0-or-later code and remain AGPL-3.0-or-later.

### 3. Attribution notices

#### 3.1 Space Station 14 — Space Wizards Federation

Base game code and content. Licensed under the MIT License, reproduced in
[`LICENSE.TXT`](LICENSE.TXT):

> Copyright (c) 2017-2026 Space Wizards Federation
>
> Permission is hereby granted, free of charge, to any person obtaining a copy of
> this software and associated documentation files (the "Software"), to deal in the
> Software without restriction, including without limitation the rights to use,
> copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
> Software, and to permit persons to whom the Software is furnished to do so,
> subject to the following conditions:
>
> The above copyright notice and this permission notice shall be included in all
> copies or substantial portions of the Software.

Source: <https://github.com/space-wizards/space-station-14>

#### 3.2 Starlight

Starlight contributions made from **2024-11-04** (commit `84205e38`) through
**2026-02-28** (commit `01eff0f7`) were released under the *Starlight Fork License*
(a modified MIT license that additionally requires attribution to the Starlight
project). That license remains in effect for those contributions until the respective
authors explicitly agree to relicensing; the text is preserved in
[`LICENSE-Starlight.TXT`](LICENSE-Starlight.TXT).

Contributions outside of that period, and any Starlight contribution whose author has
consented to relicensing, are licensed under the MIT License (`LICENSE.TXT`).

When reusing Starlight-derived material from this repository you must attribute the
Starlight project as its source: <https://github.com/ss14Starlight/space-station-14>

#### 3.3 Goob Station

All code and prototype content ported from Goob Station is licensed under the
**GNU Affero General Public License, version 3.0 or later**. Ported files keep — or
must keep — their `SPDX-License-Identifier: AGPL-3.0-or-later` header.

Ported content includes the **Harpy** species — the race itself together with its
flight, singing and syrinx (voice change) systems, prototypes and localization. The
Harpy prototypes, code and sprites in this repository originate from Goob Station and
its `_DV` (Delta-V) / `_EinsteinEngines` content trees, and are adapted here to the
Starlight/Daiquiri systems.

Source: <https://github.com/Goob-Station/Goob-Station>

Because this content is AGPL-licensed, if you run a modified version of Daiquiri-SS14
as a network service you must offer the corresponding source code to your users, as
described in section 13 of the AGPL.

#### 3.4 Adventure Space

Code ported from Adventure Space Nova (for example the bluespace artillery system) is
licensed under the MIT License, reproduced in the upstream repository's `LICENSE.TXT`.
Media assets from the same source follow CC BY-SA 3.0 unless their metadata says
otherwise.

Source: <https://github.com/AdventureSpaceTeam/adventure_space_nova>

#### 3.5 Trauma Station

The genetics subsystem (geneticist job, DNA scanner and console, genome sequencer,
unique enzymes, mutation disks, mutator tools, printouts and the mutation content
itself) was ported from **Trauma Station**. Ported code and prototypes are licensed
under the **GNU Affero General Public License, version 3.0 or later**, and keep — or
must keep — their `SPDX-License-Identifier: AGPL-3.0-or-later` header.

Media assets copied from Trauma Station under `Resources/Textures/_Trauma` and
`Resources/Audio/_Trauma` follow the license recorded in their `meta.json` /
`attributions.yml` (usually **CC BY-SA 3.0**, credited there to the original authors,
including /tg/station for the mutation action sprites and sounds).

Source: <https://github.com/Trauma-Station/Trauma-Station>

Because this content is AGPL-licensed, if you run a modified version of Daiquiri-SS14
as a network service you must offer the corresponding source code to your users, as
described in section 13 of the AGPL.

### 4. Media assets

Media assets (sprites, sounds, fonts) are **not** covered by the code licenses above
and each asset follows the license recorded for it:

- **Space Station 14 assets** are usually CC BY-SA 3.0, with some CC BY-NC-SA 3.0
  assets that must be removed for commercial use. The license of an asset is stated
  in the asset's `meta.json`.
- **Starlight assets** are licensed under **Creative Commons 3.0 BY-SA** unless
  stated otherwise in the containing folder or file.
- **Goob Station assets** are licensed under **CC BY-SA 3.0** unless stated otherwise
  (per-asset license and copyright are recorded in the asset metadata / `.license`
  files).
- **Trauma Station assets** are licensed under **CC BY-SA 3.0** unless stated otherwise
  (per-asset license and copyright are recorded in `meta.json` / `attributions.yml`).
- **Daiquiri-SS14 original assets** are licensed under **CC BY-SA 3.0** unless stated
  otherwise.
- The "Cargo" audio assets listed in [`LICENSE-Sawians.md`](LICENSE-Sawians.md) are
  governed by that separate custom license.

### 5. Original Daiquiri-SS14 contributions

Original work created for Daiquiri-SS14 is contributed under
**AGPL-3.0-or-later** for code and prototypes, and **CC BY-SA 3.0** for assets,
unless a file states otherwise.

### 6. No warranty

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### 7. License files in this repository

| File | Contents |
|---|---|
| [`LICENSE.TXT`](LICENSE.TXT) | MIT license — Space Wizards Federation and Starlight |
| [`LICENSE-Starlight.TXT`](LICENSE-Starlight.TXT) | Legacy Starlight Fork License (2024-11-04 … 2026-02-28) |
| [`LICENSE-Sawians.md`](LICENSE-Sawians.md) | Custom license for the "Cargo" audio assets |
| [`LICENSES/AGPL-3.0-or-later.txt`](LICENSES/AGPL-3.0-or-later.txt) | GNU Affero General Public License v3.0 or later (Goob Station and Daiquiri code) |
| `LICENSE-Daiquiri.md` | This document |

---

# Русский

## Daiquiri-SS14 — Лицензирование и атрибуция

Этот документ описывает, как лицензируется репозиторий **Daiquiri-SS14** и как
указывается авторство всех upstream-проектов, на которых он основан.

Daiquiri-SS14 — это контент-пак для Space Station 14 от сообщества. Это форк,
который распространяет и изменяет код, прототипы, локализацию и ассеты пяти
независимых upstream-проектов:

| # | Upstream-проект | Репозиторий | Роль |
|---|---|---|---|
| 1 | **Space Station 14** — Space Wizards Federation | <https://github.com/space-wizards/space-station-14> | Базовая игра и контент движка |
| 2 | **Starlight** | <https://github.com/ss14Starlight/space-station-14> | Прямой upstream-форк, от которого произведён этот репозиторий |
| 3 | **Goob Station** | <https://github.com/Goob-Station/Goob-Station> | Контент (лаваленд, ксеноморфы, эмоции, спрайты и прочее), перенесённый в этот форк |
| 4 | **Adventure Space** (Adventure Space Nova) | <https://github.com/AdventureSpaceTeam/adventure_space_nova> | Блюспейс-артиллерия и связанный контент, перенесённые в этот форк |
| 5 | **Trauma Station** | <https://github.com/Trauma-Station/Trauma-Station> | Генетика (ДНК-сканер, секвенатор, мутации) и связанный контент, перенесённые в этот форк |

Исходная лицензия каждого upstream-проекта продолжает действовать для файлов,
которые из него происходят. Ничто в этом репозитории не претендует на права
этих проектов и их участников и не лишает их этих прав.

### 1. Лицензия объединённой работы

Объединённый дистрибутив Daiquiri-SS14 распространяется под **GNU Affero General
Public License, версии 3.0 или новее** (`AGPL-3.0-or-later`), так как он включает
копилефт-код и контент, перенесённые из Goob Station.

Полный текст AGPL доступен в [`LICENSES/AGPL-3.0-or-later.txt`](LICENSES/AGPL-3.0-or-later.txt).

Файлы, происходящие из Space Station 14 и Starlight, остаются доступными вам
**по их исходным лицензиям** (см. раздел 3). Поскольку MIT разрешает
сублицензирование, эти файлы также могут распространяться в составе объединённой
работы под AGPL, однако исходное разрешительное предоставление прав не отзывается —
вы по-прежнему можете брать эти файлы на их исходных условиях.

Если вы используете только часть репозитория, проверьте лицензию этой части:

- Файлы с заголовком `SPDX-License-Identifier` лицензированы так, как указано в заголовке.
- Файлы без заголовка следуют карте лицензий в разделе 2.
- Медиа-ассеты следуют разделу 4.

### 2. Карта лицензий

| Путь / область | Происхождение | Лицензия |
|---|---|---|
| `RobustToolbox/` (сабмодуль) | Space Wizards Federation | MIT (код движка) |
| `Content.*/**/*.cs` (базовые каталоги без форк-каталога с префиксом `_`) | Space Station 14 (Space Wizards Federation) | MIT (код) |
| `Content.Shared/_Starlight`, `Content.Server/_Starlight`, `Content.Client/_Starlight` | Starlight | MIT (код); для вкладов 2024-11-04 ... 2026-02-28 действует legacy-лицензия Starlight (см. раздел 3.2) |
| `Content.*/_Goobstation`, `Content.*/_Lavaland` (C#) | Goob Station | AGPL-3.0-or-later (код) |
| `Content.*/_Adventure` (C#) | Adventure Space | MIT (код) |
| `Content.*/_Daiquiri/Genetics` (C#) | Trauma Station | AGPL-3.0-or-later (код) |
| `Content.*/_Daiquiri` (C#) | Daiquiri-SS14 (этот форк) | AGPL-3.0-or-later (код) |
| `Resources/**` (прототипы, карты, спрайты, звуки, шрифты, локализация) | Space Station 14 (Space Wizards Federation) | **Контент: CC BY-SA 3.0** (часть — CC BY-NC-SA 3.0); не MIT |
| `Resources/**/_Starlight` (прототипы, карты, спрайты, звуки, локализация) | Starlight | **Контент: CC BY-SA 3.0**; не MIT/AGPL |
| `Resources/**/_Goobstation`, `Resources/**/_Lavaland` | Goob Station | По файлам: прототипы с `SPDX-License-Identifier: AGPL-3.0-or-later` — AGPL; медиа — CC BY-SA 3.0, если не указано иное |
| `Resources/**/_Adventure` | Adventure Space | Медиа: CC BY-SA 3.0, если не указано иное |
| `Resources/**/_Daiquiri/Genetics` | Trauma Station | Прототипы: AGPL-3.0-or-later там, где указано; медиа: CC BY-SA 3.0, если не указано иное |
| `Resources/**/_Trauma` | Trauma Station | Медиа: CC BY-SA 3.0, если не указано иное (лицензия каждого ассета в `meta.json` / `attributions.yml`) |
| `Resources/**/_Daiquiri` | Daiquiri-SS14 (этот форк) | Свои прототипы: AGPL-3.0-or-later; свои медиа: CC BY-SA 3.0, если не указано иное |
| `Resources/Audio/_Starlight/Effects/Experiment/*.ogg`, `Resources/Audio/_Starlight/Voice/Experiment/*.ogg` | Orsoniks («Cargo») | Отдельная «Cargo License» — см. [`LICENSE-Sawians.md`](LICENSE-Sawians.md) |

Код и контент лицензируются раздельно: файлы `.cs` следуют лицензии кода по
происхождению, а всё в `Resources/` (включая YAML-прототипы и карты) является
**контентом** и следует CC BY-SA 3.0 или лицензии конкретного файла. Контент Space
Station 14 и Starlight не перелицензируется под MIT/AGPL, двойное лицензирование
одного файла не заявляется.

### 3. Атрибуция

#### 3.1 Space Station 14 — Space Wizards Federation

Код и контент базовой игры. Лицензированы под MIT License, текст воспроизведён в
[`LICENSE.TXT`](LICENSE.TXT):

> Copyright (c) 2017-2026 Space Wizards Federation
>
> Permission is hereby granted, free of charge, to any person obtaining a copy of
> this software and associated documentation files (the "Software"), to deal in the
> Software without restriction, including without limitation the rights to use,
> copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
> Software, and to permit persons to whom the Software is furnished to do so,
> subject to the following conditions:
>
> The above copyright notice and this permission notice shall be included in all
> copies or substantial portions of the Software.

Источник: <https://github.com/space-wizards/space-station-14>

#### 3.2 Starlight

Вклады Starlight, сделанные с **2024-11-04** (коммит `84205e38`) по
**2026-02-28** (коммит `01eff0f7`), были выпущены под *Starlight Fork License*
(изменённая лицензия MIT, которая дополнительно требует указания авторства
проекта Starlight). Эта лицензия остаётся в силе для данных вкладов, пока
соответствующие авторы явно не согласятся на перелицензирование; её текст сохранён
в [`LICENSE-Starlight.TXT`](LICENSE-Starlight.TXT).

Вклады вне этого периода, а также вклады Starlight, авторы которых согласились на
перелицензирование, лицензируются под MIT License (`LICENSE.TXT`).

При использовании материалов, происходящих от Starlight, вы обязаны указать проект
Starlight как источник: <https://github.com/ss14Starlight/space-station-14>

#### 3.3 Goob Station

Весь код и контент прототипов, перенесённые из Goob Station, лицензированы под
**GNU Affero General Public License, версии 3.0 или новее**. Перенесённые файлы
сохраняют — и должны сохранять — заголовок
`SPDX-License-Identifier: AGPL-3.0-or-later`.

Среди перенесённого контента — раса **Гарпия**: сама раса, а также системы полёта,
пения и сиринкса (смены голоса), её прототипы и локализация. Прототипы, код и
спрайты гарпий в этом репозитории происходят из Goob Station и его деревьев
контента `_DV` (Delta-V) / `_EinsteinEngines` и адаптированы здесь под системы
Starlight/Daiquiri.

Источник: <https://github.com/Goob-Station/Goob-Station>

Поскольку этот контент лицензирован под AGPL, если вы запускаете изменённую версию
Daiquiri-SS14 как сетевой сервис, вы обязаны предоставить пользователям
соответствующий исходный код, как описано в разделе 13 AGPL.

#### 3.4 Adventure Space

Код, перенесённый из Adventure Space Nova (например, система блюспейс-артиллерии),
лицензирован под MIT License; текст лицензии воспроизведён в `LICENSE.TXT` upstream-
репозитория. Медиа-ассеты из того же источника распространяются по CC BY-SA 3.0,
если в их метаданных не указано иное.

Источник: <https://github.com/AdventureSpaceTeam/adventure_space_nova>

#### 3.5 Trauma Station

Подсистема генетики (профессия генетика, ДНК-сканер и консоль, секвенатор генома,
уникальные ферменты, диски мутаций, инструменты-мутаторы, распечатки и сам контент
мутаций) перенесена из **Trauma Station**. Перенесённый код и прототипы
лицензированы под **GNU Affero General Public License, версии 3.0 или новее** и
сохраняют — и должны сохранять — заголовок `SPDX-License-Identifier: AGPL-3.0-or-later`.

Медиа-ассеты, скопированные из Trauma Station в `Resources/Textures/_Trauma` и
`Resources/Audio/_Trauma`, следуют лицензии, указанной в их `meta.json` /
`attributions.yml` (обычно **CC BY-SA 3.0**, с указанием исходных авторов, включая
/tg/station для спрайтов и звуков действий мутаций).

Источник: <https://github.com/Trauma-Station/Trauma-Station>

Поскольку этот контент лицензирован под AGPL, если вы запускаете изменённую версию
Daiquiri-SS14 как сетевой сервис, вы обязаны предоставить пользователям
соответствующий исходный код, как описано в разделе 13 AGPL.

### 4. Медиа-ассеты

Медиа-ассеты (спрайты, звуки, шрифты) **не** покрываются лицензиями кода выше;
каждый ассет следует лицензии, указанной для него:

- **Ассеты Space Station 14** обычно под CC BY-SA 3.0; часть ассетов под
  CC BY-NC-SA 3.0 требует удаления при коммерческом использовании. Лицензия ассета
  указана в его `meta.json`.
- **Ассеты Starlight** лицензированы под **Creative Commons 3.0 BY-SA**, если в
  содержащем их каталоге или файле не указано иное.
- **Ассеты Goob Station** лицензированы под **CC BY-SA 3.0**, если не указано иное
  (лицензия и правообладатель каждого ассета указаны в метаданных ассета /
  файлах `.license`).
- **Ассеты Trauma Station** лицензированы под **CC BY-SA 3.0**, если не указано иное
  (лицензия и правообладатель каждого ассета указаны в `meta.json` /
  `attributions.yml`).
- **Оригинальные ассеты Daiquiri-SS14** лицензированы под **CC BY-SA 3.0**, если не
  указано иное.
- Звуковые ассеты «Cargo», перечисленные в [`LICENSE-Sawians.md`](LICENSE-Sawians.md),
  регулируются этой отдельной лицензией.

### 5. Оригинальные наработки Daiquiri-SS14

Оригинальные наработки, созданные для Daiquiri-SS14, передаются под
**AGPL-3.0-or-later** для кода и прототипов и под **CC BY-SA 3.0** для ассетов,
если в файле не указано иное.

### 6. Отсутствие гарантий

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### 7. Файлы лицензий в этом репозитории

| Файл | Содержимое |
|---|---|
| [`LICENSE.TXT`](LICENSE.TXT) | MIT License — Space Wizards Federation и Starlight |
| [`LICENSE-Starlight.TXT`](LICENSE-Starlight.TXT) | Legacy-лицензия Starlight Fork (2024-11-04 … 2026-02-28) |
| [`LICENSE-Sawians.md`](LICENSE-Sawians.md) | Отдельная лицензия на звуковые ассеты «Cargo» |
| [`LICENSES/AGPL-3.0-or-later.txt`](LICENSES/AGPL-3.0-or-later.txt) | GNU Affero General Public License v3.0 или новее (код Goob Station и Daiquiri) |
| `LICENSE-Daiquiri.md` | Этот документ |

---

*Этот документ является обзором лицензирования и атрибуции, а не юридической
консультацией. При сомнениях по конкретному файлу смотрите его заголовок, его файл
`.license` или проект, из которого он пришёл.*

*This document is an attribution and licensing overview, not legal advice. When in
doubt about a specific file, consult its header, its `.license` file, or the project
it came from.*
