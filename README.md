# 🛰️ Asteroid Avoider

**Asteroid Avoider** is a fast-paced, mobile-first space survival game developed in Unity. The player controls a modular spaceship, aiming to survive as long as possible while avoiding dynamically spawning asteroids that evolve in behavior and difficulty over time.

This project started as a learning prototype and is actively evolving into a polished mobile game with responsive mechanics, progressive difficulty, and game feel enhancements.

---

## 🎮 Gameplay Overview

- Tap and drag to steer your spaceship
- Avoid procedurally spawned asteroids
- The longer you survive, the harder it gets

---

## 🚀 Features Implemented

### 🧠 Dynamic Difficulty Adjustment (DDA)
A real-time difficulty system adjusts the game based on the player's performance:
- Difficulty level increases every 15 seconds if the player is performing well
- Metrics tracked:
  - Time survived
  - Asteroids avoided
  - Hits taken
- Increasing difficulty affects:
  - Asteroid spawn rate
  - Movement complexity
  - Velocity ranges

### 🌀 Advanced Asteroid Behaviors
Asteroids evolve in behavior as difficulty increases:
| Difficulty Level | Behavior |
|------------------|----------|
| 1–2              | Straight-line asteroids |
| 3–4              | Zigzag (sinusoidal movement) |
| 5+               | Homing behavior (slow tracking toward player) |

Each behavior adds unpredictability and challenge while maintaining fairness.

### 💥 Game Feel Enhancements
- **Sinusoidal camera shake** on difficulty increase
- **Red flash overlay** for intense visual feedback
- **On-screen difficulty level UI** to track progression

---

## 🎯 Controls

- **Touchscreen (Mobile)**: Hold to move toward touch position
- **Keyboard (Editor)**: Controlled via mouse emulation
- The player is always kept within the screen bounds via a wrapping mechanic

---

## 🛠️ Technical Breakdown

| Script | Role |
|--------|------|
| `PlayerMovements.cs` | Controls player movement and rotation based on touch |
| `PlayerPerformanceTracker.cs` | Tracks in-game performance stats |
| `DifficultyManager.cs` | Calculates difficulty level and triggers feedback |
| `AstreoidSpawner.cs` | Spawns asteroids and injects difficulty-based logic |
| `Astreoid.cs` | Handles asteroid movement, collision logic, and complex patterns |

---

## 📂 Scenes

- `Scene_menu`: Main menu (WIP)
- `Scene_main`: Core gameplay scene (all main systems implemented here)

---

## 📱 Target Platform

- Android (Initial release)
- iOS (Planned)

---

## 📌 Upcoming Features

- Split-type asteroids (split into two on impact)
- Collectibles / coins
- Shield & power-up mechanics
- Score system & leaderboard
- In-game settings (difficulty toggle, sound options)

---

## 🎨 Assets & Credits

- Spaceship assets from **Star Sparrow Modular Spaceship Kit**
- Game architecture and input design inspired by Unity mobile dev best practices

---

## 👨‍💻 Author

Made with ❤️ by [Mertalp Taşdelen](https://github.com/MertalpTasdelen)

---

> This project is in active development. Feedback and collaboration are welcome!
