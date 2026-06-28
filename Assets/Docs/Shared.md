# Shared

Version: 1.0

---

## Overview

The `Shared/` folder contains cross-cutting infrastructure used by **every feature** (see `ARCHITECTURE.md` §29). Nothing here belongs to a single feature — if code would otherwise be duplicated or if features need a common channel to communicate, it lives here.

```
Shared/
├── Core/            — EventBus and SceneLoader (global singletons)
├── Events/          — All cross-feature game events
├── Constants/       — Static identifiers shared across layers
├── Configurations/  — Global game config values
├── Extensions/      — C# extension methods
├── Utilities/       — Stateless helper classes
└── Exceptions/      — Domain exceptions thrown across features
```

Shared code has **no dependency on any Feature**. Features depend on Shared, never the reverse.

---

## Core

**Files:** `Shared/Core/IGameEvent.cs`, `Shared/Core/IEventBus.cs`, `Shared/Core/EventBus.cs`, `Shared/Core/SceneLoader.cs`

The foundation layer that enables decoupled communication and scene management across the entire game (see `ARCHITECTURE.md` §23 Event Bus and §26 Bootstrap Scene).

### IGameEvent

Marker interface that all events must implement. Enforces type safety on the event bus — only `IGameEvent` implementors can be published or subscribed.

**Used by:** Every file in `Shared/Events/` implements this interface.

### IEventBus

Contract for the event bus. Features depend on this interface (not the concrete class) so that the implementation can be swapped or mocked in tests.

**Used by:** Every feature that publishes or subscribes to events injects `IEventBus`.

### EventBus

Concrete global event dispatcher. Registered as a singleton in `Bootstrap/BootstrapLifetimeScope.cs` and kept alive via `DontDestroyOnLoad`.

**Registered by:** `Bootstrap/BootstrapLifetimeScope.cs` at game startup.  
**Called by (publish):**
- `Deployment` → `UnitDeployedEvent`
- `Hero` → `HeroSpawnedEvent`
- `Enemy` → `EnemySpawnedEvent`, `EnemyKilledEvent`, `BossSpawnedEvent`, `BossDefeatedEvent`
- `CoreDefense` → `CoreDamagedEvent`
- `Stage` → `StageStartedEvent`, `StageCompletedEvent`
- `Reward` → `RewardGrantedEvent`
- `Upgrade` → `HeroUpgradedEvent`, `SkillUnlockedEvent`
- `SaveSystem` → `GameSavedEvent`

**Subscribed by (consume):**
- `Wave/Presentation/Gameplay/WaveManager.cs` ← `EnemyKilledEvent`, `BossDefeatedEvent`, `StageStartedEvent`
- `CoreDefense/Presentation/UI/CoreHealthUI.cs` ← `CoreDamagedEvent`
- `Reward` ← `StageCompletedEvent`
- `Hero` ← `HeroUpgradedEvent`
- `SaveSystem` ← `GameSavedEvent`
- `Wave/Presentation/UI/WaveProgressUI.cs` ← `EnemyKilledEvent`

### SceneLoader

Handles async scene transitions across the game's scene flow: Bootstrap → MainMenu → SquadSelection → Gameplay → Victory → Upgrade (see `ARCHITECTURE.md` §25).

**Registered by:** `Bootstrap/BootstrapLifetimeScope.cs`.  
**Called by:**
- `MainMenu/Presentation/SceneManagement/MainMenuSceneController.cs` → navigate to SquadSelection
- `Stage/Presentation/SceneManagement/StageSceneController.cs` → navigate to Victory or reload

---

## Events

**Files:** `Shared/Events/*.cs` (13 event classes)

All domain events that cross feature boundaries. Each event is a plain C# class implementing `IGameEvent` — no Unity dependency.

| Event | Published by | Consumed by |
|---|---|---|
| `UnitDeployedEvent` | Deployment | Wave |
| `HeroSpawnedEvent` | Hero | (UI / future systems) |
| `EnemySpawnedEvent` | Enemy | Wave, WaveProgressUI |
| `EnemyKilledEvent` | Enemy (via Combat) | Wave, WaveProgressUI, Reward |
| `BossSpawnedEvent` | Wave | (UI / future systems) |
| `BossDefeatedEvent` | Enemy | Wave, Stage |
| `CoreDamagedEvent` | CoreDefense | CoreHealthUI, game-over logic |
| `StageStartedEvent` | Stage | Wave |
| `StageCompletedEvent` | Wave | Reward, SaveSystem |
| `RewardGrantedEvent` | Reward | SaveSystem |
| `HeroUpgradedEvent` | Upgrade | Hero |
| `SkillUnlockedEvent` | Upgrade | CommanderSkill |
| `GameSavedEvent` | SaveSystem | SaveIndicatorUI |

**How to add a new event:**
1. Create a class in `Shared/Events/` implementing `IGameEvent`.
2. Publish via `IEventBus` from the source feature.
3. Subscribe via `IEventBus` in the consuming feature.

---

## Constants

**Files:** `Shared/Constants/GameConstants.cs`, `Shared/Constants/LayerConstants.cs`, `Shared/Constants/TagConstants.cs`

Static readonly values that would otherwise be magic strings or numbers scattered across features.

### GameConstants

Game-wide numeric and string constants (energy costs, HP caps, wave timing defaults, etc.).

**Used by:** Application layer validators (`DeploymentValidator.cs`, `UpgradeValidator.cs`) and domain entities.

### LayerConstants

Unity physics layer integer IDs (e.g., `Enemy`, `Hero`, `Projectile`).

**Used by:** `Hero/Presentation/Gameplay/HeroController.cs`, `Enemy/Presentation/Gameplay/EnemyController.cs`, and `Combat/Presentation/Gameplay/CombatHandler.cs` for `Physics2D` layer masks.

### TagConstants

Unity `GameObject.tag` string identifiers.

**Used by:** Presentation layer controllers that locate GameObjects by tag at runtime.

---

## Configurations

**File:** `Shared/Configurations/GameConfig.cs`

A single configuration object holding global game settings (debug flags, difficulty multipliers, feature toggles, etc.) that do not belong to any one feature.

**Registered by:** `Bootstrap/BootstrapLifetimeScope.cs` as a singleton (injected via VContainer).  
**Used by:** Any use case or service that needs game-wide configuration values.

---

## Extensions

**Files:** `Shared/Extensions/ListExtensions.cs`, `Shared/Extensions/StringExtensions.cs`, `Shared/Extensions/Vector2Extensions.cs`

C# extension methods that extend built-in or Unity types. These eliminate repeated utility code across feature layers.

### ListExtensions

Helpers on `List<T>` (e.g., shuffle, random pick).

**Used by:** `Wave/Presentation/Gameplay/WaveManager.cs` (randomize enemy spawn order), `Reward` (pick random reward items).

### StringExtensions

String formatting or parsing helpers.

**Used by:** `SaveSystem` (data serialization) and UI panels for display formatting.

### Vector2Extensions

Math helpers on `Vector2` (e.g., direction clamping, distance checks).

**Used by:** `Hero/Presentation/Gameplay/HeroController.cs`, `Unit/Presentation/Gameplay/UnitController.cs`, `Enemy/Presentation/Gameplay/EnemyController.cs` for movement and targeting calculations.

---

## Utilities

**Files:** `Shared/Utilities/MathUtility.cs`, `Shared/Utilities/RandomUtility.cs`

Stateless helper classes with no Unity dependency.

### MathUtility

Shared math functions used in game logic (e.g., percentage damage reduction, clamping).

**Used by:** `Combat/Infrastructure/Services/CombatService.cs` for damage formula, `Upgrade/Infrastructure/Services/UpgradeService.cs` for stat scaling.

### RandomUtility

Deterministic or seeded random wrapper.

**Used by:** `Wave/Presentation/Gameplay/WaveManager.cs` (enemy type selection per spawn), `Reward/Infrastructure/Services/RewardService.cs` (loot roll).

---

## Exceptions

**Files:** `Shared/Exceptions/GameException.cs`, `Shared/Exceptions/InsufficientResourcesException.cs`, `Shared/Exceptions/InvalidDeploymentException.cs`

Domain exceptions that can be thrown from any feature and caught in the presentation or application layer.

### GameException

Base exception class for all game-specific errors. All other custom exceptions inherit from it.

**Caught by:** Top-level error handlers in Presentation layer controllers.

### InsufficientResourcesException

Thrown when a player action requires more Energy, Mana, or Gold than available.

**Thrown by:** `Deployment/Application/Validators/DeploymentValidator.cs`, `Upgrade/Application/Validators/UpgradeValidator.cs`, `CommanderSkill/Application/UseCases/CastCommanderSkillUseCase.cs`.  
**Caught by:** `DeploymentPanel.cs`, `UpgradePanel.cs`, `CommanderSkillPanel.cs` to show error feedback to the player.

### InvalidDeploymentException

Thrown when a unit is placed at a disallowed position on the battlefield.

**Thrown by:** `Deployment/Application/Validators/DeploymentValidator.cs`.  
**Caught by:** `Deployment/Presentation/Input/DeploymentInputHandler.cs` to block the action and show feedback.
