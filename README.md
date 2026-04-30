# VeloraClient

Unity-клиент для Velora: проект с addressable-сценами, DI через VContainer, UI-слоем на View/ViewModel биндингах, websocket-подключением и protobuf-пакетами для обмена сообщениями.

## Требования

- Unity `6000.4.2f1`
- Git LFS, если в репозитории появятся крупные бинарные ассеты
- Локальный websocket-сервер для сетевого функционала: `ws://localhost:8080/velora`

Основные зависимости проекта:

- VContainer
- UniTask
- R3
- Addressables
- NativeWebSocket
- protobuf-unity / Google.Protobuf
- Odin Inspector
- DOTween
- Universal Render Pipeline

Часть пакетов подключена через `Packages/manifest.json`, vendor-плагины лежат в `Assets/Plugins`.

## Быстрый старт

1. Открой проект в Unity версии `6000.4.2f1`.
2. Дождись восстановления пакетов через Unity Package Manager.
3. Открой стартовую сцену:

   ```text
   Assets/Assets/Scenes/InitialScene.unity
   ```

4. Запусти Play Mode.
5. Для проверки чата подними websocket-сервер на `localhost:8080` с endpoint `/velora`.

> В `EditorBuildSettings.asset` сейчас может быть устаревшая ссылка на `SampleScene.unity`; актуальная точка входа проекта находится в `InitialScene.unity`.

## Структура проекта

```text
Assets/
  Assets/
    Prefabs/UI/          UI-prefab'ы: canvas, экраны, chat views
    Scenes/              InitialScene, LoadingScene, MainMenu, LobbyScene, GameScene
    Sprite/              Спрайты и фоны для UI
  Configs/               ScriptableObject-конфиги, загружаемые через Addressables
  Plugins/
    Protobuf/            packets.proto и сгенерированный Packets.cs
  Scripts/
    Core/
      DI/                Root lifetime scope и bootstrap
      Utils/
        Factory/         Фабрики View, ViewModel и Screen
        SceneManagement/ Загрузка addressable-сцен и прогресс загрузки
        Screens/         Screen service и постоянный UI root
        Pool/            Сервис object pool
        Logger/          Logger service с выводом в чат
        StateMachine/    State machine проекта и игры
    Network/             Клиент на NativeWebSocket
    UI/
      Core/              Базовые View, Screen, ViewModel и binder-классы
      Binders/           Конкретные UI binders
      Services/          Chat service и данные сообщений
      ViewModels/        ViewModel'и экранов
      Views/             MonoBehaviour-представления экранов
```

## Архитектура

### Bootstrap и DI

`RootBootstrap` собирает `RootLifeTimeScope`, инициализирует сервисы и переносит root object в `DontDestroyOnLoad`. Сервисы и фабрики регистрируются в `RootLifeTimeScope`; конфиги загружаются через Addressables по настроенному config label.

### Поток сцен

State machine проекта стартует с `ProjectBootstrapState`, загружает `MainMenu`, затем открывает `MainMenuScreen`. Нажатие Play в `MainMenuViewModel` переводит проект в `ProjectGameState`, загружает `GameScene` через `SceneLoader` и открывает `ChatScreen`.

Сцены загружаются additively через Addressables:

- `InitialScene`
- `LoadingScene`
- `MainMenu`
- `LobbyScene`
- `GameScene`

### UI

Экраны открываются через `IScreenService`. `ScreensFactory` загружает prefab'ы экранов из `ScreensData`, создаёт их через `ViewsFactory` и размещает под постоянным UI root из `IUiRootService`.

UI-слой использует простой MVVM-подход с биндингами:

- `View<TViewModel>` создаёт и инициализирует view model.
- `[AutoBind]` помечает соответствующие binders во view и view model.
- `AutoBindResolver` связывает binders по явному key или нормализованному имени поля.
- `ViewBinder<T>` прокидывает значения в Unity UI controls.
- `ViewModelBinder<T>` отдаёт reactive values/commands из view model.

### Сеть

`WebsocketConnectionService` подключается к:

```text
ws://localhost:8080/velora
```

Сообщения сериализуются через protobuf на основе `Assets/Plugins/Protobuf/packets.proto`. Текущая модель пакетов поддерживает:

- `ChatMessage`
- `IdMessage`
- `Packet` с `sender_id` и `oneof msg`

### Addressables и конфиги

Addressables используются для:

- загрузки сцен;
- загрузки prefab'ов экранов;
- загрузки конфигов.

Важные конфиги:

- `Assets/Configs/SceneConfig.asset` - prefab canvas и маппинг экранов;
- `Assets/Configs/PoolParameters.asset` - настройки object pool;
- `Assets/Configs/LoggerParameters.asset` - цвета сообщений логгера.

## Заметки по разработке

- Новый экран добавляется через создание prefab'а `Screen<TViewModel>`, перевод prefab'а в Addressables и добавление записи в `SceneConfig.asset`.
- Для сервисов предпочтительны constructor injection или VContainer injection; UI views получают зависимости через `ViewsFactory`.
- Переходы между сценами лучше держать за `SceneLoader` и project states, а не загружать сцены напрямую из UI-кода.
- Изменения сетевых пакетов нужно синхронизировать между `packets.proto` и сгенерированными C# protobuf-классами.

## Лицензия

MIT. См. `LICENSE`.
