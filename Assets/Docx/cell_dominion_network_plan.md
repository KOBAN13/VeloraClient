# Cell Dominion — Network Development Plan

## Короткий вывод

Как сетевому разработчику лучше начинать не с движения и не с визуального спавна, а с сетевого контракта и минимальной авторитетной симуляции матча. В GDD игра завязана на проверку backend: комнаты, старт матча, синхронизация движения, серверная логика, спавн объектов, фазы и результат. Поэтому первый вертикальный срез должен доказать, что клиент отправляет только input, а сервер возвращает состояние мира.

Текущий клиент уже имеет базу: WebSocket transport, protobuf codec, `NetworkClient`, login/register/chat и DI через VContainer. Но `packets.proto` пока покрывает только auth/chat/id. Следующий крупный шаг — расширить протокол под lobby/match/input/snapshot/events и договориться с backend о стабильных правилах тиков.

## Базовые принципы

1. Server authoritative: клиент не сообщает "я подобрал ресурс" или "я нанёс урон", а только отправляет input.
2. Клиентская симуляция сначала read-only: отображает snapshots сервера, интерполирует позиции и отправляет команды.
3. Все сетевые сущности имеют server entity id.
4. Все сообщения матча должны быть идемпотентны или иметь sequence/tick, чтобы нормально переживать задержки, дубли и поздние пакеты.
5. Для MVP фиксируем матч на 2 игрока, 5 минут: Growth 2 минуты, War 3 минуты, Result 15 секунд.
6. Сначала делаем честный вертикальный срез "комната -> старт -> движение -> snapshot", потом добавляем механики слоями.

## Этап 0 — Зафиксировать сетевой контракт

Цель: убрать неоднозначность между клиентом и сервером до реализации gameplay.

1. Описать в `Assets/Plugins/Protobuf/packets.proto` группы сообщений:
   - session/auth: уже есть login/register/ok/deny, добавить session/token при необходимости;
   - lobby: create room, join room, leave room, ready, room state;
   - match lifecycle: match found/created, match start, phase changed, match ended;
   - client input: movement, attack, upgrade selection;
   - world state: full snapshot, delta snapshot, entity spawn/update/despawn;
   - errors: typed error code + reason.
2. Добавить базовые enum:
   - `MatchPhase`: preparation, growth, war, result;
   - `EntityType`: player_cell, core, nutrient, tank, mother, wall;
   - `UpgradeType`: damage, speed, max_hp, spawn_tank, spawn_mother;
   - `TeamSlot` или `SectorId`: 0..3;
   - `NetworkErrorCode`.
3. Ввести обязательные поля для матчевых сообщений:
   - `match_id`;
   - `server_tick`;
   - `client_sequence` для input;
   - `entity_id`;
   - `owner_id`.
4. Согласовать частоты:
   - input send rate: 20-30 Hz;
   - server simulation tick: 20-30 Hz;
   - snapshot rate: 10-20 Hz для MVP;
   - interpolation buffer: примерно 100-150 ms.
5. Обновить генерацию C# protobuf-классов и backend-классов из одного proto-файла.

Готово, когда клиент и сервер компилируются от одного proto и есть документированная таблица сообщений "кто кому и когда шлёт".

## Этап 1 — Привести транспорт к матчевой нагрузке

Цель: текущий WebSocket слой должен спокойно держать частые input/snapshot сообщения.

1. Добавить `NetworkConnectionService` поверх `INetworkClient`:
   - состояние connected/disconnected/reconnecting;
   - явный connect/disconnect;
   - единая публикация ошибок;
   - handshake/session после подключения.
2. Разделить packet routing:
   - не подписывать каждый сервис напрямую на весь `Received`;
   - сделать `NetworkMessageRouter`, который маршрутизирует oneof-сообщения по типу.
3. Добавить request/response correlation для lobby/auth команд:
   - `request_id`;
   - timeout;
   - retry только там, где это безопасно.
4. Для realtime input не использовать request/response:
   - input отправляется fire-and-forget;
   - сервер подтверждает принятый input через `last_processed_input_sequence` в snapshot.
5. Добавить логирование сетевых метрик:
   - ping/rtt;
   - snapshot age;
   - packets per second;
   - disconnect reason.

Готово, когда можно отправлять частые тестовые input-пакеты и принимать snapshots без засорения auth/chat-сервисов.

## Этап 2 — Lobby и старт матча

Цель: получить рабочий путь до GameScene без gameplay.

1. Реализовать client services:
   - `LobbyClientService`;
   - `RoomClientService`;
   - `MatchClientService`.
2. Реализовать сообщения:
   - create room;
   - join room;
   - leave room;
   - ready/unready;
   - room state snapshot;
   - match starting;
   - match started.
3. На клиенте сделать переход:
   - MainMenu/Login -> LobbyScene;
   - LobbyScene -> GameScene по `MatchStarted`.
4. В `MatchStarted` передавать:
   - `match_id`;
   - `player_id`;
   - `slot/sector`;
   - seed карты;
   - стартовый server tick/time.
5. На сервере на первом проходе поддержать только 2 игрока.

Готово, когда два клиента через ParrelSync могут зайти в одну комнату, нажать ready и одновременно перейти в GameScene.

## Этап 3 — Минимальный world snapshot

Цель: клиент рисует авторитетный мир сервера.

1. Описать минимальные entity snapshots:
   - player cell: id, owner, position, rotation/aim, hp, biomass, level, alive;
   - core: id, owner, position, hp;
   - nutrient: id, position, value, active;
   - wall/gate: id, open/closed;
   - match state: phase, phase time left, score.
2. На клиенте сделать `NetworkWorldStateService`:
   - хранит последнее состояние entities;
   - создаёт/обновляет/удаляет visual objects;
   - не принимает gameplay-решений.
3. Подключить object pool для визуальных сущностей.
4. Сделать простые prefabs-заглушки:
   - player cell;
   - core;
   - nutrient;
   - wall.
5. Добавить обработку full snapshot при входе в матч.

Готово, когда сервер может прислать состояние карты, а клиент видит игроков, cores, еду и стены в правильных секторах.

## Этап 4 — Input pipeline и движение

Цель: игрок управляет клеткой через сервер.

1. На клиенте собрать `PlayerInputNetworkService`:
   - WASD movement vector;
   - aim direction;
   - attack pressed/held;
   - client input sequence;
   - local timestamp.
2. Отправлять `ClientInputMessage` с фиксированной частотой, а не каждый frame.
3. На сервере:
   - валидировать input;
   - двигать player cell в авторитетной симуляции;
   - ограничивать движение сектором до War Phase;
   - возвращать position в snapshot.
4. На клиенте:
   - интерполировать remote entities;
   - для локального игрока сначала тоже использовать server snapshot без prediction;
   - добавить prediction позже, только если управление ощущается плохо.
5. Добавить отображение debug info:
   - current tick;
   - last acknowledged input;
   - interpolation delay.

Готово, когда два клиента видят движение друг друга, а проход через центральные стены запрещён сервером в Growth Phase.

## Этап 5 — Фазы матча и стены

Цель: сервер управляет временем и фазами, клиент только отображает.

1. Реализовать на сервере state machine матча:
   - preparation;
   - growth;
   - war;
   - result.
2. В snapshot передавать:
   - current phase;
   - phase start tick/time;
   - phase end tick/time;
   - remaining time.
3. На смене Growth -> War сервер открывает стены.
4. Клиент:
   - обновляет HUD таймера;
   - включает/выключает визуал стен по snapshot/event;
   - не решает локально, открыты стены или нет.

Готово, когда матч сам переходит в War Phase и оба клиента одновременно видят открытые проходы.

## Этап 6 — Сбор nutrients и biomass

Цель: первая полезная server-authoritative механика.

1. Сервер спавнит nutrients в секторах:
   - max 80 per zone;
   - spawn interval 0.5 sec;
   - value 1 или 5.
2. Сервер проверяет collision player cell -> nutrient.
3. Сервер начисляет:
   - biomass;
   - score;
   - level progress.
4. Клиент получает:
   - nutrient despawn/update;
   - player biomass;
   - score.
5. Клиент показывает HUD:
   - biomass;
   - level;
   - score.

Готово, когда игрок собирает еду без клиентского подтверждения, а второй клиент видит исчезновение тех же nutrients.

## Этап 7 — Level up и upgrades

Цель: добавить первый выбор игрока без сложной эволюции.

1. Сервер решает, когда игрок получил level up.
2. Сервер отправляет `UpgradeOfferedMessage`:
   - список из 3 вариантов;
   - offer id;
   - timeout, если нужен.
3. Клиент показывает простое окно выбора.
4. Клиент отправляет только `UpgradeSelected`:
   - offer id;
   - selected upgrade type.
5. Сервер валидирует выбор и применяет:
   - Damage Up;
   - Speed Up;
   - Max HP Up;
   - Spawn Tank.
6. Результат применения приходит через snapshot/events.

Готово, когда нельзя локально накрутить biomass/уровень/апгрейд, а выбранный upgrade виден обоим клиентам.

## Этап 8 — Бой, смерть и respawn

Цель: проверить сетевую валидацию урона и состояния alive/dead.

1. Клиент отправляет `attack_pressed` или `attack_held` в input.
2. Сервер:
   - проверяет cooldown;
   - выбирает валидную цель в range;
   - наносит damage;
   - начисляет score;
   - отправляет combat event для визуала.
3. Реализовать HP:
   - player cell;
   - core;
   - tank.
4. Реализовать смерть player cell:
   - alive=false;
   - input движения игнорируется;
   - respawn через 5 сек у core.
5. Реализовать уничтожение core:
   - player eliminated;
   - матч завершается, если остался один core.

Готово, когда клиент не может сам выбрать цель или урон, а смерть и respawn одинаково отображаются у всех.

## Этап 9 — Tank NPC

Цель: добавить первую автономную сущность, полностью управляемую сервером.

1. Tank создаётся только сервером после upgrade Spawn Tank.
2. Серверная логика Tank:
   - следует за владельцем;
   - если рядом enemy player или enemy tank, атакует;
   - в War Phase может атаковать enemy core;
   - не уходит слишком далеко от владельца до War Phase.
3. Snapshot Tank:
   - id;
   - owner;
   - position;
   - hp;
   - target id, если нужно для debug/анимаций.
4. Клиент только отображает движение и атаки Tank.

Готово, когда Tank появляется у обоих клиентов, ходит одинаково и наносит урон только по решению сервера.

## Этап 10 — Завершение матча и результаты

Цель: закрыть полный матчевый цикл.

1. Сервер завершает матч по условиям:
   - остался один живой core;
   - истёк таймер War Phase.
2. Сервер считает result:
   - place;
   - score;
   - gathered nutrients;
   - spawned units;
   - killed units;
   - core damage;
   - winner id.
3. Клиент получает `MatchEndedMessage`.
4. Клиент показывает Result Screen.
5. После result:
   - leave match;
   - return to lobby/main menu;
   - clean network world state.

Готово, когда можно сыграть матч от комнаты до result screen без перезапуска клиента.

## Этап 11 — Reconnect и устойчивость

Цель: не ломать матч при кратком обрыве.

1. Добавить reconnect handshake:
   - session token;
   - match id;
   - player id.
2. Сервер держит слот игрока N секунд после disconnect.
3. При reconnect клиент получает full snapshot.
4. Если игрок не вернулся:
   - в MVP можно считать его eliminated или idle;
   - решение должно быть одинаковым для всех клиентов.
5. Добавить обработку late join/rejoin в GameScene.

Готово, когда можно оборвать соединение одного клиента, переподключиться и восстановить состояние матча.

## Этап 12 — Полировка realtime-синхронизации

Цель: улучшить ощущения после того, как полный цикл уже работает.

1. Настроить interpolation buffer.
2. Добавить client-side prediction для локального движения, если задержка заметна.
3. Добавить reconciliation по `last_processed_input_sequence`.
4. Сгладить corrections:
   - snap только при больших расхождениях;
   - soft correction при малых.
5. Оптимизировать snapshot:
   - delta snapshots;
   - interest management по дистанции, если понадобится;
   - quantization координат.

Готово, когда движение локального игрока ощущается отзывчиво, а remote players не дёргаются при обычном ping.

## Что делать первым в коде клиента

1. Расширить `packets.proto` под lobby/match/input/snapshot.
2. Сгенерировать `Packets.cs`.
3. Добавить router поверх `INetworkClient`.
4. Добавить `LobbyClientService` и `MatchClientService`.
5. Добавить `NetworkWorldStateService`.
6. Подключить GameScene к full snapshot и простым visual prefabs.
7. Добавить отправку `ClientInputMessage` с фиксированной частотой.

## Что делать первым на backend

1. Поднять websocket endpoint, совместимый с текущим framing/protobuf.
2. Поддержать auth/session или временный dev player id.
3. Реализовать lobby room на 2 игрока.
4. Реализовать server tick loop.
5. Реализовать минимальный match state:
   - players;
   - cores;
   - walls;
   - phase timer.
6. Принимать client input.
7. Рассылать full snapshots.

## Рекомендуемый вертикальный срез 1

Минимальная цель первой рабочей итерации:

1. Два клиента подключаются.
2. Создают/заходят в комнату.
3. Нажимают ready.
4. Получают `MatchStarted`.
5. Видят 2 player cells, 2 cores и стены.
6. Двигаются через server-authoritative input.
7. Видят движение друг друга через snapshots.
8. Growth Phase не даёт пройти через стену.
9. После таймера War Phase открывает стену.

Только после этого стоит добавлять nutrients, biomass, upgrades, бой и NPC.

## Не делать в начале

1. Не делать client-authoritative сбор еды или урон.
2. Не начинать с prediction/reconciliation до обычных snapshots.
3. Не добавлять Mother, Defender, Spitter и снаряды до Tank.
4. Не делать 3-4 игроков до стабильного 2-player цикла.
5. Не делать красивую эволюцию до server-authoritative upgrades.
6. Не завязывать gameplay на Unity physics клиента.
7. Не размазывать обработку `Packet` по всем сервисам без router.

## Контрольные проверки

1. Два клиента видят одинаковые entity ids и позиции.
2. Отключение одного клиента не крашит второй.
3. Игрок не может собрать nutrient, отправив кастомный пакет.
4. Игрок не может нанести урон вне range/cooldown.
5. Серверный phase timer является единственным источником правды.
6. После full snapshot клиент может восстановить весь visual world.
7. Все матчевые сообщения логируются с `match_id` и `server_tick`.

