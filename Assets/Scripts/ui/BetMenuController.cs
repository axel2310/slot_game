using UnityEngine;

/// <summary>
/// Controls the right-side Bet Menu: toggles open/close on Right-Click
/// and closes when an option or Exit is clicked (exact GIF mechanic).
/// </summary>
public class BetMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject betMenuPanel;

    private void Start()
    {
        
    }

    private void Update()
    {
        // Right Click check (Supports both New and Old Input System)
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null && 
            UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame)
        {
            ToggleMenu();
        }
#else
        if (Input.GetMouseButtonDown(1)) // 1 = Right Mouse Button
        {
            ToggleMenu();
        }
#endif
    }

    public void ToggleMenu()
    {
        if (betMenuPanel != null)
        {
            bool isCurrentlyActive = betMenuPanel.activeSelf;
            betMenuPanel.SetActive(!isCurrentlyActive);
        }
    }

    public void CloseMenu()
    {
        if (betMenuPanel != null)
        {
            betMenuPanel.SetActive(false);
        }
    }

    public void OpenMenu()
    {
        if (betMenuPanel != null)
        {
            betMenuPanel.SetActive(true);
        }
    }
}