# Unity MVP Starter (Service Locator + EventBus + UI + Scene Loader)

Base project for Unity (2022.3.62f1) showcasing a clean, testable architecture:
- **Service Locator** + Installers (composition root)
- **EventBus** (decoupled communication)
- **SceneLoader** driven by events
- **UI Manager** (MVP) + scene anchors (**CanvasHub**)
- **AudioManager** (plain C# service + updater MonoBehaviour)

## Requirements
- Unity **2022.3.62f1**
- Git LFS (for binary assets)

## Quick Start
1. Clone the repo.
2. Open `Scenes/Boot.unity`.

## Architecture Overview
- **Core/ServiceLocator**: `ServiceLocator` and `Installer` base.
- **Core/Events**: `IEvent`, `IEventBus`, `EventBus`, plus UI/Scene events.
- **Core/Scenes**: `SceneLoader` subscribes to `LoadSceneRequest` / publishes progress/completed.
- **Core/UI**:
  - `UiManager` listens to `ShowView`/`HideView`/`ToggleView`.
  - `CanvasHub` provides scene-specific parents (Screen/Overlay/Popup).
  - `PresenterFactory` creates presenters per `ScreenId`.
  - MVP: `IView` + `BaseView` + `IPresenter` + `BasePresenter`.
- **Core/Audio**:
  - `AudioManager` is a non-MonoBehaviour service,
    uses `AudioManagerUpdater` to drive fades without coroutines.
  - Controls `AudioMixer` params: Music/SFX.

## Scenes
- **Boot**: composition root + GameBootstrapper (fires Main Menu load and `ShowView`).
- **MainMenu**: contains `CanvasHub` anchors and sample UI.

## How to Add a New Screen
1. Create a `MyView : MonoBehaviour, IView`.
2. Create a `MyPresenter : BasePresenter` (ctor takes `IView`).
3. Register the prefab in `UiManager` catalog and add to `PresenterFactory`.
4. Trigger from anywhere: `bus.Publish(new ShowView ( ScreenId.MyScreen ));`

## Scene Loading by Events
```csharp
bus.Publish(new LoadSceneRequest(
  scene: "Gameplay",
  additive: false,
  activateOnLoad: true,
  cancelPrevious: true
));
```

## Audio
```csharp
var audio = ServiceLocator.Instance.Get<IAudioManager>();
audio.SetMusicVolume(0.8f);
audio.PlayMusic(introClip, loop:true, fadeSeconds:0.5f);
audio.PlaySfx(clickClip);
```

## Demo Features

The included `MainMenu` scene demonstrates:
- **Main Menu Screen** (`MainMenuScreenView` / `MainMenuScreenPresenter`)
  - "Settings" button opens a popup.
  - "Go Ingame" button simulates scene transition.
- **Settings Popup** (`SettingsScreenView` / `SettingsScreenPresenter`)
  - Music and SFX volume sliders connected to `AudioManager`.
  - Changes persist in real time.
  
This provides a working example of:
- How the UI Manager handles screen/popup instantiation via events.
- Using MVP to decouple view logic from presentation.
- Integrating `AudioManager` controls into UI.