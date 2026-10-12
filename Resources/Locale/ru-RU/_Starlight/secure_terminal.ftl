## Secure Command Terminal – UI strings

secure-terminal-window-title = Защищённый терминал
secure-terminal-requests-header = Запросы
secure-terminal-information-header = Информация
secure-terminal-authorization-header = Авторизация

secure-terminal-select-request = Выберите запрос из списка слева, чтобы увидеть подробности.

secure-terminal-request-button = Запросить
secure-terminal-request-button-confirm = Подтвердить?
secure-terminal-authorize-button = Авторизовать
secure-terminal-deny-button = Отклонить / Отменить
secure-terminal-recall-button = Отозвать арсенал
secure-terminal-recall-locked = { $minutes ->
    [1] Отзыв будет доступен через 1 минуту.
   *[other] Отзыв будет доступен через {$minutes} мин.
}
secure-terminal-used-note = Этот арсенал уже был окончательно активирован или отозван в этом раунде и не может быть развёрнут повторно.
secure-terminal-already-used = Этот ресурс уже был использован в этом раунде и не может быть запрошен повторно.

secure-terminal-auth-waiting = Нет активного предложения по этому запросу.
secure-terminal-auth-desc = Текущее предложение — нет ответа = [color=red]красный[/color], согласен = [color=green]зелёный[/color]:
secure-terminal-awaiting-member = Ожидание {$label}

secure-terminal-pending-countdown-label = Истекает через {$minutes}м {$seconds}с…
secure-terminal-countdown-label = Активация через {$minutes}м {$seconds}с…

secure-terminal-fee-note = Комиссия за обработку: {$fee}
secure-terminal-salary-note = Зарплата на станции снижена на {$penalty}% из-за расходов на мобилизацию.
secure-terminal-delay-note = { $minutes ->
    [1] ETA: 1 минута после авторизации.
   *[other] ETA: {$minutes} мин. после авторизации.
}
secure-terminal-delay-note-immediate = Вступает в силу сразу после авторизации.

secure-terminal-requires-no-war-note = Отключено во время War Ops.
secure-terminal-requires-war-note = Доступно только во время War Ops.
secure-terminal-requires-alert-note = Требуется активная тревога {$level}.
secure-terminal-alert-time-remaining = { $minutes ->
    [1] Тревога должна быть активна ещё 1 минуту, прежде чем это можно будет запросить.
   *[other] Тревога должна быть активна ещё {$minutes} мин., прежде чем это можно будет запросить.
}
secure-terminal-on-cooldown-note = { $minutes ->
    [1] На перезарядке — будет доступно через 1 минуту.
   *[other] На перезарядке — будет доступно через {$minutes} мин.
}
secure-terminal-requires-alert-suffix = Нужно: {$level}
secure-terminal-requires-war-suffix = Нужно: War Ops

secure-terminal-reason = Укажите причину запроса:

## Server → global announcements

secure-terminal-proposal-created = {$request} был запрошен и ожидает совместной авторизации.
secure-terminal-proposal-created-reason = {$request} был запрошен и ожидает совместной авторизации. Причина: {$reason}
secure-terminal-proposal-denied = Запрос {$request} был отменён.
secure-terminal-proposal-denied-cc = Запрос {$request} был отклонён Центральным Командованием.
secure-terminal-radio-proposal = {$request} был предложен. Пожалуйста, подойдите к ближайшему устройству авторизации ключ-картой, чтобы авторизовать или отклонить.
secure-terminal-radio-proposal-reason = {$request} был предложен. Пожалуйста, подойдите к ближайшему устройству авторизации ключ-картой, чтобы авторизовать или отклонить. Причина: {$reason}
secure-terminal-radio-denied = Запрос {$request} был отменён.
secure-terminal-activation-countdown = {$request} был полностью авторизован.
    Активация через {$minutes} мин.
    Зарплата на станции была снижена из-за расходов на мобилизацию.
secure-terminal-unknown-job = Неизвестно

## Popup messages

secure-terminal-no-station = Станция для этой консоли не найдена.
secure-terminal-request-denied = Доступ запрещён.
secure-terminal-authorize-denied = У вас нет необходимого допуска для совместной подписи этого запроса.
secure-terminal-requires-war = Этот запрос доступен только когда официально объявлены War Ops.
secure-terminal-wrong-alert = Текущий уровень тревоги не соответствует требованиям этого запроса.
secure-terminal-alert-not-long-enough = Уровень тревоги активен недостаточно долго для авторизации. Подождите и попробуйте снова.
secure-terminal-recall-too-soon = Арсенал развёрнут недостаточно долго для отзыва. Подождите.
secure-terminal-on-cooldown = Этот запрос на перезарядке.
secure-terminal-already-pending = Предложение по этому запросу уже ожидает рассмотрения.
secure-terminal-already-active = Другой запрос уже ожидает или активируется. Дождитесь его завершения, прежде чем создавать новый.
secure-terminal-no-active-proposal = Активное предложение по этому запросу не найдено.
secure-terminal-already-authorized = Вы уже авторизовали это предложение.
secure-terminal-already-activated = Этот терминал уже авторизовал это предложение.
secure-terminal-auth-note = Этот терминал предназначен только для авторизации.
secure-terminal-authorized-by = Внимание — запрос {$request} был авторизован. Авторизовали: {$signatories}.
secure-terminal-armory-recalled = Отдан приказ об отзыве {$request}. Развёртывание арсенала было отменено.
secure-terminal-awaiting-admin = Внимание — запрос {$request} был отправлен. Ожидается авторизация Центральным Командованием.
secure-terminal-admin = Запрос одобрения администратора для: {$request}
                        Причина: {$reason}
                        Используйте AGhost чтобы одобрить/отклонить запрос.

## Request names & descriptions

secure-terminal-warops-security-name = Ядерная группа реагирования
secure-terminal-warops-security-desc = Развёртывает охранный отряд ОБР, специализированный для War Ops. Доступно только во время War Ops.
                                       Используйте, когда станция подвергается прямому вооружённому нападению во время объявленных War Ops.
secure-terminal-warops-security-announcement = Группа экстренного реагирования — отряд безопасности спецназначения — была авторизована и уже в пути. Примерное прибытие: 30 минут.

secure-terminal-ert-security-name = ОБР Безопасность
secure-terminal-ert-security-desc = Развёртывает охранный отряд ОБР.
secure-terminal-ert-security-announcement = Группа экстренного реагирования — отряд безопасности — была авторизована и уже в пути. Примерное прибытие: 10 минут.

secure-terminal-ert-engineering-name = ОБР Инженерный
secure-terminal-ert-engineering-desc = Развёртывает инженерный отряд ОБР для помощи с критической инфраструктурой станции.
    Рекомендуется, когда станция получила катастрофические структурные, атмосферные или энергетические повреждения, превышающие возможности местного ремонта.
secure-terminal-ert-engineering-announcement = Группа экстренного реагирования — инженерный отряд — была авторизована и уже в пути. Примерное прибытие: 10 минут.

secure-terminal-ert-medical-name = ОБР Медицинский
secure-terminal-ert-medical-desc = Развёртывает медицинский отряд ОБР для сортировки массовых пострадавших и экстренной хирургии.
    Рекомендуется, когда медицинский отдел станции перегружен, выведен из строя или уничтожен.
secure-terminal-ert-medical-announcement = Группа экстренного реагирования — медицинский отряд — была авторизована и уже в пути. Примерное прибытие: 10 минут.

secure-terminal-ert-janitorial-name = ОБР Уборочный
secure-terminal-ert-janitorial-desc = Развёртывает уборочный отряд ОБР для опасной уборки и восстановления станции.
    Рекомендуется после масштабного биологического, химического или экологического загрязнения, требующего быстрой дезактивации.
secure-terminal-ert-janitorial-announcement = Группа экстренного реагирования — уборочный отряд — была авторизована и уже в пути. Примерное прибытие: 10 минут.

secure-terminal-ert-chaplain-name = ОБР Капеллан
secure-terminal-ert-chaplain-desc = Развёртывает капеллана ОБР для поддержки морали экипажа и отпевания.
    Обеспечивает пастырскую поддержку и поддерживает мораль экипажа во время затяжных чрезвычайных ситуаций.
secure-terminal-ert-chaplain-announcement = Группа экстренного реагирования — капелланство — была авторизована и уже в пути. Примерное прибытие: 10 минут.

secure-terminal-ert-cburn-name = ОБР РХБЗЗ
secure-terminal-ert-cburn-desc = Развёртывает отряд ОБР РХБЗЗ.
secure-terminal-ert-cburn-announcement = Группа экстренного реагирования — отряд РХБЗЗ — была авторизована и уже в пути. Примерное прибытие: 15 минут.

secure-terminal-code-gamma-name = Код ГАММА
secure-terminal-code-gamma-desc = Повышает станцию до тревоги [color=palevioletred]ГАММА[/color]. Военное положение — все гражданские должны быть сопровождены охраной в безопасные зоны.
    Охрана должна быть вооружена постоянно. Все гражданские должны явиться к ближайшему главе отдела и быть сопровождены в безопасное место. Аварийное освещение активируется.
secure-terminal-code-gamma-announcement = Внимание! Код ГАММА скоро вступит в силу. Будет введено военное положение. Весь экипаж немедленно явитесь к ближайшему главе отдела.

secure-terminal-end-gamma-name = Отмена тревоги ГАММА
secure-terminal-end-gamma-desc = Снимает тревогу [color=palevioletred]ГАММА[/color] и возвращает станцию к зелёному коду. Требуется, чтобы ГАММА была активна не менее 15 минут.
secure-terminal-end-gamma-announcement = Код ГАММА снимается. Станция возвращается к нормальной работе. Оставайтесь начеку и ждите дальнейших указаний от вашего главы отдела.

secure-terminal-code-psi-name = Код ПСИ
secure-terminal-code-psi-desc = Повышает станцию до тревоги [color=mediumpurple]ПСИ[/color]. Обнаружены враждебные синтетики — избегайте несоответствующих киборгов и ищите командный состав.
    Указывает на враждебную или несоответствующую активность киборгов. Весь экипаж должен избегать неизвестных боргов, держаться группами и искать указания глав отделов.
secure-terminal-code-psi-announcement = Внимание! Командование авторизовало код ПСИ. Единицы кремния не-НаноТрейзен опознаны как активная угроза. Весь экипаж — явитесь к ближайшему главе отдела.

secure-terminal-end-psi-name = Отмена тревоги ПСИ
secure-terminal-end-psi-desc = Снимает тревогу [color=mediumpurple]ПСИ[/color] и возвращает станцию к зелёному коду. Требуется, чтобы ПСИ была активна не менее 15 минут.
secure-terminal-end-psi-announcement = Код ПСИ снимается. Опознанная синтетическая угроза была нейтрализована. Станция возвращается к нормальной работе.

secure-terminal-armory-gamma-name = Гамма-арсенал
secure-terminal-armory-gamma-desc = Отправляет [color=palevioletred]Гамма-арсенал[/color] — склад тяжёлого вооружения для ситуаций ГАММА. Одноразовое развёртывание.
                                    Выдаёт тяжёлое охранное снаряжение уполномоченному персоналу.
secure-terminal-armory-gamma-announcement = Гамма-арсенал был авторизован и уже в пути.

secure-terminal-armory-psi-name = Пси-арсенал
secure-terminal-armory-psi-desc = Отправляет [color=mediumpurple]Пси-арсенал[/color] — антикибернетическое вооружение для ситуаций ПСИ. Одноразовое развёртывание.
                                  Предоставляет инструменты, необходимые для нейтрализации несоответствующих кремниевых форм жизни.
secure-terminal-armory-psi-announcement = Пси-арсенал был авторизован и уже в пути.

secure-terminal-med-pod-name = Экстренная медицинская капсула
secure-terminal-med-pod-desc = Отправляет экстренную медицинскую капсулу — быстроразвёртываемую сортировку с хирургическим и реанимационным оборудованием.
    Используйте, когда массовые пострадавшие превышают медицинские возможности станции.
secure-terminal-med-pod-announcement = Экстренная медицинская капсула была авторизована и уже в пути. Примерное прибытие: 5 минут.

secure-terminal-itg-salvage-team-name = Утилизаторская команда МТГ
secure-terminal-itg-salvage-team-desc = Нанимает местную утилизаторскую команду Межзвёздной торговой гильдии для борьбы с активными угрозами станции.
    Рекомендуется, когда персонал безопасности отсутствует или когда безопасность не может справиться без помощи.
secure-terminal-itg-salvage-team-announcement = Местная утилизаторская команда Межзвёздной торговой гильдии была нанята для борьбы с активными угрозами станции.

secure-terminal-dismiss-itg-salvage-team-name = Распустить команду МТГ
secure-terminal-dismiss-itg-salvage-team-desc = Завершает контракт помощи Межзвёздной торговой гильдии и возвращает их команду к стандартным обязанностям.
    Рекомендуется, когда все угрозы станции устранены.
secure-terminal-dismiss-itg-salvage-team-announcement = Контракт Межзвёздной торговой гильдии завершён, и их утилизаторская команда вернулась к стандартным обязанностям.

secure-terminal-nukerequest-name = Код самоуничтожения
secure-terminal-nukerequest-desc = Запросить коды ядерного самоуничтожения.
                                   Злоупотребление системой ядерных запросов не будет допускаться ни при каких обстоятельствах.
                                   Передача не гарантирует ответа.

secure-terminal-code-violet-name = Код ФИОЛЕТ
secure-terminal-code-violet-desc = Повышает станцию до тревоги [color=Violet]Фиолет[/color].

secure-terminal-end-violet-name = Отмена фиолетовой тревоги
secure-terminal-end-violet-desc = Снимает тревогу [color=Violet]Фиолет[/color] и возвращает станцию к зелёному коду. Требуется, чтобы фиолетовый код был активен не менее 10 минут.

secure-terminal-emergency-maintenance-name = Экстренный доступ в техтоннели
secure-terminal-emergency-maintenance-desc = Выдать экстренный доступ в технические помещения.
secure-terminal-emergency-maintenance-announcement = Ограничения доступа в технические помещения и внешние шлюзы были сняты.

secure-terminal-end-emergency-maintenance-name = Отозвать экстренный доступ в техтоннели
secure-terminal-end-emergency-maintenance-desc = Отозвать экстренный доступ в технические помещения.
secure-terminal-end-emergency-maintenance-announcement = Ограничения доступа в технические помещения и внешние шлюзы были возвращены.

secure-terminal-emergency-station-name = Общестанционный экстренный доступ
secure-terminal-emergency-station-desc = Активировать общестанционный экстренный доступ.
secure-terminal-emergency-station-announcement = Ограничения доступа на всех шлюзах станции были сняты из-за продолжающегося кризиса. Законы о проникновении продолжают действовать, если командование не прикажет иначе.

secure-terminal-end-emergency-station-name = Деактивировать общестанционный экстренный доступ
secure-terminal-end-emergency-station-desc = Деактивировать общестанционный экстренный доступ.
secure-terminal-end-emergency-station-announcement = Ограничения доступа на всех шлюзах станции были возвращены. Обратитесь за помощью к ИИ станции или коллеге, если вы застряли.

secure-terminal-admin-approval-approve = Одобрить
secure-terminal-admin-approval-authorized-by = Подписано:
secure-terminal-admin-approval-deny = Отклонить
secure-terminal-admin-approval-description = Действие: {$description}
secure-terminal-admin-approval-reason = Причина: {$reason}
secure-terminal-admin-approval-request = Запрос: {$request}
secure-terminal-admin-approval-title = Одобрение админа терминала
secure-terminal-ai-leadership-name = Руководство ИИ
secure-terminal-authorized-by-central-command = Центком подписал запрос.
secure-terminal-authorized-by-central-command-deferred = Центком передал решение командованию станции.
secure-terminal-authorized-by-label = Подписано:
secure-terminal-awaiting-admin-desc = Полностью одобрено локальным командованием. Ожидание ответа Центкома…
secure-terminal-captain-and-ntrep-name = Капитан и представитель NanoTrasen
secure-terminal-captain-name = Капитан
secure-terminal-captain-or-ntrep-name = Капитан или представитель NanoTrasen
secure-terminal-chief-medical-officer-name = Главный врач
secure-terminal-civilian-leadership-name = Гражданское руководство
secure-terminal-command-name = Командование
secure-terminal-engineering-leadership-name = Инженерное руководство
secure-terminal-fee-held = Удержано { $fee }₡ до авторизации.
secure-terminal-head-of-security-name = Глава службы безопасности
secure-terminal-insufficient-funds = Недостаточно средств. Требуется: { $fee }₡
secure-terminal-med-and-science-leadership-name = Медицинское и научное руководство
secure-terminal-medical-leadership-name = Медицинское руководство
secure-terminal-proposal-cancelled-by = Терминал — {$actor} отменил запрос {$request}.
secure-terminal-proposal-rescinded-by = Терминал — запрос {$request} отозван: {$rescinders}.
secure-terminal-rescind-button = Отозвать
secure-terminal-rescind-label = Отозвать
secure-terminal-research-director-name = Научный руководитель
secure-terminal-salary-source-everyone = Все
secure-terminal-salary-source-interstellar-trade-guild = Межзвёздная торговая гильдия
secure-terminal-security-and-command-name = Безопасность и командование
secure-terminal-security-leadership-name = Руководство безопасности
secure-terminal-security-name = Безопасность
secure-terminal-ui-veto = Вето
secure-terminal-unlock-escape-pods-announcement = Командование разрешило использовать спасательные капсулы для эвакуации
secure-terminal-unlock-escape-pods-desc = Спасательные капсулы будут разблокированы, экипаж сможет запускать их по желанию
secure-terminal-unlock-escape-pods-name = Разблокировать капсулы
