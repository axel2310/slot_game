using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls an individual reel: spinning, infinite scrolling, 
/// target landing, and realistic bounce-back easing.
/// </summary>
public class Reel : MonoBehaviour
{
    [Header("Reel Configuration")]
    [Tooltip("Prefab containing an Image component to represent each symbol.")]
    [SerializeField] private GameObject symbolPrefab;
    [SerializeField] private float symbolHeight = 110f;
    [SerializeField] private int totalSymbolsOnReel = 5; // 3 visible + 2 buffer for looping

    [Header("Speed & Easing Settings")]
    [SerializeField] private float maxSpinSpeed = 2200f;
    [SerializeField] private float accelerationTime = 0.3f;
    [SerializeField] private float bounceOvershoot = 25f;
    [SerializeField] private float bounceDuration = 0.25f;

    [Header("Symbol Sprites")]
    [SerializeField] private Sprite cherrySprite; // slot-symbol1
    [SerializeField] private Sprite sevenSprite;  // slot-symbol2
    [SerializeField] private Sprite bellSprite;   // slot-symbol3
    [SerializeField] private Sprite barSprite;    // slot-symbol4

    // Runtime state
    private List<RectTransform> activeSymbolTransforms = new List<RectTransform>();
    private List<Image> activeSymbolImages = new List<Image>();
    private List<SymbolType> activeSymbolTypes = new List<SymbolType>();

    private bool isSpinning = false;
    private float currentSpeed = 0f;
    private SymbolType targetSymbol;

    public bool IsSpinning => isSpinning;

    public void InitializeReel()
    {
        // Clear any previous editor child objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        activeSymbolTransforms.Clear();
        activeSymbolImages.Clear();
        activeSymbolTypes.Clear();

        // Spawn symbols: centered at y = 0, with symbols above and below
        int half = totalSymbolsOnReel / 2;
        for (int i = -half; i <= half; i++)
        {
            GameObject symbolObj = Instantiate(symbolPrefab, transform);
            RectTransform rt = symbolObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, i * symbolHeight);
            
            Image img = symbolObj.GetComponent<Image>();
            SymbolType randomType = (SymbolType)UnityEngine.Random.Range(0, 4);
            
            img.sprite = GetSpriteForType(randomType);

            activeSymbolTransforms.Add(rt);
            activeSymbolImages.Add(img);
            activeSymbolTypes.Add(randomType);
        }
    }

    /// <summary>
    /// Starts reel spinning with acceleration.
    /// </summary>
    public void StartSpin()
    {
        if (isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    /// <summary>
    /// Commands the reel to stop on the desired target symbol at center.
    /// </summary>
    public void StopSpin(SymbolType desiredSymbol, Action onComplete)
    {
        StartCoroutine(StopRoutine(desiredSymbol, onComplete));
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;
        currentSpeed = 0f;

        // Acceleration phase
        float elapsed = 0f;
        while (elapsed < accelerationTime)
        {
            elapsed += Time.deltaTime;
            currentSpeed = Mathf.Lerp(0, maxSpinSpeed, elapsed / accelerationTime);
            MoveSymbols(currentSpeed * Time.deltaTime);
            yield return null;
        }

        currentSpeed = maxSpinSpeed;

        // Full speed loop
        while (isSpinning)
        {
            MoveSymbols(currentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void MoveSymbols(float deltaDistance)
    {
        float bottomLimit = -(totalSymbolsOnReel / 2 + 1) * symbolHeight;
        float topRespawnY = (totalSymbolsOnReel / 2) * symbolHeight;

        for (int i = 0; i < activeSymbolTransforms.Count; i++)
        {
            RectTransform rt = activeSymbolTransforms[i];
            rt.anchoredPosition -= new Vector2(0, deltaDistance);

            // Wrap around if went past bottom
            if (rt.anchoredPosition.y < bottomLimit)
            {
                // Find highest current symbol
                float highestY = float.MinValue;
                for (int j = 0; j < activeSymbolTransforms.Count; j++)
                {
                    if (activeSymbolTransforms[j].anchoredPosition.y > highestY)
                        highestY = activeSymbolTransforms[j].anchoredPosition.y;
                }

                rt.anchoredPosition = new Vector2(0, highestY + symbolHeight);

                // Give it a fresh random symbol during fast spin
                SymbolType randType = (SymbolType)UnityEngine.Random.Range(0, 4);
                activeSymbolImages[i].sprite = GetSpriteForType(randType);
                activeSymbolTypes[i] = randType;
            }
        }
    }

    private IEnumerator StopRoutine(SymbolType desiredSymbol, Action onComplete)
    {
        this.targetSymbol = desiredSymbol;

        // Find the symbol currently moving towards center
        // Align and snap it to exact center (0, 0)
        yield return AlignTargetSymbol(desiredSymbol);

        isSpinning = false;

        // Bounce / Easing animation for physical slot machine feel
        yield return PlayBounceEffect();

        onComplete?.Invoke();
    }

    private IEnumerator AlignTargetSymbol(SymbolType desiredSymbol)
    {
        // Pick the center symbol element (middle index)
        int centerIndex = totalSymbolsOnReel / 2;
        RectTransform centerRt = activeSymbolTransforms[centerIndex];
        Image centerImg = activeSymbolImages[centerIndex];

        // Assign the exact outcome symbol
        centerImg.sprite = GetSpriteForType(desiredSymbol);
        activeSymbolTypes[centerIndex] = desiredSymbol;

        // Also assign adjacent random symbols for visual consistency
        for (int i = 0; i < activeSymbolTransforms.Count; i++)
        {
            if (i != centerIndex)
            {
                SymbolType adjacent = (SymbolType)UnityEngine.Random.Range(0, 4);
                activeSymbolImages[i].sprite = GetSpriteForType(adjacent);
                activeSymbolTypes[i] = adjacent;
            }
        }

        // Smooth deceleration to exact center
        float duration = 0.25f;
        float elapsed = 0f;
        Vector2 startPos = centerRt.anchoredPosition;
        Vector2 targetPos = Vector2.zero;

        // Snap all relative to center
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float currentCenterY = Mathf.Lerp(startPos.y, 0f, t);

            for (int i = 0; i < activeSymbolTransforms.Count; i++)
            {
                int offsetFromCenter = i - centerIndex;
                activeSymbolTransforms[i].anchoredPosition = new Vector2(0, currentCenterY + (offsetFromCenter * symbolHeight));
            }
            yield return null;
        }

        // Ensure perfect snap
        for (int i = 0; i < activeSymbolTransforms.Count; i++)
        {
            int offsetFromCenter = i - centerIndex;
            activeSymbolTransforms[i].anchoredPosition = new Vector2(0, offsetFromCenter * symbolHeight);
        }
    }

    private IEnumerator PlayBounceEffect()
    {
        int centerIndex = totalSymbolsOnReel / 2;
        float halfDuration = bounceDuration / 2f;

        // 1. Overshoot slightly downwards
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float yOffset = Mathf.Lerp(0f, -bounceOvershoot, elapsed / halfDuration);
            ApplyOffset(yOffset, centerIndex);
            yield return null;
        }

        // 2. Spring back to 0
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float yOffset = Mathf.Lerp(-bounceOvershoot, 0f, elapsed / halfDuration);
            ApplyOffset(yOffset, centerIndex);
            yield return null;
        }

        ApplyOffset(0f, centerIndex);
    }

    private void ApplyOffset(float yOffset, int centerIndex)
    {
        for (int i = 0; i < activeSymbolTransforms.Count; i++)
        {
            int offsetFromCenter = i - centerIndex;
            activeSymbolTransforms[i].anchoredPosition = new Vector2(0, (offsetFromCenter * symbolHeight) + yOffset);
        }
    }

    public Sprite GetSpriteForType(SymbolType type)
    {
        switch (type)
        {
            case SymbolType.Cherry: return cherrySprite;
            case SymbolType.Seven:  return sevenSprite;
            case SymbolType.Bell:   return bellSprite;
            case SymbolType.Bar:    return barSprite;
            default: return cherrySprite;
        }
    }
}