# 🎰 Unity Slot Machine Game Assignment

A fully functional, modular 2D Classic 3-Reel Slot Machine game built with **Unity (2022.3 LTS / Unity 6)** following industry-standard design patterns, clean OOP principles, and authentic mechanical game feel.

---

## 🎮 Game Overview

The game simulates a retro 16-bit arcade/casino slot machine featuring:
- **3 Dynamic Spinning Reels** with vertical endless symbol wrapping.
- **Natural Physics & Easing**: Progressive acceleration, sequential reel stops (**Reel 1 → Reel 2 → Reel 3**), and mechanical **bounce/snap-back overshoot** on landing.
- **RNG-Driven Outcomes**: Mathematical separation between outcome generation and visual presentation.
- **Dynamic Wallet Economy**: Balance deductions, real-time balance tracking, and configurable multiplier payouts.
- **Right-Click Context Menu**: Toggleable betting menu supporting **Bet 10G**, **Bet 50G**, and **Bet 100G**.
- **Interactive Mechanical Lever**: Dual-state lever animation that triggers spin sequences.
- **Win Celebration Modal**: Grand popup dialog showcasing winning payouts with **YES (Replay with same bet)** and **NO (Collect & Return)** options.

---

## 🏆 Winning Combinations & Payout Table

| Symbol | Combination | Multiplier | Example Payout (Bet = 50G) |
| :---: | :---: | :---: | :---: |
| 7️⃣ **Lucky Seven** | 3x Match | **100x (JACKPOT)** | **5,000G** |
| 🍫 **Triple BAR** | 3x Match | **50x (Mega Win)** | **2,500G** |
| 🔔 **Bell** | 3x Match | **25x (Big Win)** | **1,250G** |
| 🍒 **Cherries** | 3x Match | **10x (Standard Win)** | **500G** |
| 🍒 **Cherries** | 2x Match | **2x (Consolation Return)** | **100G** |

---

## 🕹️ Controls & How to Play

- **Right Click**: Open / Close the Bet Selection Context Menu.
- **Click Bet 10G / 50G / 100G**: Sets the wager, closes the menu, pulls the mechanical lever, and begins the spin sequence.
- **Click Lever**: Pulls the machine lever and spins with the current active bet.
- **Win Dialog Buttons**:
  - **YES**: Replay immediately with the exact same wager.
  - **NO**: Collect winnings and dismiss modal.

---

## 🧠 Architecture & Technical Thought Process

The project is structured according to **SOLID principles** and separation of concerns:

```
Assets/
├── Prefabs/
│   └── SymbolItem.prefab          # Reusable symbol UI prefab
├── Scenes/
│   └── MainScene.unity            # Primary game scene
├── Scripts/
│   ├── Logic/
│   │   ├── SymbolType.cs          # Strongly-typed enum for symbols
│   │   ├── RNGManager.cs          # Weighted probability RNG with evaluator debug hooks
│   │   └── BetManager.cs          # Player balance, wagers, and payout calculation
│   ├── Slot/
│   │   ├── Reel.cs                # Endless scroll, symbol pooling, and bounce easing
│   │   ├── SlotMachineController.cs # Master coordinator for sequential reel stopping
│   │   └── LeverController.cs     # Dual-state mechanical lever animation
│   └── UI/
│       ├── BetMenuController.cs   # Right-click context menu toggle
│       └── WinPopupController.cs  # Win celebration dialog and replay flow
└── UI/                            # Sprites, sliced buttons, and graphics
```

### 1. Pre-calculated RNG Flow (Casino Industry Standard)
Unlike naive implementations where random movement determines symbols by physics collision, real casino slot machines determine the outcome via a cryptographically fair/weighted Random Number Generator **upfront** upon spin initiation. The visual reels then accelerate, cycle infinitely using symbol pooling, and decelerate to land exactly on the predetermined outcome with zero visual seams.

### 2. Evaluator Test Hooks (Force Win)
In `RNGManager.cs`, evaluators can toggle `forceWin = true` and pick any `forcedSymbol` directly in the Unity Inspector to verify jackpot and win calculations deterministically.

### 3. Smooth Mechanical Easing & Bounce
Each reel features:
1. **Acceleration Phase**: Smooth ramp-up using `Mathf.Lerp`.
2. **Infinite Looping Phase**: Vertical wrapping of symbol RectTransforms.
3. **Deceleration & Snap**: Precision alignment to the center payline (`y = 0`).
4. **Physical Bounce**: A 2-phase overshoot (`y = -bounceOvershoot`) and spring return (`y = 0`) using `Mathf.SmoothStep` for authentic arcade game feel.

---

## 🚀 How to Run the Project

### Running in Unity Editor
1. Clone the repository:
   ```bash
   git clone https://github.com/axel2310/slot_game.git
   ```
2. Open **Unity Hub** and click **Add** → select the cloned directory.
3. Recommended Editor: **Unity 2022.3 LTS** or **Unity 6**.
4. Open `Assets/Scenes/MainScene.unity` and click **Play (`▶`)**.

### Running WebGL Build (Browser Play)
1. Navigate to `/Build/WebGL/` folder.
2. Run a local web server (e.g. using Python: `python -m http.server 8000` inside `/Build/WebGL/`).
3. Open `http://localhost:8000` in any modern web browser.

---

## 🌟 Bonus Features Implemented
- **2-State Mechanical Lever**: Dual sprite layering with socket plunge and recoil animation.
- **Win Celebration Modal**: Integrated `popup.png` with custom-sliced golden `Yes_No_Btn` sprites.
- **Consolation Multiplier**: 2-Cherry match returns 2x bet so the player remains engaged.
- **Full Inspector Debugging**: Runtime test hooks to evaluate payouts and reel behaviors on demand.
