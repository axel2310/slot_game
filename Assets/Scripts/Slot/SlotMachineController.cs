using System.Collections;
using UnityEngine;

/// <summary>
/// Master controller for the slot machine.
/// Pure button-driven spin lifecycle and payout evaluation.
/// </summary>
public class SlotMachineController : MonoBehaviour
{
    public static SlotMachineController Instance { get; private set; }

    [Header("Reel References")]
    [SerializeField] private Reel reel1;
    [SerializeField] private Reel reel2;
    [SerializeField] private Reel reel3;

    [Header("Spin Timing")]
    [SerializeField] private float reel1SpinDuration = 1.2f;
    [SerializeField] private float reel2Delay = 0.5f;
    [SerializeField] private float reel3Delay = 0.5f;

    private bool isGameSpinning = false;
    public bool IsSpinning => isGameSpinning;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (reel1 != null) reel1.InitializeReel();
        if (reel2 != null) reel2.InitializeReel();
        if (reel3 != null) reel3.InitializeReel();
    }

    /// <summary>
    /// Called after bet has been placed and lever has been pulled.
    /// </summary>
    public void TriggerSpinAfterBet()
    {
        if (isGameSpinning) return;
        StartCoroutine(SpinSequenceRoutine());
    }

    private IEnumerator SpinSequenceRoutine()
    {
        isGameSpinning = true;

        // 1. Get RNG outcome
        SymbolType[] outcomes = RNGManager.Instance.GenerateSpinOutcome();

        // 2. Start spinning
        reel1.StartSpin();
        reel2.StartSpin();
        reel3.StartSpin();

        yield return new WaitForSeconds(reel1SpinDuration);

        // 3. Stop Reel 1
        bool r1Done = false;
        reel1.StopSpin(outcomes[0], () => r1Done = true);
        yield return new WaitForSeconds(reel2Delay);

        // 4. Stop Reel 2
        bool r2Done = false;
        reel2.StopSpin(outcomes[1], () => r2Done = true);
        yield return new WaitForSeconds(reel3Delay);

        // 5. Stop Reel 3
        bool r3Done = false;
        reel3.StopSpin(outcomes[2], () => r3Done = true);

        yield return new WaitUntil(() => r1Done && r2Done && r3Done);

        isGameSpinning = false;

        // 6. Calculate Payout
        EvaluateOutcome(outcomes);
    }

    private void EvaluateOutcome(SymbolType[] outcomes)
    {
        if (BetManager.Instance != null)
        {
            int winAmount = BetManager.Instance.CalculatePayout(outcomes);
            if (winAmount == 0)
            {
                Debug.Log("<color=cyan>[NO WIN] Try again!</color>");
            }
        }
    }
}