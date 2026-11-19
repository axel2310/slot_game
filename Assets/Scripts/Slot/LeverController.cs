using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visual lever pull animation triggered by Bet Buttons.
/// Direct mouse click is disabled.
/// </summary>
public class LeverController : MonoBehaviour
{
    public static LeverController Instance { get; private set; }

    [Header("Visual Images")]
    [SerializeField] private Image leverUpImage;
    [SerializeField] private Image leverDownImage;

    [Header("Settings")]
    [SerializeField] private float pullHoldTime = 0.2f;

    private bool isPulling = false;
    public bool IsPulling => isPulling;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (leverUpImage != null) leverUpImage.enabled = true;
        if (leverDownImage != null) leverDownImage.enabled = false;
    }

    public void PullLever(Action onPulled = null)
    {
        if (isPulling) return;
        StartCoroutine(PullRoutine(onPulled));
    }

    private IEnumerator PullRoutine(Action onPulled)
    {
        isPulling = true;

        // 1. Lever Down
        if (leverUpImage != null) leverUpImage.enabled = false;
        if (leverDownImage != null) leverDownImage.enabled = true;

        yield return new WaitForSeconds(pullHoldTime);

        // 2. Trigger spin
        onPulled?.Invoke();

        yield return new WaitForSeconds(0.12f);

        // 3. Lever Up
        if (leverDownImage != null) leverDownImage.enabled = false;
        if (leverUpImage != null) leverUpImage.enabled = true;

        isPulling = false;
    }
}