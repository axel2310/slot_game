# Slot Game Assignment - Unity

A 2D 3-Reel Slot Machine game built in Unity for the assignment test.

---

## 🎮 Game Overview
The game implements a classic 3-reel slot machine featuring smooth spinning reels, sequential stopping (Reel 1 -> Reel 2 -> Reel 3), and physical bounce-back easing on landing. It includes a wallet economy with dynamic betting, RNG-based outcome calculation, right-click context menu, and a win celebration popup.

---

## ✨ Features
- **Smooth Reel Animations**: Acceleration, seamless vertical looping of symbols, and bounce easing upon stop.
- **Sequential Stopping**: Reel 1 stops first, followed by Reel 2 and Reel 3 with suspense delays.
- **Fair RNG System**: Outcomes are pre-calculated using weighted random distribution, and reels smoothly stop on the target symbols.
- **Betting & Wallet System**: Starting balance of 500G with selectable bets of 10G, 50G, and 100G.
- **Right-Click Bet Menu**: Right-click anywhere on the screen to toggle the betting menu open/closed (matches original reference GIF).
- **Interactive Lever Animation**: Pulls down into the socket and springs back up when placing a bet.
- **Win Celebration Popup (Bonus Feature)**: Winning combinations trigger a modal using `popup.png` and custom-sliced `Yes_No_Btn.png`:
  - **YES**: Spin again immediately with the same bet amount.
  - **NO**: Collect winnings and dismiss popup.
- **16-Bit Retro Audio (Bonus Feature)**: Custom sound effects for lever pull, continuous reel spin ticking, mechanical reel stops, and celebratory win jingles.

---

## 🏆 Payout Table

| Symbol | Combination | Multiplier | Example (Bet = 10G) | Example (Bet = 50G) |
| :--- | :--- | :--- | :--- | :--- |
| **Lucky Seven (7)** | 3x Match | **100x (Jackpot)** | 1000G | 5000G |
| **Triple BAR** | 3x Match | **50x (Mega Win)** | 500G | 2500G |
| **Bell** | 3x Match | **25x (Big Win)** | 250G | 1250G |
| **Cherries** | 3x Match | **10x (Standard Win)** | 100G | 500G |
| **Cherries** | 2x Match | **2x (Bonus Return)** | 20G | 100G |

---

## 🕹️ Controls
- **Right-Click**: Open / Close the Bet Selection Menu.
- **Bet 10G / 50G / 100G**: Sets wager, closes menu, pulls lever, and spins reels.
- **Win Modal Buttons**:
  - `YES`: Re-spins immediately with the same bet.
  - `NO`: Closes modal and keeps balance.

---

## 🧠 Architecture & Approach
1. **Pre-calculated RNG Architecture**: When a spin starts, `RNGManager` determines the 3 target symbols immediately based on probability weights. The visual reels spin and align the target symbols to the payline (`y = 0`) before playing the bounce animation.
2. **Evaluator Testing / Debug Hook**: To make testing fast and deterministic:
   - Select `GameController` in the Hierarchy.
   - In `RNGManager`, check `Force Win` (boolean) and select any `Forced Symbol`.
   - Every spin will now guarantee a 3-of-a-kind win with that symbol.
3. **Folder Organization**:
   - `Assets/Scripts/Slot/`: `Reel.cs`, `SlotMachineController.cs`, `LeverController.cs`
   - `Assets/Scripts/Logic/`: `RNGManager.cs`, `BetManager.cs`, `SymbolType.cs`
   - `Assets/Scripts/ui/`: `BetMenuController.cs`, `WinPopupController.cs`
   - `Assets/Scripts/Audio/`: `SoundManager.cs`
   - `Assets/Prefabs/`: `SymbolItem.prefab`
   - `Assets/Sounds/`: Retro `.wav` audio files
   - `Assets/Scenes/`: `MainScene.unity`
   - `Build/WebGL/`: Playable WebGL browser build

---

## 🚀 How to Run

### 1. Running the WebGL Build (Browser)
The playable browser build is included directly inside the repository under `/Build/WebGL/`:

Using Python:
```bash
cd Build/WebGL
python -m http.server 8000
```
Then open `http://localhost:8000` in any web browser.

Alternatively, you can use any local static server (e.g. Node `npx serve Build/WebGL` or VS Code Live Server).

### 2. Running in Unity Editor
1. Clone this repository:
   ```bash
   git clone https://github.com/axel2310/slot_game.git
   ```
2. Open **Unity Hub** -> **Add** -> Select the project folder.
3. Recommended Editor: **Unity 6** or **Unity 2022.3 LTS**.
4. Open `Assets/Scenes/MainScene.unity` and click **Play**.
