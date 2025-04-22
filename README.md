
# 🚀 AstroidAvoider

**AstroidAvoider** is a fast-paced 2D arcade-style mobile game developed in Unity. The goal is simple: survive as long as possible by dodging incoming asteroids — but the longer you last, the harder it gets.

Inspired by classic bullet-dodging games and polished with modern feedback systems, **AstroidAvoider** offers smooth gameplay, dynamic difficulty, and responsive player feedback for a highly replayable experience.

---

## 🎮 Current Features

### 🧠 Core Mechanics
- **Dynamic Asteroid Spawning**: Random directions and physics-based force control.
- **Zig-Zag & Homing Asteroids**: Intelligent enemies that track or trick the player.
- **Difficulty Manager**: Game gets harder as the player performs better — faster asteroids, tighter movement patterns.

### 🎯 Feedback & Feel
- **Near Miss Detection System**:
  - Asteroids passing dangerously close without collision trigger rewards.
  - Combo-safe detection logic ensures fair and meaningful detection.
- **Slow-Motion Effect**:
  - 0.15s time slowdown when a near miss occurs — for dramatic impact.
- **Visual + Audio Feedback**:
  - Dynamic camera shake
  - Boosted flame effect on the rocket
  - Retro-style floating `+10` score icons
  - Woosh-style sound cue

### 📈 Score System
- Time-based scoring + bonus for each successful avoid
- High score tracking with `PlayerPrefs`
- Fully resettable across sessions

### 🛠 Polished UI Elements
- Pause & Game Over Screens
- Play Again & Return to Menu with full state reset
- Difficulty indicator UI
- Pixel-art style floating bonus prefab integrated into Canvas

---

## 📁 Project Structure Highlights

```
Assets/
├── Scripts/
│   ├── Astreoid.cs
│   ├── PlayerMovement.cs
│   ├── ScoreSystem.cs
│   ├── GameOverHandler.cs
│   ├── DifficultyManager.cs
│   ├── NearMissFeedbackSystem.cs
│   ├── FloatingBonusPrefab.cs
│   └── PlayerPerformanceTracker.cs
├── Prefabs/
│   └── FloatingBonusIcon.prefab
├── Sounds/
│   └── near_miss_ping.wav
├── UI/
│   ├── Canvas
│   ├── TMP_Texts
│   └── BonusSpawnPoint
```

---

## 📅 Upcoming Features

- 🏆 **Mission / Achievement System**
  - Unlock rewards or multipliers for reaching milestones (e.g., 10 near misses)
- 🛸 **Unlockable Ships**
  - Different ship models with cosmetic or functional differences

---

## 🧪 Built With

- **Unity 2022+** (URP recommended)
- **TextMeshPro** for pixel-perfect UI
- **C#** scripting using Unity's MonoBehaviour system

---

## 👾 Screenshots

*(to be added)*

---

## 📢 Contributing / Feedback

If you're a developer, artist, or game feel enthusiast — feel free to fork the repo or create issues with suggestions and improvements!

---
