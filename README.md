# 🚀 Asteroid Avoider

**Asteroid Avoider** is a fast-paced, survival-based mobile arcade game developed in Unity.  
The objective is simple: avoid increasingly dangerous asteroids, stay alive, and climb the score ranks — the longer you survive, the harder it gets.

---

## 🎮 Gameplay

You control a modular spaceship in deep space.  
Your mission is to avoid waves of procedurally spawned asteroids with varied behaviors.

### Core Mechanics:
- **Dodge to survive**: One collision and it’s game over.
- **Score system**: Earn points by surviving time and avoiding asteroids.
- **Dynamic difficulty**: The game gets harder as you perform better.
- **Split asteroids**: Some asteroids divide into smaller ones after a delay, increasing chaos.

---

## 🧠 Key Features

| Feature | Description |
|--------|-------------|
| 💥 Split Asteroids | Asteroids automatically split into smaller ones mid-air or on impact |
| 🧠 Dynamic Difficulty | Difficulty increases based on player performance every 15 seconds |
| 🔄 Zig-Zag & Homing Patterns | Advanced movement behaviors that challenge the player |
| ⏱️ Time-Based Scoring | Score increases with time survived |
| 🎯 Avoidance Rewards | Score boosts for successfully dodging asteroids |
| 💀 Game Over UI | Score shown at the end, gameplay halted |

---

## 🖥️ Current Scenes

- `Scene_menu`: Main menu (WIP)
- `Scene_main`: Main gameplay loop

---

## 🛠️ Tech Overview

| System | Scripts |
|--------|---------|
| Score System | `ScoreSystem.cs` (singleton, time & event based scoring) |
| Performance Tracking | `PlayerPerformanceTracker.cs` |
| Difficulty Control | `DifficultyManager.cs` |
| Asteroid Behavior | `Astreoid.cs`, `AstreoidSpawner.cs` |
| Game Flow | `GameOverHandler.cs` |

---

## 🎨 UI

- In-game score display
- Game Over screen with final score summary
- Optional debug logging for asteroid behavior

---

## 📱 Target Platform

- Mobile (Android first)
- Optional keyboard & mouse input for testing in Unity Editor

---

## 🚧 Features In Progress

- 💰 Coin system (collectibles)
- 🛡️ Power-ups (shields, slow-motion)
- 📈 High score saving & local leaderboard
- 🛍️ Upgrade shop (post-run improvements)

---

## 👨‍💻 Developed By

**Mertalp Taşdelen**  
Follow the journey at: [github.com/MertalpTasdelen](https://github.com/MertalpTasdelen)

---

## ❤️ Notes

This project started as a learning exercise and is evolving into a full-featured arcade experience.  
All feedback, contributions, and ideas are welcome!

> Built with Unity. Fueled by coffee & pixels ☕🕹️
