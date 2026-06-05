# Ash Defender

<p align="center">
  <img src="Docs/banner.png" alt="Ash Defender Banner" width="800">
</p>

## Overview

Ash Defender is a 2D Hero Defense game developed with Unity.

Set in a world devastated by the invasion of creatures from the Abyss, humanity has been pushed to the brink of extinction. The last remaining fortress, Gravewall, stands as the final defense protecting the Last Flame—the final source of hope and life for mankind.

Players take on the role of the last surviving commander, leading elite warriors into battle to defend Gravewall against endless waves of enemies and powerful bosses.

Unlike traditional Tower Defense games, players do not build towers. Instead, they strategically deploy units and heroes onto the battlefield to stop incoming enemies and protect the Core.

---

# Story

Many years ago, monsters emerged from the Abyss and invaded the world.

Kingdoms fell one after another.

Humanity's last hope now rests within a fortress called Gravewall, home of the Last Flame.

As the final commander, you must recruit warriors, unlock powerful abilities, and lead the defense against increasingly dangerous enemies to prevent the world from being consumed by darkness.

---

# Genre

- Hero Defense
- Real-Time Strategy
- Wave Survival
- Action RPG
- Level-Based Progression

---

# Core Gameplay Loop

```text
Prepare Squad
      ↓
Select Up To 4 Units
      ↓
Start Battle
      ↓
Deploy Units
      ↓
Defend The Core
      ↓
Defeat Enemy Waves
      ↓
Defeat Boss
      ↓
Receive Rewards
      ↓
Upgrade Units & Skills
      ↓
Unlock New Stage
      ↓
Repeat
```

---

# Gameplay Features

## Squad Preparation

Before entering a battle, players can:

- Select up to 4 units or heroes
- Configure their battle lineup
- Choose the best strategy for the stage

Each stage may require different team compositions to overcome unique enemy types and bosses.

---

## Unit Deployment System

Instead of placing defensive towers, players deploy combat units directly onto the battlefield.

Features:

- Deploy units using Energy
- Build and maintain a balanced army
- Control battlefield pressure through strategic deployment
- Adapt to enemy waves in real time

---

## Hero System

Heroes are powerful units capable of turning the tide of battle.

Hero features:

- Automatic target detection
- Auto movement toward nearby enemies
- Auto attack behavior
- Unique skills and abilities
- Upgradeable stats and talents

---

## Commander Skills

The Commander can support the battlefield using special abilities.

Examples:

- Healing allies
- Damage buffs
- Area-of-effect attacks
- Crowd control abilities
- Emergency battlefield support

Commander Skills consume Mana or Energy and must be used strategically.

---

## Wave Defense Gameplay

Each stage contains:

- Multiple enemy waves
- Regular enemies
- Elite enemies
- A final stage boss

Players must survive all waves while preventing enemies from destroying the Core.

---

## Progression System

Completing stages rewards players with resources used to strengthen their army.

Progression includes:

- Hero upgrades
- Unit upgrades
- Skill upgrades
- New hero unlocks
- Commander improvements
- Stage progression

---

# Victory Conditions

Players win when:

- All enemy waves are defeated
- The stage boss is eliminated
- The Core remains alive

---

# Defeat Conditions

Players lose when:

- Core HP reaches zero

---

# Maps & Bosses

## Map 1 – Simple Summer

### Enemies

- Slime
- Goblin

### Boss

- Giant Slime

---

## Map 2 – Poison Swamp

### Enemies

- Slime
- Spider
- Mushroom Monster

### Boss

- Giant Spider

---

## Map 3 – Dungeon

### Enemies

- Skeleton
- Bat
- Goblin Warrior

### Boss

- Skeleton King

---

## Map 4 – Frozen Fortress Lands

### Enemies

- Ice Slime
- Ice Goblin
- Snow Wolf

### Boss

- Ice Golem

---

## Map 5 – Abyss Gate

### Enemies

- Demon Slime
- Dark Knight
- Fire Bat

### Boss

- Demon Lord

---

# Items

## Health Potion

Restores Hero HP during battle.

---

## Mana Crystal

Restores Mana or Energy used for Commander Skills.

---

## Gold Coin

Main currency used for:

- Hero upgrades
- Purchasing items
- Progression systems

---

## Soul Fragment

Used to:

- Upgrade skills
- Unlock new heroes

---

## Ancient Scroll

Used to:

- Unlock Commander Skills
- Increase magical power

---

## Revival Feather

Used to revive fallen heroes.

---

# Planned Systems

## Combat System

- Melee combat
- Ranged combat
- Damage calculation
- Critical hits
- Status effects

---

## Hero AI

- Auto target selection
- Auto movement
- Auto attack
- Skill usage

---

## Enemy AI

- Follow path toward Core
- Attack deployed units
- Attack Core when in range
- Boss-specific behaviors

---

## Wave System

- Wave spawning
- Difficulty scaling
- Elite enemies
- Boss encounters

---

## Upgrade System

- Hero Leveling
- Unit Enhancement
- Skill Upgrades
- Commander Progression

---

## Save System

- Save player progression
- Save unlocked content
- Save upgrades
- Save stage completion

---

# Technology Stack

## Engine

- Unity 6

## Programming Language

- C#

## Architecture

- Clean Architecture
- Feature-Based Architecture
- Event-Driven Architecture

## Libraries

- VContainer
- UniTask
- Addressables

## Version Control

- Git
- GitHub

---

# Project Structure

```text
Assets
│
├── Art
├── Audio
├── Animations
├── Materials
├── Prefabs
├── Scenes
├── ScriptableObjects
│
├── Features
│   ├── Hero
│   ├── Unit
│   ├── Enemy
│   ├── Combat
│   ├── Skills
│   ├── WaveSystem
│   ├── Upgrade
│   └── SaveSystem
│
├── Shared
│   ├── Core
│   ├── Events
│   ├── Utilities
│   └── Constants
│
└── UI
```

---

# Development Roadmap

## Week 1

- Project setup
- Asset integration
- Core gameplay prototype

## Week 2

- Hero movement
- Auto attack system
- Enemy AI

## Week 3

- Wave spawning system
- Combat system

## Week 4

- Skill system
- Commander skills

## Week 5

- Build first three maps
- Enemy balancing

## Week 6

- Boss fights
- User interface
- HP bars
- Audio integration

## Week 7

- Shop system
- Upgrade system
- Save and load functionality

## Week 8

- Bug fixing
- Game balancing
- Visual polish
- Demo preparation

---

# Development Team

PRU213 Team Project

---

# License

This project is developed for educational purposes as part of the PRU213 course.

---

# Screenshots

Coming Soon

---

# Future Improvements

- Additional Heroes
- More Commander Skills
- New Enemy Types
- Equipment System
- Hero Talent Tree
- Endless Mode
- Achievement System
- Leaderboards
- Multiplayer Co-op Defense