# Вкладка телепортации


abductors-ui-teleport = Телепорт
abductors-ui-attract = Притянуть

abductors-ui-gizmo-transferred = Информация о цели передана

# Вкладка экспериментов

abductors-ui-experiment = Эксперимент
abductors-ui-complete-experiment = Завершить эксперимент

# Вкладка брони

abductors-ui-armor-control = Управление бронёй

abductors-ui-combat-mode = Боевой режим
abductors-ui-stealth-mode = Скрытный режим

abductors-ui-lock-armor = Заблокировать броню
abductors-ui-unlock-armor = Разблокировать броню

abductors-ui-vest-linked = Жилет подключён

# Вкладка магазина

abductors-ui-shop = Магазин

abductors-ui-shop-Wonderprod = Вундерпрод
abductors-ui-shop-WeaponAlien = Инопланетный пистолет
abductors-ui-shop-ClothingHeadHelmetAbductor = Шлем
abductors-ui-shop-AbductorGizmo = Гизмо
abductors-ui-shop-AbductorExtractor = Экстрактор
abductors-ui-shop-MedkitCombat = Боевая аптечка
abductors-ui-shop-VendingMachineRestockAbductorDispenser = пополнение инопланетного раздатчика

# Роль призрака, цели и т.д.

abductors-ghost-role-name = Учёный-похититель
abductors-ghost-role-desc = Похищайте людей, напичкивайте их органами сомнительного происхождения.
abductora-ghost-role-name = Агент-похититель
abductora-ghost-role-desc = Похищайте людей, защищайте учёного.
abductors-ghost-role-rules = Вы — [color=red][bold]Похититель[/bold][/color].
                             Ваша задача — похищать людей со станции и заменять их органы различными экспериментальными устройствами,
                             после чего возвращать их обратно. Вам запрещено уничтожать станцию или намеренно убивать людей.
                             В ваших интересах возвращать подопытных живыми и здоровыми для чистоты эксперимента.

                             Вы не помните ничего из своей прошлой жизни и не помните ничего, что узнали будучи призраком.
                             Вам разрешено помнить знания об игре в целом, такие как готовка, использование предметов и т.д.
                             Вам категорически [color=red]ЗАПРЕЩЕНО[/color] помнить, например, имя, внешность и т.д. вашего прошлого персонажа.

abductor-round-end-agent-name = похититель

objective-issuer-abductors = [color=#FD0098]Материнский корабль[/color]

objective-condition-abduct-title = Похитить {$count} чел.
objective-condition-abduct-description = (используйте Гизмо на обездвиженной жертве, затем используйте Гизмо на консоли похитителей и выберите действие притяжения), затем замените их сердце одной из желёз, поместите их в экспериментатор и нажмите завершить эксперимент.

abductor-role-greeting = Я — профессиональный боевой учёный высокотехнологичной расы. Моя задача — похищать людей, проводить над ними эксперименты и возвращать их целыми для чистоты эксперимента. Не в моих интересах уничтожать станцию, убивать или помогать экипажу.

roles-antag-abductor-objective = Похищайте экипаж станции и проводите над ними свои эксперименты!
abductor-buy = Купить
abductor-console-balance = Баланс: { $balance }
abductor-dispencer = раздатчик: { $found ->
        [true] [color=green]подключён[/color]
       *[false] [color=red]не найден[/color]
    }
abductor-experimentator = экспериментатор: { $found ->
        [true] [color=green]подключён[/color]
       *[false] [color=red]не найден[/color]
    }
abductor-need-armor = [color=red][font size=16]Нужно подключить броню абдуктора![/font][/color]
abductor-pad = площадка: { $found ->
        [true] [color=green]подключена[/color]
       *[false] [color=red]не найдена[/color]
    }
abductor-price = { " " }Цена: { $price }
abductor-target = цель: [color=green]{ $name }[/color]
abductor-target-none = цель: [color=red]НЕТ[/color]
abductor-victim = жертва: [color=green]{ $name }[/color]
abductor-victim-none = жертва: [color=red]НЕТ[/color]
