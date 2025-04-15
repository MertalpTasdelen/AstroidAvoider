# 🛰️ Asteroid Avoider

Asteroid Avoider is a mobile-focused space survival game built with Unity.  
Your goal is to maneuver a modular spaceship and avoid waves of dynamically generated asteroids as long as you can.

This game started as a Udemy learning project and is evolving into a polished mobile game with smart difficulty progression and responsive visual feedback.

---

## 🎮 Core Gameplay

- Control your spaceship to avoid incoming asteroids.
- Survive as long as possible and challenge yourself as the game progressively gets harder.

---

## 🧠 Features Implemented

### 🧩 Dynamic Difficulty Adjustment (DDA)
A real-time difficulty system that adjusts the spawn rate and behavior of asteroids based on player performance:

- **Performance Metrics Tracked:**
  - Time survived
  - Number of asteroids avoided
  - Number of collisions (hits)

- **Adjustment Logic:**
  - Every 15 seconds, the system evaluates the player's score (`avoided - hits`) and updates the difficulty level.
  - Difficulty level affects asteroid spawn rate and velocity range.

### 🌀 Advanced Asteroid Behaviors
Starting from level 3, asteroids display more complex movement patterns:

- **Zig-Zag Movement**: Sinusoidal lateral motion for harder prediction  
- *(Planned)* Homing asteroids, burst speed, and random directional changes for higher levels

### 🌈 Visual Feedback
Difficulty changes are clearly communicated through:

- **Sinusoidal Screen Shake**: Adds punch when the game gets tougher
- **Red Flash Overlay**: Indicates danger and increasing intensity
- **On-Screen Difficulty Display**: Live UI update of the current difficulty level

---

## 🧪 Current Scenes

- `Scene_menu`: Main menu screen
- `Scene_main`: Core gameplay scene (where all logic is implemented)

---

## 📂 Scripts Overview

| Script | Purpose |
|--------|---------|
| `PlayerPerformanceTracker` | Tracks real-time performance data |
| `DifficultyManager` | Adjusts difficulty and triggers visual feedback |
| `AstreoidSpawner` | Spawns asteroids based on difficulty level |
| `Astreoid` | Handles movement logic and collision detection |

---

## 🛠️ How to Run

1. Open the project in Unity
2. Load `Scene_main`
3. Hit play to test gameplay
4. Observe the difficulty level increasing over time and the resulting visual/audio feedback

---

## 🎯 Next Steps

- Add new asteroid behaviors (Homing, Split, Burst)
- Implement in-game economy (coins, upgrades)
- Leaderboard and score tracking
- Polish UI and optimize for mobile screen resolutions

---

## 📱 Target Platform

- Android (initial)
- iOS (planned)

---

## 🙌 Credits

- Modular spaceship assets from Star Sparrow
- Based on foundational lessons from a Udemy Unity course

---

Made with ❤️ by [Mertalp Taşdelen](https://github.com/MertalpTasdelen)
