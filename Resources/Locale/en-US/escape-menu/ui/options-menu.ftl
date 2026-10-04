## General stuff

<<<<<<< HEAD
ui-options-title = Игровые настройки
ui-options-tab-accessibility = Специальные возможности
ui-options-tab-admin = Администрирование
ui-options-tab-graphics = Графика
ui-options-tab-controls = Управление
ui-options-tab-audio = Аудио
ui-options-tab-network = Сеть
ui-options-tab-misc = Общие
=======
ui-options-title = Game Options
ui-options-tab-accessibility = Accessibility
ui-options-tab-admin = Admin
ui-options-tab-graphics = Graphics
ui-options-tab-controls = Controls
ui-options-tab-audio = Audio
ui-options-tab-misc = General
>>>>>>> upstream/starlight-dev

ui-options-apply = Сохранить и применить
ui-options-reset-all = Сбросить изменённые
ui-options-default = Сбросить настройки

ui-options-value-percent = { TOSTRING($value, "P0") }

# Misc/General menu

<<<<<<< HEAD
ui-options-discordrich = Включить расширенное присутствие Discord
ui-options-general-ui-style = Стиль интерфейса
ui-options-general-discord = Discord
ui-options-general-cursor = Курсор
ui-options-general-speech = Речь
ui-options-general-storage = Хранилища
ui-options-general-accessibility = Специальные возможности
=======
ui-options-discordrich = Enable Discord Rich Presence
ui-options-general-discord = Discord
ui-options-general-cursor = Cursor
ui-options-general-speech = Speech
ui-options-general-storage = Storage
>>>>>>> upstream/starlight-dev

## Audio menu

ui-options-master-volume = Общая громкость:
ui-options-midi-volume = Громкость MIDI (инструментов):
ui-options-ambient-music-volume = Громкость фоновой музыки:
ui-options-ambience-volume = Громкость окружения:
ui-options-lobby-volume = Громкость лобби и конца раунда:
ui-options-interface-volume = Громкость интерфейса:
ui-options-ambience-max-sounds = Одновременных звуков окружения:
ui-options-lobby-music = Музыка в лобби и в конце раунда
ui-options-restart-sounds = Звуки перезапуска раунда
ui-options-event-music = Музыка событий
ui-options-admin-sounds = Воспроизводить звуки администраторов
ui-options-bwoink-sound = Воспроизводить звук уведомления AHelp
ui-options-volume-label = Громкость

ui-options-audio-device = Output device:
ui-options-audio-device-default = Default device
ui-options-audio-hrtf = Enable HRTF (spatial audio, requires restart)

## Graphics menu

ui-options-display-label = Экран
ui-options-quality-label = Качество
ui-options-misc-label = Разное
ui-options-interface-label = Интерфейс

<<<<<<< HEAD
ui-options-auto-fill-highlights = Автоматически заполнять список подсветки на основе имени и профессии персонажа
ui-options-highlights-color = Цвет подсветки:
ui-options-highlights-color-example = Это выделенный текст.
ui-options-show-held-item = Показывать предмет в руке рядом с курсором
ui-options-show-combat-mode-indicators = Показывать индикаторы боевого режима у курсора
ui-options-opaque-storage-window = Непрозрачное окно хранилища
ui-options-show-ooc-patron-color = Показывать цвет Patreon в OOC
ui-options-show-looc-on-head = Показывать чат LOOC над головой персонажа
ui-options-fancy-speech = Показывать имена в облачках речи
ui-options-fancy-name-background = Добавлять фон именам в облачках речи
ui-options-vsync = Вертикальная синхронизация
ui-options-fullscreen = Полноэкранный режим
ui-options-lighting-label = Качество освещения:
ui-options-lighting-very-low = Очень низкое
ui-options-lighting-low = Низкое
ui-options-lighting-medium = Среднее
ui-options-lighting-high = Высокое
ui-options-scale-label = Масштаб интерфейса:
ui-options-scale-auto = Автоматически ({ TOSTRING($scale, "P0") })
=======
ui-options-auto-fill-highlights = Automatically set the highlights list based on your character's name and job
ui-options-highlights-color = Highlights color:
ui-options-highlights-color-example = This is highlighted text.
ui-options-show-held-item = Show held item next to cursor
ui-options-show-combat-mode-indicators = Show combat mode indicators with cursor
ui-options-opaque-storage-window = Opaque storage window
ui-options-show-ooc-patron-color = Show OOC Patreon color
ui-options-show-looc-on-head = Show LOOC chat above characters head
ui-options-fancy-speech = Show names in speech bubbles
ui-options-fancy-name-background = Add background to speech bubble names
ui-options-vsync = VSync
ui-options-fullscreen = Fullscreen
ui-options-lighting-label = Lighting Quality:
ui-options-lighting-very-low = Very Low
ui-options-lighting-low = Low
ui-options-lighting-medium = Medium
ui-options-lighting-high = High
ui-options-scale-label = UI Scale:
ui-options-scale-auto = Automatic ({ TOSTRING($scale, "P0") })
>>>>>>> upstream/starlight-dev
ui-options-scale-75 = 75%
ui-options-scale-100 = 100%
ui-options-scale-125 = 125%
ui-options-scale-150 = 150%
ui-options-scale-175 = 175%
ui-options-scale-200 = 200%
ui-options-hud-theme = Тема HUD:
ui-options-hud-theme-default = По умолчанию
ui-options-hud-theme-plasmafire = Плазменный огонь
ui-options-hud-theme-slimecore = Слаймкор
ui-options-hud-theme-clockwork = Часовой механизм
ui-options-hud-theme-retro = Ретро
ui-options-hud-theme-minimalist = Минималистичная
ui-options-hud-theme-ashen = Пепельная
ui-options-hud-layout-default = По умолчанию
ui-options-hud-layout-separated = Раздельный
ui-options-vp-stretch = Растягивать область просмотра под окно игры
ui-options-vp-scale = Фиксированный масштаб области просмотра:
ui-options-vp-scale-value = x{ $scale }

ui-options-vp-integer-scaling = Prefer integer scaling (might cause black bars/clipping)
ui-options-vp-integer-scaling-tooltip = If this option is enabled, the viewport will be scaled using an integer value
                                        at specific resolutions. While this results in crisp textures, it also often
                                        means that black bars appear at the top/bottom of the screen or that part
                                        of the viewport is not visible.
ui-options-filter-label = Scaling filter:
ui-options-filter-nearest = Nearest (no smoothing)
ui-options-filter-bilinear = Bilinear (smoothed)
ui-options-vp-vertical-fit = Vertical viewport fitting
ui-options-vp-vertical-fit-tooltip = When enabled, the main viewport will ignore the horizontal axis entirely when
                                     fitting to your screen. If your screen is smaller than the viewport, then this
                                     will cause the viewport to be cut off on the horizontal axis.
ui-options-vp-low-res = Low-resolution viewport
ui-options-parallax-low-quality = Low-quality Parallax (background)
ui-options-ambient-occlusion = Show Ambient Occlusion
ui-options-fps-counter = Show FPS counter
ui-options-vp-width = Viewport width:
ui-options-hud-layout = HUD layout:

## Controls menu

ui-options-hold-to-attack-melee = Удерживать для атаки (ближний бой)
ui-options-hold-to-attack-ranged = Удерживать для атаки (дальний бой)

ui-options-binds-reset-all = СБРОСИТЬ ВСЕ привязки клавиш
ui-options-binds-explanation = Нажмите, чтобы изменить привязку, кликните правой кнопкой, чтобы очистить
ui-options-unbound = Не назначено
ui-options-bind-reset = Сбросить
ui-options-key-prompt = Нажмите клавишу...

ui-options-header-movement = Движение
ui-options-header-camera = Камера
ui-options-header-interaction-basic = Основное взаимодействие
ui-options-header-interaction-adv = Расширенное взаимодействие
ui-options-header-ui = Интерфейс
ui-options-header-misc = Разное
ui-options-header-hotbar = Панель быстрого доступа
ui-options-header-shuttle = Шаттл
ui-options-header-map-editor = Редактор карт
ui-options-header-dev = Разработка
ui-options-header-general = Общее
ui-options-header-text-cursor = Текстовый курсор
ui-options-header-text-cursor-select = Выделение текста
ui-options-header-text-edit = Редактирование текста
ui-options-header-text-chat = Чат
ui-options-header-text-other = Другой ввод текста

ui-options-hotkey-keymap = Использовать клавиши US QWERTY
ui-options-hotkey-toggle-walk = Переключать режим ходьбы

ui-options-function-move-up = Двигаться вверх
ui-options-function-move-left = Двигаться влево
ui-options-function-move-down = Двигаться вниз
ui-options-function-move-right = Двигаться вправо
ui-options-function-walk = Ходить
ui-options-function-toggle-knockdown = Переключать ползание

ui-options-function-camera-rotate-left = Повернуть влево
ui-options-function-camera-rotate-right = Повернуть вправо
ui-options-function-camera-reset = Сбросить
ui-options-function-zoom-in = Приблизить
ui-options-function-zoom-out = Отдалить
ui-options-function-reset-zoom = Сбросить масштаб

ui-options-function-use = Использовать
ui-options-function-use-secondary = Использовать вторично
ui-options-function-alt-use = Альтернативное использование
ui-options-function-wide-attack = Широкая атака
ui-options-function-activate-item-in-hand = Активировать предмет в руке
ui-options-function-alt-activate-item-in-hand = Альтернативно активировать предмет в руке
ui-options-function-activate-item-in-world = Активировать предмет в мире
ui-options-function-alt-activate-item-in-world = Альтернативно активировать предмет в мире
ui-options-function-drop = Выбросить предмет
ui-options-function-examine-entity = Осмотреть
ui-options-function-swap-hands = Сменить руку
ui-options-function-swap-hands-reverse = Сменить руку (в другую сторону)
ui-options-function-move-stored-item = Переместить хранимый предмет
ui-options-function-rotate-stored-item = Повернуть хранимый предмет
ui-options-function-save-item-location = Сохранить расположение предмета
ui-options-static-storage-ui = Закрепить окно хранилища на панели быстрого доступа

ui-options-function-smart-equip-backpack = Быстро поместить в рюкзак
ui-options-function-smart-equip-belt = Быстро поместить на пояс
ui-options-function-smart-equip-suit-storage = Быстро поместить в хранилище костюма
ui-options-function-smart-equip-pocket1 = Быстро поместить в карман 1
ui-options-function-smart-equip-pocket2 = Быстро поместить в карман 2
ui-options-function-open-backpack = Открыть рюкзак
ui-options-function-open-belt = Открыть пояс
ui-options-function-throw-item-in-hand = Бросить предмет
ui-options-function-try-pull-object = Потянуть объект
ui-options-function-move-pulled-object = Переместить объект
ui-options-function-release-pulled-object = Отпустить объект
ui-options-function-point = Указать на место
ui-options-function-rotate-object-clockwise = Повернуть по часовой стрелке
ui-options-function-rotate-object-counterclockwise = Повернуть против часовой стрелки
ui-options-function-flip-object = Отразить

ui-options-function-focus-chat-input-window = Фокус на чате
ui-options-function-focus-local-chat-window = Фокус на чате (IC)
ui-options-function-focus-emote = Фокус на чате (эмоции)
ui-options-function-focus-whisper-chat-window = Фокус на чате (шёпот)
ui-options-function-focus-radio-window = Фокус на чате (радио)
ui-options-function-focus-looc-window = Фокус на чате (LOOC)
ui-options-function-focus-ooc-window = Фокус на чате (OOC)
ui-options-function-focus-admin-chat-window = Фокус на чате (администраторы)
ui-options-function-focus-dead-chat-window = Фокус на чате (мертвые)
ui-options-function-focus-console-chat-window = Фокус на чате (консоль)
ui-options-function-cycle-chat-channel-forward = Переключить канал (вперёд)
ui-options-function-cycle-chat-channel-backward = Переключить канал (назад)
ui-options-function-open-character-menu = Открыть меню персонажа
ui-options-function-open-context-menu = Открыть контекстное меню
ui-options-function-open-crafting-menu = Открыть меню создания
ui-options-function-open-inventory-menu = Открыть инвентарь
ui-options-function-open-a-help = Открыть помощь администратора и наставника
ui-options-function-open-abilities-menu = Открыть меню действий
ui-options-function-open-emotes-menu = Открыть меню эмоций
ui-options-function-toggle-round-end-summary-window = Переключить окно итогов раунда
ui-options-function-open-entity-spawn-window = Открыть меню создания сущностей
ui-options-function-open-sandbox-window = Открыть меню песочницы
ui-options-function-open-tile-spawn-window = Открыть меню создания тайлов
ui-options-function-open-decal-spawn-window = Открыть меню создания декалей
ui-options-function-open-admin-menu = Открыть меню администратора
ui-options-function-open-guidebook = Открыть справочник
ui-options-function-window-close-all = Закрыть все окна
ui-options-function-window-close-recent = Закрыть последнее окно
ui-options-function-show-escape-menu = Переключить игровое меню
ui-options-function-escape-context = Закрыть последнее окно или открыть игровое меню

ui-options-function-take-screenshot = Сделать снимок экрана
ui-options-function-take-screenshot-no-ui = Сделать снимок экрана (без интерфейса)
ui-options-function-toggle-fullscreen = Переключить полноэкранный режим

ui-options-function-editor-place-object = Разместить объект
ui-options-function-editor-cancel-place = Отменить размещение
ui-options-function-editor-grid-place = Разместить по сетке
ui-options-function-editor-line-place = Разместить линию
ui-options-function-editor-rotate-object = Повернуть
ui-options-function-editor-flip-object = Отразить
ui-options-function-editor-copy-object = Копировать

ui-options-function-show-debug-console = Открыть консоль
ui-options-function-show-debug-monitors = Показать отладочные мониторы
ui-options-function-inspect-entity = Исследовать сущность
ui-options-function-inspect-entity-tooltip = Открыть окно ViewVariables для сущности, на которую в данный момент наведён курсор.
ui-options-function-inspect-server-component = Исследовать серверный компонент
ui-options-function-inspect-server-component-tooltip = Открыть окно ViewVariables с серверным компонентом, заданным командой "quickinspect",
    для сущности, на которую в данный момент наведён курсор.
ui-options-function-inspect-client-component = Исследовать клиентский компонент
ui-options-function-inspect-client-component-tooltip = Открыть окно ViewVariables с клиентским компонентом, заданным командой "quickinspect",
    для сущности, на которую в данный момент наведён курсор.
ui-options-function-hide-ui = Скрыть интерфейс

ui-options-function-hotbar1 = Ячейка панели быстрого доступа 1
ui-options-function-hotbar2 = Ячейка панели быстрого доступа 2
ui-options-function-hotbar3 = Ячейка панели быстрого доступа 3
ui-options-function-hotbar4 = Ячейка панели быстрого доступа 4
ui-options-function-hotbar5 = Ячейка панели быстрого доступа 5
ui-options-function-hotbar6 = Ячейка панели быстрого доступа 6
ui-options-function-hotbar7 = Ячейка панели быстрого доступа 7
ui-options-function-hotbar8 = Ячейка панели быстрого доступа 8
ui-options-function-hotbar9 = Ячейка панели быстрого доступа 9
ui-options-function-hotbar0 = Ячейка панели быстрого доступа 0
ui-options-function-hotbar-shift1 = Ячейка панели быстрого доступа Shift+1
ui-options-function-hotbar-shift2 = Ячейка панели быстрого доступа Shift+2
ui-options-function-hotbar-shift3 = Ячейка панели быстрого доступа Shift+3
ui-options-function-hotbar-shift4 = Ячейка панели быстрого доступа Shift+4
ui-options-function-hotbar-shift5 = Ячейка панели быстрого доступа Shift+5
ui-options-function-hotbar-shift6 = Ячейка панели быстрого доступа Shift+6
ui-options-function-hotbar-shift7 = Ячейка панели быстрого доступа Shift+7
ui-options-function-hotbar-shift8 = Ячейка панели быстрого доступа Shift+8
ui-options-function-hotbar-shift9 = Ячейка панели быстрого доступа Shift+9
ui-options-function-hotbar-shift0 = Ячейка панели быстрого доступа Shift+0
ui-options-function-loadout1 = Комплект панели быстрого доступа 1
ui-options-function-loadout2 = Комплект панели быстрого доступа 2
ui-options-function-loadout3 = Комплект панели быстрого доступа 3
ui-options-function-loadout4 = Комплект панели быстрого доступа 4
ui-options-function-loadout5 = Комплект панели быстрого доступа 5
ui-options-function-loadout6 = Комплект панели быстрого доступа 6
ui-options-function-loadout7 = Комплект панели быстрого доступа 7
ui-options-function-loadout8 = Комплект панели быстрого доступа 8
ui-options-function-loadout9 = Комплект панели быстрого доступа 9
ui-options-function-loadout0 = Комплект панели быстрого доступа 0
ui-options-function-loadoutshift1 = Комплект панели быстрого доступа Shift+1
ui-options-function-loadoutshift2 = Комплект панели быстрого доступа Shift+2
ui-options-function-loadoutshift3 = Комплект панели быстрого доступа Shift+3
ui-options-function-loadoutshift4 = Комплект панели быстрого доступа Shift+4
ui-options-function-loadoutshift5 = Комплект панели быстрого доступа Shift+5
ui-options-function-loadoutshift6 = Комплект панели быстрого доступа Shift+6
ui-options-function-loadoutshift7 = Комплект панели быстрого доступа Shift+7
ui-options-function-loadoutshift8 = Комплект панели быстрого доступа Shift+8
ui-options-function-loadoutshift9 = Комплект панели быстрого доступа Shift+9
ui-options-function-loadoutshift0 = Комплект панели быстрого доступа Shift+0

ui-options-function-shuttle-strafe-up = Движение шаттла вверх
ui-options-function-shuttle-strafe-right = Движение шаттла вправо
ui-options-function-shuttle-strafe-left = Движение шаттла влево
ui-options-function-shuttle-strafe-down = Движение шаттла вниз
ui-options-function-shuttle-rotate-left = Повернуть влево
ui-options-function-shuttle-rotate-right = Повернуть вправо
ui-options-function-shuttle-brake = Тормозить

ui-options-function-text-cursor-left = Переместить курсор влево
ui-options-function-text-cursor-right = Переместить курсор вправо
ui-options-function-text-cursor-up = Переместить курсор вверх
ui-options-function-text-cursor-down = Переместить курсор вниз
ui-options-function-text-cursor-word-left = Переместить курсор влево на слово
ui-options-function-text-cursor-word-right = Переместить курсор вправо на слово
ui-options-function-text-cursor-begin = Переместить курсор в начало
ui-options-function-text-cursor-end = Переместить курсор в конец
ui-options-function-text-cursor-select = Выделить текст
ui-options-function-text-cursor-select-left = Расширить выделение влево
ui-options-function-text-cursor-select-right = Расширить выделение вправо
ui-options-function-text-cursor-select-up = Расширить выделение вверх
ui-options-function-text-cursor-select-down = Расширить выделение вниз
ui-options-function-text-cursor-select-word-left = Расширить выделение влево на слово
ui-options-function-text-cursor-select-word-right = Расширить выделение вправо на слово
ui-options-function-text-cursor-select-begin = Расширить выделение до начала
ui-options-function-text-cursor-select-end = Расширить выделение до конца
ui-options-function-text-backspace = Backspace
ui-options-function-text-delete = Delete
ui-options-function-text-word-backspace = Удалить слово назад
ui-options-function-text-word-delete = Удалить слово вперёд
ui-options-function-text-newline = Новая строка
ui-options-function-text-submit = Отправить
ui-options-function-multiline-text-submit = Отправить многострочный текст
ui-options-function-text-select-all = Выделить всё
ui-options-function-text-copy = Копировать
ui-options-function-text-cut = Вырезать
ui-options-function-text-paste = Вставить
ui-options-function-text-history-prev = Предыдущее из истории
ui-options-function-text-history-next = Следующее из истории
ui-options-function-text-release-focus = Снять фокус
ui-options-function-text-scroll-to-bottom = Прокрутить вниз
ui-options-function-text-tab-complete = Дополнение по Tab
ui-options-function-text-complete-next = Следующий вариант дополнения
ui-options-function-text-complete-prev = Предыдущий вариант дополнения

## Network menu

<<<<<<< HEAD
ui-options-net-predict = Предсказание на стороне клиента

ui-options-net-interp-ratio = Размер буфера состояния
ui-options-net-interp-ratio-tooltip = Увеличение этого значения обычно делает игру более устойчивой
    к потере пакетов от сервера к клиенту, однако при этом
    немного увеличивает задержку и требует от клиента
    предсказывать больше будущих тактов.

ui-options-net-predict-tick-bias = Смещение тактов предсказания
ui-options-net-predict-tick-bias-tooltip = Увеличение этого значения обычно делает игру более устойчивой
    к потере пакетов от клиента к серверу, однако при этом
    немного увеличивает задержку и требует от клиента
    предсказывать больше будущих тактов.

ui-options-net-pvs-spawn = Лимит появления сущностей PVS
ui-options-net-pvs-spawn-tooltip = Ограничивает скорость, с которой сервер отправляет клиенту только что созданные
    сущности. Снижение этого значения может уменьшить
    подёргивания из-за появления сущностей, но может привести к их
    внезапному появлению на экране.

ui-options-net-pvs-entry = Лимит сущностей PVS
ui-options-net-pvs-entry-tooltip = Ограничивает скорость, с которой сервер отправляет клиенту вновь видимые
    сущности. Снижение этого значения может уменьшить
    подёргивания, но может привести к внезапному появлению объектов.

ui-options-net-pvs-leave = Скорость удаления PVS
ui-options-net-pvs-leave-tooltip = Ограничивает скорость, с которой клиент удаляет
    сущности, находящиеся вне области видимости. Снижение этого значения может
    уменьшить подёргивания при перемещении, но иногда может
    привести к рассинхронизации предсказания и другим проблемам.

=======
>>>>>>> upstream/starlight-dev
## Toggle window console command

cmd-options-desc = Открывает меню настроек, при необходимости выбирая конкретную вкладку.
cmd-options-help = Использование: options [вкладка]

## Accessibility menu

ui-options-accessability-header-visuals = Визуальные эффекты
ui-options-accessability-header-content = Содержимое

ui-options-enable-color-name = Добавлять цвета к именам персонажей
ui-options-colorblind-friendly = Режим для людей с нарушением цветовосприятия
ui-options-reduced-motion = Уменьшать количество визуального движения
ui-options-screen-shake-intensity = Интенсивность тряски экрана

ui-options-chat-window-opacity = Прозрачность окна чата
ui-options-speech-bubble-text-opacity = Прозрачность текста в облачках речи
ui-options-speech-bubble-speaker-opacity = Прозрачность имени говорящего в облачках речи
ui-options-speech-bubble-background-opacity = Прозрачность фона облачков речи

ui-options-censor-nudity = Скрывать наготу персонажей

## Admin menu

ui-options-admin-player-panel = Список игроков в меню администратора

ui-options-admin-player-tab-symbol-setting = Символы антагонистов в списке персонажей
ui-options-admin-player-tab-symbol-setting-off = Без символа антагониста
ui-options-admin-player-tab-symbol-setting-basic = Показывать стандартный символ антагониста
ui-options-admin-player-tab-symbol-setting-specific = Показывать специальный символ антагониста

ui-options-admin-player-tab-role-setting = Настройки отображения роли
ui-options-admin-player-tab-role-setting-roletype = Показывать тип роли
ui-options-admin-player-tab-role-setting-subtype = Показывать подтип
ui-options-admin-player-tab-role-setting-roletypesubtype = Показывать тип и подтип роли
ui-options-admin-player-tab-role-setting-subtyperoletype = Показывать подтип и тип роли

ui-options-admin-player-tab-color-setting = Настройки цвета
ui-options-admin-player-tab-color-setting-off = Ненавижу цвета
ui-options-admin-player-tab-color-setting-character = Окрашивать имена персонажей-антагонистов
ui-options-admin-player-tab-color-setting-roletype = Окрашивать все типы ролей
ui-options-admin-player-tab-color-setting-both = Окрашивать оба

ui-options-admin-overlay-title = Оверлей администратора

ui-options-admin-overlay-antag-format = Стиль метки антагониста
ui-options-admin-overlay-antag-format-binary = Показывать статус антагониста
ui-options-admin-overlay-antag-format-roletype = Показывать тип роли
ui-options-admin-overlay-antag-format-subtype = Показывать подтип

ui-options-admin-overlay-antag-symbol = Стиль символа антагониста
ui-options-admin-overlay-antag-symbol-off = Без символа антагониста
ui-options-admin-overlay-antag-symbol-basic = Показывать стандартный символ антагониста
ui-options-admin-overlay-antag-symbol-specific = Показывать специальный символ антагониста

ui-options-admin-enable-overlay-playtime = Показывать время игры
ui-options-admin-enable-overlay-starting-job = Показывать начальную профессию
ui-options-admin-overlay-merge-distance = Дистанция объединения
ui-options-admin-overlay-ghost-fade-distance = Дистанция затухания оверлея призрака от мыши
ui-options-admin-overlay-ghost-hide-distance = Дистанция скрытия оверлея призрака от мыши
