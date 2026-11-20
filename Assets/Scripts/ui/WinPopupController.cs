using UnityEngine;
using TMPro;

/// <summary>
/// Controls the Win Celebration popup using popup.png and Yes_No_Btn.
/// Handles Play Again (YES) and Collect (NO) actions.
/// </summary>
public class WinPopupController : MonoBehaviour
{
    public static WinPopupController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI winTitleText;
    [SerializeField] private TextMeshProUGUI winAmountText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (popupPanel != null) popupPanel.SetActive(false);
    }

    private void Start()
    {
        if (BetManager.Instance != null)
        {
            BetManager.Instance.OnWin += ShowWinPopup;
        }
    }

    private void OnDestroy()
    {
        if (BetManager.Instance != null)
        {
            BetManager.Instance.OnWin -= ShowWinPopup;
        }
    }

    public void ShowWinPopup(int winAmount, SymbolType symbol)
    {
        if (popupPanel == null) return;

        string title = "BIG WIN!";
        if (symbol == SymbolType.Seven) title = "JACKPOT!";
        else if (symbol == SymbolType.Bar) title = "MEGA WIN!";

        if (winTitleText != null) winTitleText.text = title;
        if (winAmountText != null) winAmountText.text = $"+{winAmount}G";

        popupPanel.SetActive(true);
    }

    /// <summary>
    /// YES Button: Closes popup and spins again!
    /// </summary>
        /// <summary>
    /// YES Button: Closes popup and spins again with the same bet amount!
    /// </summary>
    public void OnYesPlayAgain()
    {
        ClosePopup();

        // Same bet amount ke sath dobara spin karega!
        if (BetManager.Instance != null)
        {
            BetManager.Instance.SpinWithCurrentBet();
        }
    }

    /// <summary>
    /// NO Button: Just closes popup and stays on screen.
    /// </summary>
    public void OnNoCollect()
    {
        ClosePopup();
    }

    public void ClosePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}