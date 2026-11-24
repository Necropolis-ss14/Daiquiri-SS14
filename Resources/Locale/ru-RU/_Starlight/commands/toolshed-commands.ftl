command-description-radio-addcustom =
    Добавляет пользовательский канал в указанный компонент на переданной сущности. Укажите true или false в конце, чтобы убедиться, что компонент существует.
command-description-radio-remcustom =
    Удаляет пользовательский канал с данным ID из указанного компонента на переданной сущности.
command-description-container-insertentity =
    Вставляет данную сущность в указанный контейнер на переданной сущности.
command-description-container-insert =
    Вставляет переданные сущности в указанный контейнер на указанной сущности.
command-description-container-create =
    Создаёт новый контейнер на переданной сущности.
command-description-container-createslot =
    Создаёт новый слот-контейнер на переданной сущности.
command-description-container-delete =
    Удаляет контейнер на переданной сущности.
command-description-container-drop =
    Выбрасывает все содержащиеся сущности из указанного контейнера на переданной сущности.
command-description-container-dropandget =
    Выбрасывает все содержащиеся сущности из указанного контейнера на переданной сущности и возвращает все выброшенные предметы вместо переданной сущности.
command-description-container-dropanddelete =
    Выбрасывает все содержащиеся сущности из указанного контейнера на переданной сущности, затем удаляет контейнер.
command-description-container-get =
    Получает объект контейнера с данным ID контейнера на переданной сущности.
command-description-container-getentities =
    Получает все сущности в данном контейнере на переданной сущности.
command-description-container-getcontaining =
    Получает все контейнеры, в которых сейчас находится переданная сущность.
command-description-container-getoutercontainer =
    Получает самый внешний контейнер, в котором находится переданная сущность.
command-description-container-getowner =
    Получает сущность, владеющую указанным контейнером.
command-description-solution-adjcapacity =
    Настраивает вместимость данного раствора.
command-description-solution-adjtemperature =
    Настраивает вместимость данного раствора.
command-description-solution-adjthermalenergy =
    Настраивает вместимость данного раствора.
command-description-solution-create=
    Создаёт новый раствор с данным именем на переданной сущности. Возвращает существующий раствор, если он уже существует.
command-description-solution-delete=
    Удаляет указанный раствор на переданной сущности.
### Starlight (upstream #39080)
command-description-subtlemessage =
    Отправляет тонкое сообщение всем входным сущностям.
command-description-grid-getplayers =
    Получает всех игроков на переданных сетках
command-description-grid-get =
    Получает сетки, на которых стоят переданные игроки.
command-description-grid-getstation =
    Получает станции, на которых стоят переданные игроки, или станцию самой сущности, если передана сетка.
command-description-crewmanifest-addto =
    Добавляет переданную сущность в манифест экипажа указанной станции.
command-description-crewmanifest-removefrom =
    Удаляет переданную сущность из манифеста экипажа указанной станции.
command-description-crewmanifest-addplayer =
    Добавляет указанного игрока в манифесты экипажа переданных станций.
command-description-crewmanifest-removeplayer =
    Удаляет указанного игрока из манифестов экипажа переданных станций.
command-description-storage-reshape =
    Перестраивает хранилище на основе данных, переданных командой box2iconstructor.
command-description-box2iconstructor-new =
    Создаёт новое определение списка Box2i на сущности, свяжите в цепочку с командами box2iconstructor:add, затем завершите командой, которой это требуется.
command-description-box2iconstructor-add =
    Добавляет новый Box2i в существующее определение. Перед использованием вызовите box2iconstructor:new.
command-description-box2iconstructor-clean =
    Очищает неиспользуемое определение списка Box2i на сущности.
command-description-vector2dataconstructor-new =
    Создаёт новое определение списка Vector2 на сущности, свяжите в цепочку с командами vector2dataconstructor:add, затем завершите командой, которой это требуется.
command-description-vector2dataconstructor-add =
    Добавляет новый Vector2 в существующее определение. Перед использованием вызовите vector2dataconstructor:new.
command-description-vector2dataconstructor-clean =
    Очищает неиспользуемое определение списка Vector2 на сущности.
command-description-ccomp-ensure =
    Гарантирует, что все клиенты добавят компонент с указанным именем на сущность, если он существует.
command-description-ccomp-write =
    Пытается заставить всех клиентов записать что-то через vvwrite в клиентский компонент.
command-description-ccomp-rm =
    Гарантирует, что все клиенты удалят компонент с указанным именем с сущности, если он существует.
command-description-globalsound-play =
    Проигрывает звук глобально для переданных сущностей или сессий.
command-description-polymorph-begin =
    Маркер начала последовательности инструкций конфигурации полиморфа, прикрепит PolymorphSetupComponent к сущности.
command-description-polymorph-setproto =
    Устанавливает прототип, в который сущность полиморфируется.
command-description-polymorph-seteffect =
    Устанавливает прототип для спавна поверх полиморфированной сущности, обычно используется для создания спецэффектов.
command-description-polymorph-setdelay =
    Устанавливает, сколько секунд в секундах нужно ждать перед повторной активацией этого конкретного полиморфа.
command-description-polymorph-setduration =
    Устанавливает длительность полиморфа в секундах до автоматического отката.
command-description-polymorph-setforced =
    Устанавливает, что полиморф не может быть активирован или отменён самой сущностью.
command-description-polymorph-settransferdamage =
    Устанавливает перенос урона с текущей сущности на полиморфированную сущность.
command-description-polymorph-settransfername =
    Устанавливает наследование полиморфированной сущностью имени оригинала.
command-description-polymorph-settransferappearance =
    Устанавливает, переносить ли волосы, цвет кожи, рост и т.п. на полиморфированную сущность.
command-description-polymorph-setinventory =
    Устанавливает, как инвентарь сущности перенесётся на полиморфированную сущность.
command-description-polymorph-setrevertoncrit =
    Устанавливает, откатывать ли полиморф, когда сущность входит в критическое состояние.
command-description-polymorph-setrevertondeath =
    Устанавливает, откатывать ли полиморф, когда сущность убита.
command-description-polymorph-setrevertondelete =
    Устанавливает, откатывать ли полиморф, когда сущность удалена.
command-description-polymorph-setrevertoneat =
    Устанавливает, откатывать ли полиморф, когда сущность съедена.
command-description-polymorph-setallowrepeats =
    Устанавливает, разрешать ли повторные полиморфы.
command-description-polymorph-setignoreallowrepeats =
    Устанавливает разрешение полиморфа, даже если AllowRepeatedMorphs истинно.
command-description-polymorph-setcooldown =
    Устанавливает перезарядку в секундах перед следующим полиморфом.
command-description-polymorph-setentersound =
    Устанавливает звук, проигрываемый при входе в полиморф.
command-description-polymorph-setexitsound =
    Устанавливает звук, проигрываемый при выходе из полиморфа.
command-description-polymorph-clearentersound =
    Очищает звук, проигрываемый при входе в полиморф.
command-description-polymorph-clearexitsound =
    Очищает звук, проигрываемый при выходе из полиморфа.
command-description-polymorph-setenterpopup =
    Устанавливает попап, появляющийся при входе в полиморф.
command-description-polymorph-setexitpopup =
    Устанавливает попап, появляющийся при выходе из полиморфа.
command-description-polymorph-clearcopycomp =
    Очищает список компонентов для копирования в полиморф.
command-description-polymorph-addcopycomp =
    Добавляет запись в список компонентов для копирования в полиморф.
command-description-polymorph-rmcopycomp =
    Удаляет запись из списка компонентов для копирования в полиморф.
command-description-polymorph-apply =
    Мгновенно применяет полиморф и завершает.
command-description-polymorph-applyget =
    Мгновенно применяет полиморф и завершает, возвращая новую сущность.
command-description-polymorph-addaction =
    Добавляет действие полиморфа сущности с использованием текущей цепочки настройки полиморфа. После этого, вероятно, стоит вызвать polymorph:apply или polymorph:finish.
command-description-polymorph-addactionproto =
    Добавляет прототипированное действие полиморфа сущности.
command-description-polymorph-rmaction =
    Удаляет действие полиморфа с сущности, добавленное через polymorph:addaction.
command-description-polymorph-rmactionproto =
    Удаляет прототипированное действие полиморфа с сущности.
command-description-polymorph-revert =
    Откатывает к предыдущей x сущности, если возможно.
command-description-polymorph-reset =
    Сбрасывает полиморф сущности к исходному состоянию.
command-description-polymorph-finish =
    Помечает эту цепочку настройки полиморфа как завершённую, очищая и удаляя компонент.
command-description-vv-open =
    Открывает окно ViewVariables переданной сущности или пути.
command-description-vv-write =
    Изменяет значение пути через VV (View Variables). Для значения можно использовать переменную, но это должна быть сериализованная строка.
command-description-vv-owrite =
    Изменяет значение пути через VV (View Variables). Для значения можно использовать сырую переменную.
command-description-vv-read =
    Выводит значение пути через VV (View Variables).
command-description-vv-rsave =
    Получает значение пути через VV (View Variables). Может быть сохранено в переменную.
command-description-vv-rsaveraw =
    Получает значение пути через VV (View Variables). Может быть сохранено в переменную. Сохраняет сырое значение вместо сериализованной строки.
command-description-mind-wipe =
    Стирает разум игрока или сущности. Учтите, что это сделает его игру неиграбельной, пока вы не дадите ему новый разум.
command-description-mind-takeover =
    Напрямую захватывает моба, создавая разум, если его нет, и заставляя сущность стать разумной.
command-description-mind-takeoverwipe =
    Стирает ваш собственный разум, затем захватывает сущность. Это очистит все роли разума и цели и т.п.
command-description-mind-controlwipe =
    Стирает разум целевого игрока и заставляет его управлять переданной сущностью, создавая новый разум и делая сущность разумной.
command-description-killsign-set =
    Применяет киллсайн к сущности с использованием указанного состояния.
command-description-killsign-list =
    Перечисляет все доступные киллсайны.
command-description-killsign-rm =
    Удаляет киллсайн с сущности
command-description-fixinput =
    Обновляет контекст ввода сессии сущности.
command-description-faction-add =
    Добавляет фракцию этой сущности.
command-description-faction-remove =
    Удаляет фракцию у этой сущности.
command-description-faction-aggro =
    Делает эту сущность агрессивной к целевой сущности.
command-description-faction-deaggro =
    Делает эту сущность больше не агрессивной к целевой сущности.
command-description-faction-ignore =
    Заставляет эту сущность и целевую сущность игнорировать друг друга.
command-description-faction-unignore =
    Заставляет эту сущность и целевую сущность больше не игнорировать друг друга.
command-description-faction-clear =
    Очищает фракции этой сущности.
command-description-npc-sethtn =
    Создаёт НПС на сущности и устанавливает его HTN-компаунд.
command-description-npc-setenabled =
    Включает или отключает HTN-поведения этого НПС.
command-description-stationinit-begin =
    Начинает процесс инициализации новой мидраунд-станции. Прикрепляет BecomesStationMidRoundComponent к сетке.
command-description-stationinit-setid =
    Устанавливает ID станции. Это нужно для предотвращения дубликатов.
command-description-stationinit-clearbaseprotos =
    Очищает список базовых прототипов станций.
command-description-stationinit-addbaseproto =
    Добавляет базовый прототип станции для использования.
command-description-stationinit-rmbaseproto =
    Удаляет базовый прототип станции из использования.
command-description-stationinit-setallowftl =
    Устанавливает разрешение всем совершать ПБЛ-прыжки на карту, где находится эта станция.
command-description-stationinit-setuseemergencyshuttle =
    Устанавливает спавн аварийного шаттла для использования в конце раунда.
command-description-stationinit-setusearmories =
    Устанавливает спавн оружейных, которые можно отправить на станцию командой armory.
command-description-stationinit-setusearrivals =
    Устанавливает спавн шаттла прибытия для этой станции.
command-description-stationinit-setallowdungeonspawns =
    Устанавливает разрешение спавна данжей вроде ВГроида.
command-description-stationinit-setallowcargo =
    Устанавливает разрешение спавна грузовых шаттлов и АТС.
command-description-stationinit-clearallowedgridspawns =
    Очищает список гридспавнов, которым разрешено спавниться из базовых прототипов.
command-description-stationinit-addallowedgridspawn =
    Добавляет гридспавн, которому разрешено спавниться из базовых прототипов.
command-description-stationinit-rmallowedgridspawn =
    Удаляет гридспавн, которому разрешено спавниться из базовых прототипов.
command-description-stationinit-setemergencyshuttlepath =
    Устанавливает переопределение для сетки аварийного шаттла.
command-description-stationinit-clearjobs =
    Очищает все работы на этой станции.
command-description-stationinit-addjob =
    Добавляет новую работу на эту станцию.
command-description-stationinit-rmjob =
    Удаляет работу с этой станции.
command-description-stationinit-setallowevents =
    Устанавливает разрешение событиям нацеливаться на эту станцию.
command-description-stationinit-setdovariationpass =
    Устанавливает разрешение прохода раундстартовых вариаций на вновь созданной станции.
command-description-stationinit-namegrid =
    Переименовывает целевую сетку, имя сетки будет использовано для имени станции при инициализации.
command-description-stationinit-initialize =
    Завершает настройку и инициализирует станцию.
command-description-stationinit-initializeget =
    Завершает настройку и инициализирует станцию, возвращает вновь созданную сущность станции.
command-description-aitakeover =
    Заставляет переданную сущность захватить целевое ядро ИИ.
command-description-mobthreshold-initialize =
    Правильно инициализирует новый порог моба на сущности.
command-description-corporeal-on =
    Делает вашего призрака видимым и даёт ему возможность говорить.
command-description-corporeal-off =
    Делает вашего призрака невидимым и отзывает возможность говорить.
command-description-markup-adddesc =
    Добавляет размеченный текст в описание переданной сущности с данным ID.
command-description-markup-editdesc =
    Редактирует строку размеченного текста из описания переданной сущности с данным ID.
command-description-markup-rmdesc =
    Удаляет строку размеченного текста из описания переданной сущности с данным ID.
command-description-markup-cleardesc =
    Очищает все дополнительные строки размеченного текста из описания переданной сущности.
command-description-markup-listdesc =
    Перечисляет все разметки описания на переданной сущности и их ID.
command-description-atmos-add =
    Добавляет атмосферу на переданную сетку.
command-description-atmos-fix =
    Исправляет атмосферу переданной сетки.
command-description-atmos-rejoin =
    Пытается заставить переданное атмосферное устройство переподключиться к атмосфере.
command-description-jobs-makeunlimited =
    Делает слот работы безлимитным.
command-description-jobs-makelimited =
    Делает слот работы лимитным. Позволяет сбросить до 0 или до того, каким было бы мидраунд-значение.
command-description-vv-append =
    Добавить значение в коллекцию по пути через VV (просмотр переменных).
command-description-vv-rmat =
    Удалить элемент по указанному индексу из коллекции по пути через VV (просмотр переменных).
