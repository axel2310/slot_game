using UnityEngine;
using TMPro;

/// <summary>
/// Controls the Win Celebration popup using popup.png and Yes_No_Btn.
/// Plays retro win/jackpot audio and handles Play Again (YES) and Collect (NO) actions.
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

        bool isJackpot = (symbol == SymbolType.Seven);
        string title = "BIG WIN!";
        if (isJackpot) title = "JACKPOT!";
        else if (symbol == SymbolType.Bar) title = "MEGA WIN!";

        if (winTitleText != null) winTitleText.text = title;
        if (winAmountText != null) winAmountText.text = $"+{winAmount}G";

        // Play Win Audio
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWin(isJackpot);
        }

        popupPanel.SetActive(true);
    }

    public void OnYesPlayAgain()
    {
        ClosePopup();

        // Spin again with same bet
        if (BetManager.Instance != null)
        {
            BetManager.Instance.SpinWithCurrentBet();
        }
    }

    public void OnNoCollect()
    {
        ClosePopup();
    }

    public void ClosePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}
