
# 🚀 Asteroid / Astreoid Avoider

**Asteroid Avoider** is a fast-paced arcade-style mobile game built in Unity. The goal is simple: survive as long as possible by dodging incoming asteroids — and as your score climbs, the game ramps up the difficulty.

> Note: In the project code and assets you’ll see the spelling **"Astreoid"** (e.g. `AstreoidSpawner`, `Astreoid.cs`). The README uses the correct term “Asteroid”, but keeps the code naming as-is.

---

## 🎮 Current Features (Latest)

### 🧠 Core Gameplay
- **Dynamic Spawning + Difficulty Scaling**: Asteroids spawn from screen edges and accelerate over time via `DifficultyManager` → `AstreoidSpawner`.
- **Enemy Variety**: Zig-zag movement, homing behavior, and (at higher difficulty) **splitting asteroids**.
- **Object Pooling**: `AstreoidPool` preloads and recycles asteroids to reduce runtime allocations.
- **Stage System + Bonus Stage**: Periodic stage transitions show on-screen messages and trigger a **15s Bonus Stage** where lasers are enabled.
- **Mobile Controls + Screen Wrap**: On-screen joystick movement with toroidal screen-wrap (fly out one side, appear on the other).
- **Pause System**: Pause panel + time scale control with optional music pausing.

### 🎯 Feedback & Feel
- **Near Miss System**: Passing close to an asteroid triggers score bonus + feedback (camera shake, flame boost, SFX, floating bonus icon).
- **Game Over Juice**: Optional short slow-motion on crash + delayed Game Over UI for better impact.
- **Stage Transition Feedback**: Stage-complete / stage-incoming / bonus-stage messages with audio cues and fade.

### 📈 Score & Progression
- **Time-Based Score** with a multiplier (updates every frame while alive).
- **Avoid / Near-Miss Bonuses**: Avoiding asteroids and near-misses add bonus points.
- **Laser Bonus Points** during Bonus Stage (laser hits add score).
- **High Score** saved via `PlayerPrefs`.
- **Online Submission (Best Score Only)**: High score is submitted only if it beats the last submitted score.

### 🛠 UI / Meta
- **Player Name Entry** stored in `PlayerPrefs` (used for leaderboard + achievements).
- **Achievements Menu** (server-backed) with title/description/target progress display.
- **Global Leaderboard Menu** (server-backed) showing top entries + your neighborhood.
- **Rewarded Continue**: On Game Over, player can watch a rewarded ad to continue the run.

---

## 📁 Project Structure Highlights

```
Assets/
├── Scenes/
│   ├── Scene_Menu.unity
│   └── Scene_Main.unity
├── Scripts/
│   ├── Astreoid.cs
│   ├── AstreoidPool.cs
│   ├── AstreoidSpawner.cs
│   ├── GameOverHandler.cs
│   ├── Laser.cs
│   ├── LaserShooter.cs
│   ├── NearMissFeedbackSystem.cs
│   ├── PlayerHealth.cs
│   ├── PlayerMovement.cs   (class: PlayerMovements)
│   ├── ScoreSystem.cs
│   ├── AchievementMenuUI.cs
│   ├── GlobalScoreboardMenuUI.cs
│   ├── PlayerNameEntryUI.cs
│   └── Managers/
│       ├── AchievementApiClient.cs
│       ├── BonusStageManager.cs
│       ├── DifficultyManager.cs
│       ├── LeaderboardApiClient.cs
│       ├── PauseManager.cs
│       └── StageTransitionManager.cs
└── Prefabs/
  └── (UI prefabs, floating bonus, etc.)
```

---

## 🌐 Online Features

This project integrates with a backend API for achievements and global leaderboard.

- **Achievements API**: `https://api.yeninesilevim.com/achievements/...`
- **Leaderboard API**: `https://api.yeninesilevim.com/scores/...`

If the API is unreachable, the game continues to run, but menus may show empty data and you’ll see request errors in the Console.

---

## 🧪 Built With

- **Unity 6.2** (`6000.2.14f1`)
- **TextMeshPro** for UI
- **Unity Input + Mobile Joystick** (Floating Joystick)
- **Unity Ads** for rewarded continue

---

## 🎵 Credits (Audio)

This project uses music from OpenGameArt. Please see the original pages for the full license/attribution requirements.

- **Space Boss Battle Theme** — Matthew Pablo
  - Source: https://opengameart.org/content/space-boss-battle-theme
  - Attribution: The author provides specific attribution instructions on the page.

- **8-Bit Space Adventure Theme** — emanresU
  - Source: https://opengameart.org/content/8-bit-space-adventure-theme
  - Attribution note (from the page): Attribution is appreciated but not required.

---

## 👾 Screenshots

*(to be added)*

---

## 📢 Contributing / Feedback

If you're a developer, artist, or game feel enthusiast — feel free to fork the repo or create issues with suggestions and improvements!

---

## ▶️ How To Run

1. Open the project in Unity (`6000.2.14f1`).
2. Open `Assets/Scenes/Scene_Menu.unity`.
3. Press Play.

## 🕹️ Controls

- **Move**: On-screen joystick
- **Bonus Stage**: Laser shooting is enabled automatically for ~15 seconds

---
