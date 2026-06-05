# Ash Defender Architecture

Version: 1.0

Engine: Unity 6

Architecture Style: Clean Architecture + Feature-Based Architecture + Event-Driven Architecture

---

# 1. Overview

Ash Defender is a 2D Hero Defense game where players defend the Last Flame of humanity against waves of monsters from the Abyss.

Unlike traditional Tower Defense games, players do not place static towers. Instead, they deploy combat units and heroes onto the battlefield while managing resources, commander skills, and strategic positioning.

The architecture is designed to:

* Separate business logic from Unity-specific code
* Support scalability as new heroes, enemies, stages, and skills are added
* Improve maintainability and testability
* Minimize dependencies between systems
* Support future features such as Endless Mode, PvP, Multiplayer, and Live Content Updates

---

# 2. Architectural Principles

## Clean Architecture

Business rules should not depend on Unity.

```text
Presentation
      ↓
Application
      ↓
Domain

Infrastructure
      ↓
Application
      ↓
Domain
```

### Dependency Rules

Presentation can reference:

* Application
* Domain

Infrastructure can reference:

* Application
* Domain

Application can reference:

* Domain

Domain references nothing.

---

## Feature-Based Organization

Code is organized by gameplay features instead of technical layers.

Bad:

```text
Scripts
├── Managers
├── Controllers
├── Services
├── Models
```

Good:

```text
Features
├── Hero
├── Enemy
├── Combat
├── Wave
├── Upgrade
```

---

## Event-Driven Communication

Features communicate through events.

Avoid:

```text
Hero → Enemy → WaveManager
```

Prefer:

```text
Hero
 ↓
EnemyKilledEvent
 ↓
EventBus
 ↓
WaveSystem
```

---

# 3. High-Level Architecture

```text
┌───────────────────────┐
│    Presentation       │
└──────────┬────────────┘
           │
┌──────────▼────────────┐
│     Application       │
└──────────┬────────────┘
           │
┌──────────▼────────────┐
│        Domain         │
└──────────▲────────────┘
           │
┌──────────┴────────────┐
│    Infrastructure     │
└───────────────────────┘
```

---

# 4. Solution Structure

```text
Assets
│
├── _Project
│
├── Features
│
├── Shared
│
├── Art
├── Audio
├── Animations
├── Materials
├── Prefabs
├── Scenes
├── UI
├── VFX
├── ScriptableObjects
└── Addressables
```

---

# 5. Features Structure

```text
Features
│
├── Hero
├── Unit
├── Deployment
├── Enemy
├── Wave
├── Combat
├── CommanderSkill
├── CoreDefense
├── Reward
├── Upgrade
├── Stage
├── SaveSystem
└── MainMenu
```

Each feature follows:

```text
FeatureName
│
├── Domain
├── Application
├── Infrastructure
└── Presentation
```

---

# 6. Domain Layer

Contains pure business logic.

Must NOT use:

* MonoBehaviour
* Transform
* GameObject
* Rigidbody2D
* Animator
* Time
* Debug

---

## Domain Structure

```text
Domain
│
├── Entities
├── ValueObjects
├── Enums
├── Events
├── Specifications
└── Interfaces
```

---

## Core Entities

### Hero

Represents a player-controlled hero.

Properties:

* Id
* Name
* Level
* Health
* Attack
* Defense
* SkillSet

---

### Unit

Represents deployed soldiers.

Properties:

* UnitType
* Cost
* Health
* Attack
* MoveSpeed

---

### Enemy

Represents enemies.

Properties:

* EnemyType
* Health
* Attack
* MoveSpeed
* Reward

---

### Boss

Special enemy.

Properties:

* Phase
* SpecialAbilities
* EnrageState

---

### Core

Represents player's base.

Properties:

* MaxHP
* CurrentHP

---

# 7. Application Layer

Coordinates game rules.

Contains:

```text
Application
│
├── UseCases
├── Commands
├── Queries
├── DTOs
├── Validators
└── Interfaces
```

---

## Use Cases

### DeployUnitUseCase

Deploy a selected unit.

---

### StartWaveUseCase

Start enemy wave.

---

### CompleteStageUseCase

Finish stage.

---

### UpgradeHeroUseCase

Upgrade hero.

---

### CastCommanderSkillUseCase

Cast commander skill.

---

### GrantRewardUseCase

Distribute rewards.

---

### SaveProgressUseCase

Save player progress.

---

# 8. Infrastructure Layer

Responsible for external implementations.

```text
Infrastructure
│
├── Persistence
├── Repositories
├── Audio
├── SaveSystem
├── Addressables
├── Localization
└── Services
```

---

## Repositories

### HeroRepository

Stores hero data.

### UnitRepository

Stores unit data.

### StageRepository

Stores stage information.

---

## Save Services

```text
JsonSaveService

CloudSaveService

EncryptedSaveService
```

---

## Audio Services

```text
MusicService

SFXService
```

---

# 9. Presentation Layer

Unity-specific code.

Contains:

```text
Presentation
│
├── Gameplay
├── UI
├── Animation
├── Input
├── Camera
└── SceneManagement
```

Allowed:

* MonoBehaviour
* Rigidbody2D
* Collider2D
* Animator
* Input System

---

# 10. Gameplay Flow

```text
Main Menu
     ↓
Squad Selection
     ↓
Battle Stage
     ↓
Victory
     ↓
Rewards
     ↓
Upgrade
     ↓
Next Stage
```

---

# 11. Squad Selection System

Players select up to:

```text
4 Units
```

before entering battle.

Responsibilities:

* Team composition
* Unit loadout
* Hero selection

---

# 12. Deployment System

Players spend Energy to deploy units.

Flow:

```text
Energy Available
      ↓
Select Unit
      ↓
Deploy Unit
      ↓
Unit Joins Battlefield
```

---

# 13. Combat System

Handles:

* Damage Calculation
* Attack Speed
* Critical Hits
* Status Effects
* Skill Damage

---

## Combat Flow

```text
Find Target
      ↓
Move To Target
      ↓
Attack
      ↓
Deal Damage
      ↓
Check Death
```

---

# 14. Hero System

Hero Features:

* Auto Target
* Auto Movement
* Auto Attack
* Active Skills
* Upgrade Progression

---

## Hero State Machine

```text
Idle
 ↓
Move
 ↓
Attack
 ↓
CastSkill
 ↓
Dead
```

---

# 15. Unit System

Unit Features:

* Melee Units
* Ranged Units
* Tank Units
* Support Units

---

## Unit State Machine

```text
Idle
 ↓
Move
 ↓
Attack
 ↓
Dead
```

---

# 16. Enemy System

Enemy Types:

### Map 1

* Slime
* Goblin

### Map 2

* Spider
* Mushroom Monster

### Map 3

* Skeleton
* Bat
* Goblin Warrior

### Map 4

* Ice Slime
* Ice Goblin
* Snow Wolf

### Map 5

* Demon Slime
* Dark Knight
* Fire Bat

---

# 17. Boss System

Bosses:

* Giant Slime
* Giant Spider
* Skeleton King
* Ice Golem
* Demon Lord

Boss behavior:

```text
Phase 1
 ↓
Phase 2
 ↓
Enrage
```

---

# 18. Wave System

Wave Manager responsibilities:

* Spawn enemies
* Control wave timing
* Trigger boss spawn
* Notify stage completion

---

## Wave Flow

```text
Start Stage
      ↓
Wave 1
      ↓
Wave 2
      ↓
Wave 3
      ↓
Boss Spawn
      ↓
Stage Complete
```

---

# 19. Commander Skill System

Commander Skills:

* Heal
* Buff
* AoE Damage
* Crowd Control

Resources:

```text
Mana
```

Required for skill activation.

---

# 20. Reward System

Rewards:

* Gold Coin
* Soul Fragment
* Ancient Scroll
* Health Potion
* Mana Crystal
* Revival Feather

---

# 21. Upgrade System

Upgradeable:

### Hero

* HP
* Attack
* Defense

### Unit

* Damage
* Attack Speed
* Health

### Commander

* Skill Power
* Mana Efficiency

---

# 22. Stage Progression System

```text
Stage 1
 ↓
Stage 2
 ↓
Stage 3
 ↓
Stage 4
 ↓
Stage 5
```

Unlock rule:

```text
Previous Stage Completed
```

---

# 23. Event Bus

Global communication system.

---

## Events

```text
UnitDeployedEvent

HeroSpawnedEvent

EnemySpawnedEvent

EnemyKilledEvent

BossSpawnedEvent

BossDefeatedEvent

CoreDamagedEvent

StageStartedEvent

StageCompletedEvent

RewardGrantedEvent

HeroUpgradedEvent

SkillUnlockedEvent

GameSavedEvent
```

---

# 24. Save System

Saved Data:

```text
Player Progress

Unlocked Heroes

Hero Levels

Unit Levels

Commander Skills

Completed Stages

Inventory

Currencies

Settings
```

---

# 25. Scene Architecture

```text
Bootstrap
     ↓
MainMenu
     ↓
SquadSelection
     ↓
Gameplay
     ↓
Victory
     ↓
Upgrade
```

---

# 26. Bootstrap Scene

Contains:

```text
VContainer

EventBus

SaveManager

AudioManager

SceneLoader
```

Objects:

```text
DontDestroyOnLoad
```

Only for:

* GameManager
* SaveManager
* AudioManager

---

# 27. Dependency Injection

Framework:

```text
VContainer
```

---

## Registrations

```text
Repositories

UseCases

Services

EventBus

SaveSystem
```

---

# 28. ScriptableObject Strategy

Used for:

### Hero Data

```text
HeroConfigSO
```

### Unit Data

```text
UnitConfigSO
```

### Enemy Data

```text
EnemyConfigSO
```

### Skill Data

```text
SkillConfigSO
```

### Stage Data

```text
StageConfigSO
```

---

# 29. Shared Module

```text
Shared
│
├── Core
├── Events
├── Constants
├── Extensions
├── Utilities
├── Exceptions
└── Configurations
```

---

# 30. Recommended Technology Stack

Engine:

* Unity 6

Language:

* C#

DI:

* VContainer

Async:

* UniTask

Asset Management:

* Addressables

Input:

* Unity Input System

UI:

* UI Toolkit

Persistence:

* JSON Save

Version Control:

* Git
* GitHub

---

# 31. Future Expansion

Potential future systems:

* Endless Mode
* Daily Challenges
* Equipment System
* Hero Talent Tree
* Guild System
* PvP Arena
* Multiplayer Co-op
* Cloud Save
* Leaderboards
* Seasonal Events

---

# 32. Summary

Ash Defender follows a Feature-Based Clean Architecture where:

* Domain contains business rules.
* Application coordinates gameplay use cases.
* Infrastructure handles external systems.
* Presentation contains Unity-specific implementations.

This structure ensures scalability, maintainability, and extensibility as the game grows beyond its initial 5-stage Hero Defense experience.
