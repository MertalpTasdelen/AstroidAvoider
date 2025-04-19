# Asteroid Avoider 🚀

Asteroid Avoider is an endless dodge-and-survive style mobile space game built with Unity. The player controls a spaceship, avoiding incoming asteroids of varying behaviors while experiencing a visually dynamic galactic environment.

---

## 🎮 Core Gameplay
- **Avoid to survive**: Player controls a spaceship with touch or mouse and avoids procedurally spawned asteroids.
- **Dynamic difficulty**: The game increases in difficulty over time using a `DifficultyManager`, introducing faster spawns, zigzag/homing/splitting asteroids.
- **Score system**: Players earn score based on time survived and extra points for avoiding asteroids.
- **High score tracking**: Best score is saved using `PlayerPrefs` and displayed on game over screen.

---

## 🚀 Features Implemented

### ✅ Parallax Starfield Background
- 3-layer parallax system: distant stars, nebula, and fast-moving foreground stars.
- Background scrolls **opposite to player movement** for immersive depth (like [this reference](https://www.youtube.com/watch?v=_kzgG1n2eI0)).
- Layers scroll faster as difficulty increases.

### ✅ Asteroid Behaviors
- **ZigZag**: Wobbly movement.
- **Homing**: Locks onto player’s position at spawn.
- **Split**: After delay, breaks into 2 smaller asteroids.

### ✅ Performance Tracker
- Tracks total survived time, asteroids avoided, and damage taken.
- Powers dynamic difficulty system.

### ✅ Game Over System
- Game ends on player crash.
- Displays final and high score.
- Includes "Play Again" and "Return to Menu" options with proper state resets.

### 🔄 Trail + Flame Effect (Ongoing)
- Added **TrailRenderer** to player ship.
- Dynamically emits trail only during movement.
- Added optional **fire sprite particle system** behind the ship for a rocket engine flame look.

---

## 📁 Structure Overview
```
Assets/
├── Scripts/
│   ├── PlayerMovement.cs
│   ├── AstreoidSpawner.cs
│   ├── Astreoid.cs
│   ├── ScoreSystem.cs
│   ├── GameOverHandler.cs
│   ├── DifficultyManager.cs
│   └── PlayerPerformanceTracker.cs
│
├── Prefabs/
│   ├── AsteroidPrefabs
│   └── BackgroundLayers
│
├── Sprites/
│   ├── AA1.png (Fast stars)
│   ├── AA2.png (Nebula)
│   └── AA3.png (Distant stars)
│
├── Materials/
│   └── Trail_Fire_Mat.mat
```

---

## 🔧 In Progress / To Do
- [ ] Add coin system and in-game shop
- [ ] Introduce power-ups (shield, slow time, score boost)
- [ ] Ship skin unlocks
- [ ] Mission/achievement system
- [ ] Polished sound and particle feedbacks

---

## 🧠 Built With
- Unity 2022+
- C# scripting
- 2D URP Pipeline (optional)

---

## 👨‍🚀 Credits & License
This game is developed as a personal project by [@MertalpTasdelen](https://github.com/MertalpTasdelen) for gameplay experimentation and Unity mastery.

Sprites used from OpenGameArt, Itch.io, and AI-generated sources under free/non-commercial licenses.

