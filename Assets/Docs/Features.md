# Features

Version: 1.0

---

## Overview

The `Features/` folder contains all gameplay modules organized by **Feature-Based Architecture** (see `ARCHITECTURE.md` §5). Each feature is self-contained and follows **Clean Architecture** layers:

```
FeatureName/
├── Domain/          — Pure business logic, no Unity dependency
├── Application/     — Use cases, commands, DTOs, validators
├── Infrastructure/  — Repositories, services (Unity/external implementations)
└── Presentation/    — MonoBehaviours, UI panels, animation controllers
```

Features communicate **only through the EventBus** (`Shared/Core/EventBus.cs`), never by direct reference to each other.

---

## Bootstrap

**File:** `Bootstrap/BootstrapLifetimeScope.cs`

Wires the entire dependency graph via **VContainer** at game startup. Registers all repositories, use cases, services, the EventBus, and the SaveManager so they are available to every other feature via injection.

Runs in the **Bootstrap scene** (first scene loaded). Its registered objects (GameManager, SaveManager, AudioManager) are marked `DontDestroyOnLoad` and persist for the whole session.

**Called by:** Unity engine on Bootstrap scene load — nothing calls it directly.  
**Calls into:** All features (registers their dependencies), `Shared/Core/EventBus.cs`, `Shared/Core/SceneLoader.cs`.

---

## Combat

**Files:** `Combat/Domain/`, `Combat/Application/`, `Combat/Infrastructure/Services/CombatService.cs`, `Combat/Presentation/Gameplay/CombatHandler.cs`

Handles damage calculation, attack speed, critical hits, status effects, and skill damage (see `ARCHITECTURE.md` §13).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `ICombatService.cs`, `DamageResult.cs` | Define combat contract and result value object |
| Application | `ProcessAttackUseCase.cs` | Orchestrate an attack between attacker and target |
| Application | `AttackDTO.cs`, `DamageResultDTO.cs` | Transfer data across layer boundaries |
| Infrastructure | `CombatService.cs` | Implement damage formula |
| Presentation | `CombatHandler.cs` | Drive combat loop from Unity MonoBehaviour |

**Called by:** `Hero/Presentation/Gameplay/HeroController.cs`, `Unit/Presentation/Gameplay/UnitController.cs`, and `Enemy/Presentation/Gameplay/EnemyController.cs` during their Attack states.  
**Publishes:** `EnemyKilledEvent` (via EventBus when target HP reaches 0).

---

## CommanderSkill

**Files:** `CommanderSkill/Domain/`, `CommanderSkill/Application/`, `CommanderSkill/Infrastructure/`, `CommanderSkill/Presentation/UI/CommanderSkillPanel.cs`

Manages commander skills (Heal, Buff, AoE Damage, Crowd Control) that cost **Mana** to cast (see `ARCHITECTURE.md` §19).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `CommanderSkill.cs` | Skill entity (type, cost, cooldown) |
| Domain | `SkillType.cs` | Enum of all skill types |
| Application | `CastCommanderSkillUseCase.cs` | Validate mana, apply skill effect |
| Application | `CommanderSkillDTO.cs` | Data transfer for skill activation |
| Infrastructure | `CommanderSkillRepository.cs` | Load skill configs from ScriptableObjects |
| Presentation | `CommanderSkillPanel.cs` | Player input → triggers use case |

**Called by:** Player tapping a skill button in `CommanderSkillPanel.cs` → `CastCommanderSkillUseCase`.  
**Publishes:** `SkillUnlockedEvent` (when a new skill is unlocked via Upgrade).

---

## CoreDefense

**Files:** `CoreDefense/Domain/Entities/Core.cs`, `CoreDefense/Presentation/Gameplay/CoreController.cs`, `CoreDefense/Presentation/UI/CoreHealthUI.cs`

Represents the **Last Flame** — the player's base. Tracks `MaxHP` and `CurrentHP` (see `ARCHITECTURE.md` §6 Core entity).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `Core.cs` | Core entity with HP state |
| Application | `CoreDTO.cs`, `ICoreService.cs` | Service contract and data transfer |
| Infrastructure | `CoreRepository.cs` | Persist/load core state |
| Presentation | `CoreController.cs` | Receive damage, check game-over condition |
| Presentation | `CoreHealthUI.cs` | Display HP bar |

**Called by:** `Enemy/Presentation/Gameplay/EnemyController.cs` when an enemy reaches the core.  
**Publishes:** `CoreDamagedEvent` → consumed by `CoreHealthUI.cs` to update the HP display and by game-over logic.

---

## Deployment

**Files:** `Deployment/Application/UseCases/DeployUnitUseCase.cs`, `Deployment/Presentation/Input/DeploymentInputHandler.cs`, `Deployment/Presentation/UI/DeploymentPanel.cs`

Handles player spending **Energy** to place units onto the battlefield (see `ARCHITECTURE.md` §12).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `IDeploymentService.cs` | Deployment contract |
| Application | `DeployUnitCommand.cs` | Command object carrying deployment request |
| Application | `DeployUnitUseCase.cs` | Validate energy, call deployment service |
| Application | `DeploymentValidator.cs` | Guard against invalid placement |
| Application | `DeploymentRequestDTO.cs` | Input data transfer object |
| Infrastructure | `DeploymentService.cs` | Spawn unit prefab at target position |
| Presentation | `DeploymentInputHandler.cs` | Detect player tap/click on battlefield |
| Presentation | `DeploymentPanel.cs` | Show available units and energy cost |

**Called by:** Player input in `DeploymentInputHandler.cs` + `DeploymentPanel.cs` → `DeployUnitUseCase`.  
**Publishes:** `UnitDeployedEvent` → consumed by `Wave` to track units on field.  
**Throws:** `InsufficientResourcesException`, `InvalidDeploymentException` (defined in `Shared/Exceptions/`).

---

## Enemy

**Files:** `Enemy/Domain/Entities/Enemy.cs`, `Enemy/Domain/Entities/Boss.cs`, `Enemy/Presentation/Gameplay/EnemyController.cs`, `Enemy/Presentation/Gameplay/BossController.cs`, `Enemy/Presentation/Animation/EnemyAnimationController.cs`

Defines enemy and boss entities with their state machines (Idle → Move → Attack → Dead) (see `ARCHITECTURE.md` §16–17).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `Enemy.cs` | Enemy entity (type, HP, attack, speed, reward) |
| Domain | `Boss.cs` | Boss entity with Phase, SpecialAbilities, EnrageState |
| Domain | `EnemyType.cs`, `BossPhase.cs`, `EnemyState.cs` | Enums for classification and state |
| Application | `IEnemyService.cs`, `EnemyDTO.cs`, `BossDTO.cs` | Service contract and data transfer |
| Infrastructure | `EnemyRepository.cs` | Load enemy configs from ScriptableObjects |
| Presentation | `EnemyController.cs` | Drive enemy AI state machine in Unity |
| Presentation | `BossController.cs` | Drive boss phase transitions and special abilities |
| Presentation | `EnemyAnimationController.cs` | Sync Animator with current enemy state |

**Called by:** `Wave/Presentation/Gameplay/WaveManager.cs` spawns enemies.  
**Publishes:** `EnemySpawnedEvent`, `EnemyKilledEvent`, `BossSpawnedEvent`, `BossDefeatedEvent`.  
**Calls into:** `Combat` feature via `ProcessAttackUseCase` when attacking heroes/units.

---

## Hero

**Files:** `Hero/Domain/Entities/Hero.cs`, `Hero/Presentation/Gameplay/HeroController.cs`, `Hero/Presentation/Animation/HeroAnimationController.cs`, `Hero/Presentation/UI/HeroStatusUI.cs`

Player-controlled hero with auto-target, auto-movement, auto-attack, and active skills (see `ARCHITECTURE.md` §14).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `Hero.cs` | Hero entity (Id, Level, HP, Attack, Defense, SkillSet) |
| Domain | `HeroStats.cs` | Value object for hero stat bundle |
| Domain | `HeroState.cs` | Enum: Idle, Move, Attack, CastSkill, Dead |
| Application | `SelectHeroUseCase.cs` | Pick hero for current squad |
| Application | `ActivateSkillUseCase.cs` | Trigger hero active skill |
| Application | `IHeroService.cs`, `HeroDTO.cs` | Service contract and data transfer |
| Infrastructure | `HeroRepository.cs` | Load hero configs from `HeroConfigSO` |
| Presentation | `HeroController.cs` | Drive hero state machine in Unity |
| Presentation | `HeroAnimationController.cs` | Sync Animator with hero state |
| Presentation | `HeroStatusUI.cs` | Show hero HP and skill cooldowns |

**Called by:** Stage setup spawns the hero → `HeroController` runs the state machine. Squad selection calls `SelectHeroUseCase`.  
**Publishes:** `HeroSpawnedEvent`.  
**Subscribes to:** `HeroUpgradedEvent` (from Upgrade feature) to refresh stats.

---

## MainMenu

**Files:** `MainMenu/Presentation/SceneManagement/MainMenuSceneController.cs`, `MainMenu/Presentation/UI/MainMenuPanel.cs`, `MainMenu/Presentation/UI/SettingsPanel.cs`

Entry point of the game after Bootstrap. Shows the main menu UI and navigates to Squad Selection (see `ARCHITECTURE.md` §25 Scene Architecture).

| Layer | Key File | Responsibility |
|---|---|---|
| Application | `MainMenuDTO.cs` | Data for menu state |
| Presentation | `MainMenuSceneController.cs` | Control scene flow (Play, Settings, Quit) |
| Presentation | `MainMenuPanel.cs` | Main menu buttons and layout |
| Presentation | `SettingsPanel.cs` | Audio/graphics settings UI |

**Called by:** `Shared/Core/SceneLoader.cs` transitions from Bootstrap → MainMenu scene.  
**Calls into:** `Shared/Core/SceneLoader.cs` to navigate to SquadSelection scene.

---

## Reward

**Files:** `Reward/Application/UseCases/GrantRewardUseCase.cs`, `Reward/Presentation/UI/RewardPanel.cs`

Distributes post-battle rewards: Gold Coin, Soul Fragment, Ancient Scroll, Health Potion, Mana Crystal, Revival Feather (see `ARCHITECTURE.md` §20).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `RewardItem.cs` | Value object for a single reward |
| Domain | `RewardType.cs` | Enum of reward types |
| Application | `GrantRewardUseCase.cs` | Calculate and grant rewards after stage |
| Application | `RewardDTO.cs` | Transfer reward data to presentation |
| Infrastructure | `RewardService.cs` | Apply rewards to player inventory/currency |
| Presentation | `RewardPanel.cs` | Show reward summary to player |

**Called by:** `StageCompletedEvent` (from EventBus) triggers `GrantRewardUseCase`.  
**Publishes:** `RewardGrantedEvent` → consumed by `SaveSystem` to persist updated inventory.

---

## SaveSystem

**Files:** `SaveSystem/Infrastructure/SaveSystem/JsonSaveService.cs`, `CloudSaveService.cs`, `EncryptedSaveService.cs`, `SaveSystem/Application/UseCases/SaveProgressUseCase.cs`

Persists and loads all player progress (see `ARCHITECTURE.md` §24).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `SaveData.cs` | Entity holding all saveable state |
| Domain | `ISaveService.cs` | Save/load contract |
| Application | `SaveProgressUseCase.cs` | Orchestrate save operation |
| Application | `SaveDataDTO.cs` | Flatten domain state for serialization |
| Infrastructure | `JsonSaveService.cs` | Save to local JSON file |
| Infrastructure | `CloudSaveService.cs` | Save to cloud (future) |
| Infrastructure | `EncryptedSaveService.cs` | Encrypted local save |
| Presentation | `SaveIndicatorUI.cs` | Show save spinner/checkmark |

**Called by:** `GameSavedEvent` (published on stage complete, upgrade, etc.) triggers `SaveProgressUseCase`. Loaded at Bootstrap scene to restore player state.  
**Publishes:** `GameSavedEvent`.

---

## Stage

**Files:** `Stage/Application/UseCases/StartStageUseCase.cs`, `Stage/Application/UseCases/CompleteStageUseCase.cs`, `Stage/Presentation/SceneManagement/StageSceneController.cs`, `Stage/Presentation/UI/StageSelectPanel.cs`

Controls stage progression across 5 stages with unlock rules (see `ARCHITECTURE.md` §22).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `Stage.cs` | Stage entity (id, waves, enemies, boss) |
| Domain | `StageState.cs` | Enum: Locked, Unlocked, Completed |
| Application | `StartStageUseCase.cs` | Validate unlock and begin stage |
| Application | `CompleteStageUseCase.cs` | Mark stage complete, unlock next |
| Application | `IStageService.cs`, `StageDTO.cs` | Service contract and data transfer |
| Infrastructure | `StageRepository.cs` | Load stage configs from `StageConfigSO` |
| Presentation | `StageSceneController.cs` | Orchestrate gameplay scene lifecycle |
| Presentation | `StageSelectPanel.cs` | Show stage map and unlock state |

**Called by:** Player selects stage in `StageSelectPanel.cs` → `StartStageUseCase`.  
**Publishes:** `StageStartedEvent`, `StageCompletedEvent`.  
**Subscribes to:** `BossDefeatedEvent` to trigger stage completion flow.

---

## Unit

**Files:** `Unit/Domain/Entities/Unit.cs`, `Unit/Presentation/Gameplay/UnitController.cs`, `Unit/Presentation/Animation/UnitAnimationController.cs`

Deployed soldier units — Melee, Ranged, Tank, Support — with state machine (Idle → Move → Attack → Dead) (see `ARCHITECTURE.md` §15).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `Unit.cs` | Unit entity (type, cost, HP, attack, speed) |
| Domain | `UnitType.cs`, `UnitState.cs` | Enum for unit classification and state |
| Application | `IUnitService.cs`, `UnitDTO.cs` | Service contract and data transfer |
| Infrastructure | `UnitRepository.cs` | Load unit configs from `UnitConfigSO` |
| Presentation | `UnitController.cs` | Drive unit AI state machine in Unity |
| Presentation | `UnitAnimationController.cs` | Sync Animator with unit state |

**Called by:** `Deployment/Infrastructure/Services/DeploymentService.cs` spawns `UnitController` prefabs.  
**Calls into:** `Combat` feature via `ProcessAttackUseCase` when attacking enemies.

---

## Upgrade

**Files:** `Upgrade/Application/UseCases/UpgradeHeroUseCase.cs`, `UpgradeUnitUseCase.cs`, `UpgradeCommanderUseCase.cs`, `Upgrade/Presentation/UI/UpgradePanel.cs`

Upgrades Hero (HP, Attack, Defense), Unit (Damage, Speed, Health), and Commander (Skill Power, Mana Efficiency) attributes using earned resources (see `ARCHITECTURE.md` §21).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `UpgradeTarget.cs` | Enum: Hero, Unit, Commander |
| Domain | `IUpgradeService.cs` | Upgrade contract |
| Application | `UpgradeHeroUseCase.cs` | Apply hero stat upgrade |
| Application | `UpgradeUnitUseCase.cs` | Apply unit stat upgrade |
| Application | `UpgradeCommanderUseCase.cs` | Apply commander skill upgrade |
| Application | `UpgradeValidator.cs` | Check resource availability before upgrade |
| Application | `UpgradeDTO.cs` | Transfer upgrade request data |
| Infrastructure | `UpgradeService.cs` | Apply stat delta to entity |
| Presentation | `UpgradePanel.cs` | Show upgrade tree, costs, confirm action |

**Called by:** Player in the **Upgrade scene** (after Victory) interacts with `UpgradePanel.cs` → respective use case.  
**Publishes:** `HeroUpgradedEvent`, `SkillUnlockedEvent`.  
**Throws:** `InsufficientResourcesException` (defined in `Shared/Exceptions/`) when player lacks resources.

---

## Wave

**Files:** `Wave/Application/UseCases/StartWaveUseCase.cs`, `Wave/Presentation/Gameplay/WaveManager.cs`, `Wave/Presentation/UI/WaveProgressUI.cs`

Spawns enemy waves, controls timing, triggers boss spawn, and notifies stage completion (see `ARCHITECTURE.md` §18).

| Layer | Key File | Responsibility |
|---|---|---|
| Domain | `Wave.cs` | Wave entity (wave number, enemy list, timing) |
| Domain | `IWaveRepository.cs` | Wave data contract |
| Application | `StartWaveUseCase.cs` | Begin next wave sequence |
| Application | `IWaveService.cs`, `WaveDTO.cs` | Service contract and data transfer |
| Infrastructure | `WaveRepository.cs` | Load wave config from `StageConfigSO` |
| Presentation | `WaveManager.cs` | Spawn enemies on timer, detect wave clear |
| Presentation | `WaveProgressUI.cs` | Display current wave number and remaining enemies |

**Called by:** `StageStartedEvent` → `StartWaveUseCase` → `WaveManager` begins spawning.  
**Calls into:** `Enemy` feature to instantiate enemy controllers.  
**Publishes:** `StageCompletedEvent` (after final boss wave is cleared).  
**Subscribes to:** `EnemyKilledEvent`, `BossDefeatedEvent` to track wave completion.
