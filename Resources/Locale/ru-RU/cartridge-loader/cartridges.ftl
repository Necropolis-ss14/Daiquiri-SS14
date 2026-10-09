device-pda-slot-component-slot-name-cartridge = Картридж

default-program-name = Программа
notekeeper-program-name = Заметки
book-writer-program-name = Книги
book-writer-new-book = Новая книга
book-writer-back = Назад
book-writer-delete = Удалить
book-writer-save = Сохранить
book-writer-cancel = Отмена
book-writer-title-placeholder = Название книги...
book-writer-desc-placeholder = Короткое описание...
book-writer-cover = Обложка:
book-writer-add-page = + Стр.
book-writer-err-title = Название — минимум 3 символа.
book-writer-err-empty = Нужна хоть одна непустая страница.
book-writer-err-long = Слишком длинно: название до 64, описание до 140, страница до 12000 символов.
book-writer-save-draft = В черновик
book-writer-publish = Опубликовать
book-writer-draft-tag = [Черновик]
book-writer-drafts = Черновики
book-writer-editor-title = Новая книга
book-writer-print = Распечатать
book-writer-edit = Редактировать
book-writer-print-cooldown = Вы можете печатать книги только раз в минуту
book-writer-printed-page = Страница { $n }
ent-BookPrinted = распечатанная книга
ent-BookPrinted-desc = Книга, распечатанная из КПК.
book-writer-search-placeholder = Поиск...
book-writer-genre-all = Все жанры
book-writer-genre = Жанр:
book-writer-variant-archive = Архив
book-writer-variant-scarlet = Алая
book-writer-variant-herbal = Травник
book-writer-variant-grimoire = Гримуар
book-writer-variant-codex = Кодекс
book-writer-variant-folio = Фолиант
book-writer-variant-bestiary = Бестиарий
book-writer-variant-atlas = Атлас
book-writer-variant-chronicle = Хроника
book-writer-variant-songs = Песенник
book-writer-variant-necro = Некрономикон
book-writer-variant-almanac = Альманах
book-writer-genre-other = Прочее
book-writer-genre-scifi = Фантастика
book-writer-genre-horror = Ужасы
book-writer-genre-detective = Детектив
book-writer-genre-romance = Романтика
book-writer-genre-guide = Руководство
book-writer-genre-history = История
book-writer-genre-poetry = Поэзия
book-writer-genre-manga = Манга
book-writer-genre-comics = Комикс
book-writer-genre-hentai = Хентай
book-writer-genre-erotica = Эротика
book-writer-genre-fantasy = Фэнтези
book-writer-genre-thriller = Триллер
book-writer-genre-mystery = Мистика
book-writer-genre-adventure = Приключения
book-writer-genre-drama = Драма
book-writer-genre-comedy = Комедия
book-writer-genre-tragedy = Трагедия
book-writer-genre-fable = Басня
book-writer-genre-fairytale = Сказка
book-writer-genre-legend = Легенда
book-writer-genre-myth = Миф
book-writer-genre-biography = Биография
book-writer-genre-autobiography = Автобиография
book-writer-genre-diary = Дневник
book-writer-genre-textbook = Учебник
book-writer-genre-science = Научное
book-writer-genre-fanfic = Фанфик
book-writer-cover-blue = Синяя
book-writer-cover-red = Красная
book-writer-cover-green = Зелёная
book-writer-cover-purple = Фиолетовая
book-writer-cover-black = Чёрная
book-writer-cover-gold = Золотая
book-writer-icon-book = Книга
book-writer-icon-eye = Глаз
book-writer-icon-skull = Череп
book-writer-icon-stars = Звёзды
book-writer-icon-temple = Храм
book-writer-icon-diamond = Алмаз
book-writer-icon-magic = Магия
book-writer-icon-planet = Планета
nano-task-program-name = НаноДела
news-read-program-name = Новости станции

crew-manifest-program-name = Манифест экипажа
crew-manifest-cartridge-loading = Загрузка...
crew-manifest-cartridge-loading-failed = Ошибка загрузки манифеста экипажа!

net-probe-program-name = Зонд сетей
net-probe-scan = Просканирован { $device }!
net-probe-label-name = Название
net-probe-label-address = Адрес
net-probe-label-frequency = Частота
net-probe-label-network = Сеть

log-probe-program-name = Зонд логов
log-probe-scan = Загружены логи устройства { $device }!
log-probe-label-time = Время
log-probe-label-accessor = Использовано:
log-probe-label-number = #
log-probe-print-button = Распечатать логи
log-probe-printout-device = Сканированное устройство: { $name }
log-probe-printout-header = Последние логи:
log-probe-printout-entry = #{ $number } / { $time } / { $accessor }

astro-nav-program-name = АстроНав

med-tek-program-name = МедТек

# NanoTask cartridge

nano-task-ui-heading-high-priority-tasks =
    { $amount ->
        [zero] Нет задач высокого приоритета
        [one] 1 задача высокого приоритета
        [few] { $amount } задачи высокого приоритета
        *[other] { $amount } задач высокого приоритета
    }
nano-task-ui-heading-medium-priority-tasks =
    { $amount ->
        [zero] Нет задач среднего приоритета
        [one] 1 задача среднего приоритета
        [few] { $amount } задачи среднего приоритета
        *[other] { $amount } задач среднего приоритета
    }
nano-task-ui-heading-low-priority-tasks =
    { $amount ->
        [zero] Нет задач низкого приоритета
        [one] 1 задача низкого приоритета
        [few] { $amount } задачи низкого приоритета
        *[other] { $amount } задач низкого приоритета
    }
nano-task-ui-done = Готово
nano-task-ui-revert-done = Отмена
nano-task-ui-priority-low = Низкий
nano-task-ui-priority-medium = Средний
nano-task-ui-priority-high = Высокий
nano-task-ui-cancel = Отмена
nano-task-ui-print = Распечатать
nano-task-ui-delete = Удалить
nano-task-ui-save = Сохранить
nano-task-ui-new-task = Новая задача
nano-task-ui-description-label = Описание:
nano-task-ui-description-placeholder = Взять что-то важное
nano-task-ui-requester-label = Заявитель:
nano-task-ui-requester-placeholder = Джон Нанотрейзен
nano-task-ui-item-title = Редактировать задачу
nano-task-printed-description = [bold]Описание:[/bold] { $description }
nano-task-printed-requester = [bold]Заявитель:[/bold] { $requester }
nano-task-printed-high-priority = [bold]Приоритет[/bold]: [color=red]Высокий[/color]
nano-task-printed-medium-priority = [bold]Приоритет[/bold]: Средний
nano-task-printed-low-priority = [bold]Приоритет[/bold]: Низкий

# Wanted list cartridge
wanted-list-program-name = Список разыскиваемых
wanted-list-label-no-records = Всё спокойно, ковбой.
wanted-list-search-placeholder = Поиск по имени и статусу

wanted-list-age-label = [color=darkgray]Возраст:[/color] [color=white]{ $age }[/color]
wanted-list-job-label = [color=darkgray]Должность:[/color] [color=white]{ $job }[/color]
wanted-list-species-label = [color=darkgray]Вид:[/color] [color=white]{ $species }[/color]
wanted-list-gender-label = [color=darkgray]Гендер:[/color] [color=white]{ $gender }[/color]

wanted-list-reason-label = [color=darkgray]Причина:[/color] [color=white]{ $reason }[/color]
wanted-list-unknown-reason-label = неизвестная причина

wanted-list-initiator-label = [color=darkgray]Инициатор:[/color] [color=white]{ $initiator }[/color]
wanted-list-unknown-initiator-label = неизвестный инициатор

wanted-list-status-label = [color=darkgray]статус:[/color] { $status ->
    [suspected] [color=yellow]подозревается[/color]
    [wanted] [color=red]разыскивается[/color]
    [detained] [color=#b18644]под арестом[/color]
    [paroled] [color=green]освобождён по УДО[/color]
    [discharged] [color=green]освобождён[/color]
    [hostile] [color=darkred]враждебен[/color]
    [eliminated] [color=gray]ликвидирован[/color]
    *[other] нет
}

wanted-list-history-table-time-col = Время
wanted-list-history-table-reason-col = Преступление
wanted-list-history-table-initiator-col = Инициатор
