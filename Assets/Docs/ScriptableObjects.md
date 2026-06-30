# ScriptableObjects

Version: 1.0

---

## Overview

The `ScriptableObjects/` folder contains all **data configuration classes** for the game (see `ARCHITECTURE.md` §28). Each class extends Unity's `ScriptableObject`, meaning designers create `.asset` instances of them directly in the Unity Editor — no code change required to add a new hero, enemy, or stage.

```
ScriptableObjects/
├── Enemy/     — EnemyConfigSO.cs
├── Hero/      — HeroConfigSO.cs
├── Skill/     — SkillConfigSO.cs
├── Stage/     — StageConfigSO.cs
└── Unit/      — UnitConfigSO.cs
```

These are **pure data containers** — no game logic lives here. Logic lives in the `Features/` layer; these SOs are loaded by Infrastructure repositories and injected into the Application layer via use cases.

---

## EnemyConfigSO

**File:** `ScriptableObjects/Enemy/EnemyConfigSO.cs`  
**Menu path:** `AshDefender/ScriptableObjects/EnemyConfigSO`

Holds static configuration for a single enemy type. Also defines the `EnemyType` enum covering all 13 enemy variants across 5 maps (see `ARCHITECTURE.md` §16).

### Fields

| Group | Field | Type | Description |
|---|---|---|---|
| Identity | `enemyId` | `string` | Unique identifier |
| Identity | `enemyName` | `string` | Display name |
| Identity | `icon` | `Sprite` | Portrait sprite |
| Identity | `enemyType` | `EnemyType` | Slime, Goblin, Spider, MushroomMonster, Skeleton, Bat, GoblinWarrior, IceSlime, IceGoblin, SnowWolf, DemonSlime, DarkKnight, FireBat |
| Base Stats | `health` | `int` | Max HP |
| Base Stats | `attack` | `int` | Attack power |
| Base Stats | `moveSpeed` | `float` | Movement speed |
| Reward | `reward` | `int` | Gold dropped on death |

### Where it is loaded

- **`Enemy/Infrastructure/Repositories/EnemyRepository.cs`** — loads `.asset` instances and maps them to `Enemy` domain entities.
- **`ScriptableObjects/Stage/StageConfigSO.cs`** — referenced directly inside `WaveConfig.enemyConfig` and `StageConfigSO.bossConfig` to define which enemies appear in each wave and as the boss.

### Where it is used

- **`Enemy/Application/UseCases/` and `IEnemyService.cs`** — receives enemy data via `EnemyDTO` built from this config.
- **`Wave/Presentation/Gameplay/WaveManager.cs`** — reads enemy configs from `StageConfigSO.waves` to spawn the correct enemy prefab per wave.

---

## HeroConfigSO

**File:** `ScriptableObjects/Hero/HeroConfigSO.cs`  
**Menu path:** `AshDefender/ScriptableObjects/HeroConfigSO`

Holds static base stats and skill assignments for a single hero (see `ARCHITECTURE.md` §14 and §28 Hero Data).

### Fields

| Group | Field | Type | Description |
|---|---|---|---|
| Identity | `heroId` | `string` | Unique identifier |
| Identity | `heroName` | `string` | Display name |
| Identity | `icon` | `Sprite` | Portrait sprite |
| Base Stats | `baseHealth` | `int` | Starting HP |
| Base Stats | `baseAttack` | `int` | Starting attack power |
| Base Stats | `baseDefense` | `int` | Starting defense |
| Skills | `skillSet` | `List<SkillConfigSO>` | Skills this hero can use |

### Where it is loaded

- **`Hero/Infrastructure/Repositories/HeroRepository.cs`** — loads `.asset` instances and maps them to `Hero` domain entities.

### Where it is used

- **`Hero/Application/UseCases/SelectHeroUseCase.cs`** — reads hero data when the player picks a hero during Squad Selection.
- **`Hero/Presentation/UI/HeroStatusUI.cs`** — displays hero name, icon, and base stats.
- **`Upgrade/Application/UseCases/UpgradeHeroUseCase.cs`** — uses base stats from this config as the starting point before applying upgrade deltas.

---

## SkillConfigSO

**File:** `ScriptableObjects/Skill/SkillConfigSO.cs`  
**Menu path:** `AshDefender/ScriptableObjects/SkillConfigSO`

Holds configuration for a single skill — used by both **Hero skills** (via `HeroConfigSO.skillSet`) and **Commander skills** (see `ARCHITECTURE.md` §19). Also defines the `SkillType` enum.

### Fields

| Group | Field | Type | Description |
|---|---|---|---|
| Identity | `skillId` | `string` | Unique identifier |
| Identity | `skillName` | `string` | Display name |
| Identity | `icon` | `Sprite` | Skill icon sprite |
| Identity | `skillType` | `SkillType` | Heal, Buff, AoEDamage, CrowdControl |
| Cost | `manaCost` | `int` | Mana required to cast |
| Cost | `cooldown` | `float` | Seconds between casts |
| Effect | `power` | `float` | Heal amount, damage value, or buff magnitude |

### Where it is loaded

- **`Hero/Infrastructure/Repositories/HeroRepository.cs`** — indirectly, via `HeroConfigSO.skillSet`.
- **`CommanderSkill/Infrastructure/Repositories/CommanderSkillRepository.cs`** — loads commander skill `.asset` instances directly.

### Where it is used

- **`Hero/Application/UseCases/ActivateSkillUseCase.cs`** — reads `manaCost`, `cooldown`, and `power` when the hero activates a skill.
- **`CommanderSkill/Application/UseCases/CastCommanderSkillUseCase.cs`** — reads `manaCost` to validate mana and `power` to apply the skill effect.
- **`CommanderSkill/Presentation/UI/CommanderSkillPanel.cs`** — displays skill icon, name, and cooldown to the player.

---

## StageConfigSO

**File:** `ScriptableObjects/Stage/StageConfigSO.cs`  
**Menu path:** `AshDefender/ScriptableObjects/StageConfigSO`

Holds the full definition of a stage: its wave list, boss, and unlock prerequisite (see `ARCHITECTURE.md` §22). Also defines the nested `WaveConfig` serializable class.

### Fields

| Group | Field | Type | Description |
|---|---|---|---|
| Identity | `stageId` | `string` | Unique identifier |
| Identity | `stageName` | `string` | Display name |
| Waves | `waves` | `List<WaveConfig>` | Ordered list of wave definitions |
| Boss | `bossConfig` | `EnemyConfigSO` | Boss enemy spawned after all waves |
| Unlock | `requiredPreviousStage` | `StageConfigSO` | Stage that must be completed first (null for Stage 1) |

### WaveConfig (nested)

| Field | Type | Description |
|---|---|---|
| `enemyConfig` | `EnemyConfigSO` | Which enemy type spawns in this wave |
| `spawnCount` | `int` | How many enemies spawn |
| `spawnInterval` | `float` | Seconds between each spawn |

### Where it is loaded

- **`Stage/Infrastructure/Repositories/StageRepository.cs`** — loads all stage `.asset` instances.
- **`Wave/Infrastructure/Repositories/WaveRepository.cs`** — reads `StageConfigSO.waves` to build the wave sequence.

### Where it is used

- **`Stage/Application/UseCases/StartStageUseCase.cs`** — validates `requiredPreviousStage` before allowing the player to enter.
- **`Stage/Presentation/UI/StageSelectPanel.cs`** — displays stage name and lock state based on `requiredPreviousStage`.
- **`Wave/Presentation/Gameplay/WaveManager.cs`** — iterates `waves` to spawn enemies, then spawns `bossConfig` after the final wave.

---

## UnitConfigSO

**File:** `ScriptableObjects/Unit/UnitConfigSO.cs`  
**Menu path:** `AshDefender/ScriptableObjects/UnitConfigSO`

Holds static configuration for a single deployable unit type (see `ARCHITECTURE.md` §15 and §28 Unit Data). Also defines the `UnitType` enum.

### Fields

| Group | Field | Type | Description |
|---|---|---|---|
| Identity | `unitId` | `string` | Unique identifier |
| Identity | `unitName` | `string` | Display name |
| Identity | `icon` | `Sprite` | Portrait sprite |
| Identity | `unitType` | `UnitType` | Melee, Ranged, Tank, Support |
| Deployment | `cost` | `int` | Energy cost to deploy |
| Base Stats | `health` | `int` | Max HP |
| Base Stats | `attack` | `int` | Attack power |
| Base Stats | `moveSpeed` | `float` | Movement speed |

### Where it is loaded

- **`Unit/Infrastructure/Repositories/UnitRepository.cs`** — loads `.asset` instances and maps them to `Unit` domain entities.

### Where it is used

- **`Deployment/Application/Validators/DeploymentValidator.cs`** — reads `cost` to verify the player has enough Energy before deploying.
- **`Deployment/Application/UseCases/DeployUnitUseCase.cs`** — passes unit config data to the deployment service.
- **`Deployment/Presentation/UI/DeploymentPanel.cs`** — displays unit icon, name, type, and energy cost to the player.
- **`Upgrade/Application/UseCases/UpgradeUnitUseCase.cs`** — uses base stats as the starting point before applying upgrade deltas.
