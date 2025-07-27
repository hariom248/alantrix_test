using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum CardVisualState
{
    Hidden,
    Flipped,
    Matched
}


public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    public Sprite cardFront;
    public Sprite cardBack;

    [Header("UI References")]
    public Image cardImage;
    public Button cardButton;

    [Header("Animation Settings")]
    public float flipDuration = 0.3f;
    public AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private bool isFlipping = false;

    public CardVisualState VisualState { get; private set; }

    public int CardId { get; set; }

    void Start()
    {
        // Set initial state
        cardImage.sprite = cardBack;
    }

    public bool CanClick() => !isFlipping && VisualState == CardVisualState.Hidden;

    public void FlipCard()
    {
        if (isFlipping) return;

        StartCoroutine(FlipCardCoroutine());
    }

    private IEnumerator FlipCardCoroutine()
    {
        isFlipping = true;

        // Flip to show front
        if (VisualState == CardVisualState.Hidden)
        {
            yield return StartCoroutine(FlipAnimation(cardFront));
            VisualState = CardVisualState.Flipped;
        }
        else
        {
            yield return StartCoroutine(FlipAnimation(cardBack));
            VisualState = CardVisualState.Hidden;
        }

        isFlipping = false;
    }

    private IEnumerator FlipAnimation(Sprite toSprite)
    {
        float halfDuration = flipDuration / 2f;
        float elapsed = 0f;

        // First half: shrink X to 0
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(1f, 0f, flipCurve.Evaluate(t));
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        // Change sprite at the midpoint
        cardImage.sprite = toSprite;

        elapsed = 0f;

        // Second half: grow X from 0 to 1
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(0f, 1f, flipCurve.Evaluate(t));
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        // Final scale
        transform.localScale = Vector3.one;
    }

    public void SetMatched()
    {
        VisualState = CardVisualState.Matched;
        cardButton.interactable = false;
        // Visual feedback for matched cards
        StartCoroutine(MatchedAnimation());
    }

    private IEnumerator MatchedAnimation()
    {
        // Pulse animation for matched cards
        Vector3 originalScale = transform.localScale;

        for (int i = 0; i < 3; i++)
        {
            transform.localScale = originalScale * 1.1f;
            yield return new WaitForSeconds(0.1f);
            transform.localScale = originalScale;
            yield return new WaitForSeconds(0.1f);
        }

        // Fade out matched cards
        Color originalColor = cardImage.color;
        float fadeTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.3f, elapsedTime / fadeTime);
            cardImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        cardImage.sprite = cardFront;
    }
}