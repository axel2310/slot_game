using System;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages player wallet balance, bet placement, and payout calculations.
/// Triggers lever pull and spin sequence automatically on bet click.
/// </summary>
public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

    [Header("Wallet Settings")]
    [SerializeField] private int startingBalance = 500;
    private int currentBalance;

    [Header("Current Bet")]
    [SerializeField] private int currentBet = 10;

    [Header("UI Text Displays")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI currentBetText;
    [SerializeField] private TextMeshProUGUI winAmountText;

    // Events
    public event Action<int> OnBalanceChanged;
    public event Action<int> OnBetChanged;
    public event Action<int, SymbolType> OnWin;

    public int CurrentBalance => currentBalance;
    public int CurrentBet => currentBet;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentBalance = startingBalance;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void SetBet(int betAmount)
    {
        currentBet = betAmount;
        OnBetChanged?.Invoke(currentBet);
        UpdateUI();
        Debug.Log($"<color=cyan>[BetManager] Bet changed to: {currentBet}G</color>");
    }

    // --- Direct UI Button Handlers ---
    public void OnClickBet10()
    {
        SetBet(10);
        ExecuteBetAndSpin();
    }

    public void OnClickBet50()
    {
        SetBet(50);
        ExecuteBetAndSpin();
    }

    public void OnClickBet100()
    {
        SetBet(100);
        ExecuteBetAndSpin();
    }

        public void SpinWithCurrentBet()
    {
        ExecuteBetAndSpin();
    }

    private void ExecuteBetAndSpin()
    {
        // 1. Check balance first
        if (!TryPlaceBet()) return;

        // 2. Close Menu
        BetMenuController menu = FindObjectOfType<BetMenuController>();
        if (menu != null) menu.CloseMenu();

        // 3. Pull Lever AND Spin!
        if (LeverController.Instance != null)
        {
            LeverController.Instance.PullLever(() =>
            {
                if (SlotMachineController.Instance != null)
                {
                    SlotMachineController.Instance.TriggerSpinAfterBet();
                }
            });
        }
        else if (SlotMachineController.Instance != null)
        {
            SlotMachineController.Instance.TriggerSpinAfterBet();
        }
    }

    public bool TryPlaceBet()
    {
        if (currentBalance >= currentBet)
        {
            currentBalance -= currentBet;
            OnBalanceChanged?.Invoke(currentBalance);
            UpdateUI();
            Debug.Log($"<color=orange>[Bet Placed] Deducted: {currentBet}G | New Balance: {currentBalance}G</color>");
            return true;
        }

        Debug.LogWarning("[BetManager] Not enough coins to place bet!");
        return false;
    }

    public int CalculatePayout(SymbolType[] outcome)
    {
        int winAmount = 0;

        // 3-of-a-kind
        if (outcome[0] == outcome[1] && outcome[1] == outcome[2])
        {
            SymbolType s = outcome[0];
            int multiplier = 10;

            switch (s)
            {
                case SymbolType.Seven:  multiplier = 100; break;
                case SymbolType.Bar:    multiplier = 50;  break;
                case SymbolType.Bell:   multiplier = 25;  break;
                case SymbolType.Cherry: multiplier = 10;  break;
            }

            winAmount = currentBet * multiplier;
            currentBalance += winAmount;
            OnWin?.Invoke(winAmount, s);
            Debug.Log($"<color=yellow>[WINNER] 3x {s}! Multiplier: {multiplier}x, Won: {winAmount}G</color>");
        }
        // Bonus 2x Cherries
        else if (CountSymbol(outcome, SymbolType.Cherry) == 2)
        {
            winAmount = currentBet * 2;
            currentBalance += winAmount;
            OnWin?.Invoke(winAmount, SymbolType.Cherry);
            Debug.Log($"<color=orange>[BONUS WIN] 2x Cherries! Won: {winAmount}G</color>");
        }

        OnBalanceChanged?.Invoke(currentBalance);
        UpdateUI(winAmount);
        return winAmount;
    }

    private int CountSymbol(SymbolType[] outcome, SymbolType type)
    {
        int count = 0;
        foreach (var s in outcome) if (s == type) count++;
        return count;
    }

    public void AddBalance(int amount)
    {
        currentBalance += amount;
        UpdateUI();
    }

    private void UpdateUI(int lastWin = 0)
    {
        if (balanceText != null) balanceText.text = $"{currentBalance}G";
        if (currentBetText != null) currentBetText.text = $"Bet: {currentBet}G";
        if (winAmountText != null) winAmountText.text = lastWin > 0 ? $"WIN: +{lastWin}G" : "";
    }
}