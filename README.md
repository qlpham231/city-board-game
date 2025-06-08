# 🏙️ City Board Game – Unity Digital Component

This repository contains the **Unity** implementation of the digital screen used in a hybrid card game, developed as part of a Master’s thesis. The screen simulates a dynamic city that responds to player decisions, enhancing the overall gameplay experience.

---

## 🎓 Thesis Context

- Created for a Master’s thesis exploring **physical‑digital hybrid gameplay**.
- The Unity screen visualizes city evolution based on in-game player actions.
- Developed in **Unity Editor 2021.3.5f1 (LTS)**.
- Contains all source code for the digital side of the game.

---

## 🚀 Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download)  
- Unity **2021.3.5f1** (LTS version) recommended

### Clone & Open

1. Clone the repo:
   ```bash
   git clone https://github.com/qlpham231/city-board-game.git
   ```
2. Open **Unity Hub**  
3. Click **"Add"**, then select the folder you just cloned  
4. Open the project with **Unity version 2021.3.5f1**  
5. Wait for Unity to **import and compile the assets** (this may take a few minutes)  
6. Navigate to `Assets/Scenes/`, open the **main scene**, and hit **Play** in the Unity Editor to preview the simulation  

## 📁 Project Structure
```bash
city-board-game/
├── Assets/
│   ├── Prefabs/          # Reusable GameObjects (e.g., UI panels)
│   ├── Resources/        # Audio and textures loaded at runtime
│   ├── Scenes/           # Unity scenes, including the main city simulation
│   ├── Scripts/          # C# scripts controlling game logic and visuals
├── ProjectSettings/      # Unity project settings
├── Packages/             # Unity package dependencies
├── LICENSE               # Project license (MIT)
└── README.md             # Project documentation (you are here)
```

## 🧾 License

This project is licensed under the [MIT License](LICENSE).