using UnityEngine;

/// <summary>
/// Handles fair RNG calculations and supports test modes for evaluator review.
/// </summary>
public class RNGManager : MonoBehaviour
{
    public static RNGManager Instance { get; private set; }

    [Header("Testing & Debug (Evaluator Controls)")]
    [Tooltip("If true, forces a winning combination for testing.")]
    public bool forceWin = false;
    public SymbolType forcedSymbol = SymbolType.Seven;

    [Header("RNG Probability Weights")]
    [Range(1, 100)] [SerializeField] private int cherryWeight = 40; // Common
    [Range(1, 100)] [SerializeField] private int bellWeight = 30;   // Medium
    [Range(1, 100)] [SerializeField] private int barWeight = 20;    // High
    [Range(1, 100)] [SerializeField] private int sevenWeight = 10;  // Rare (Jackpot)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Generates target symbols for the 3 reels.
    /// </summary>
    public SymbolType[] GenerateSpinOutcome()
    {
        SymbolType[] outcome = new SymbolType[3];

        if (forceWin)
        {
            outcome[0] = forcedSymbol;
            outcome[1] = forcedSymbol;
            outcome[2] = forcedSymbol;
            Debug.Log($"<color=green>[RNG TEST MODE] Forced 3x {forcedSymbol} WIN!</color>");
            return outcome;
        }

        for (int i = 0; i < 3; i++)
        {
            outcome[i] = GetRandomSymbol();
        }

        Debug.Log($"[RNG Outcome] Reel1: {outcome[0]}, Reel2: {outcome[1]}, Reel3: {outcome[2]}");
        return outcome;
    }

    private SymbolType GetRandomSymbol()
    {
        int total = cherryWeight + bellWeight + barWeight + sevenWeight;
        int roll = Random.Range(0, total);

        if (roll < cherryWeight) return SymbolType.Cherry;
        roll -= cherryWeight;

        if (roll < bellWeight) return SymbolType.Bell;
        roll -= bellWeight;

        if (roll < barWeight) return SymbolType.Bar;

        return SymbolType.Seven;
    }
}