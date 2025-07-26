using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    public int cardId;
    public Sprite cardFront;
    public Sprite cardBack;

    [Header("UI References")]
    public Image cardImage;
    public Button cardButton;

    [Header("Animation Settings")]
    public float flipDuration = 0.3f;
    public AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isFlipping = false;


    void Start()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
        }

        // Set initial state
        cardImage.sprite = cardBack;
    }

    public void OnCardClicked()
    {
        if (isFlipping || isMatched || isFlipped)
            return;

        FlipCard();
    }

    [ContextMenu("Flip")]
    public void FlipCard()
    {
        if (isFlipping) return;

        StartCoroutine(FlipCardCoroutine());
    }

    private IEnumerator FlipCardCoroutine()
    {
        isFlipping = true;

        // Flip to show front
        if (!isFlipped)
        {
            yield return StartCoroutine(FlipAnimation(cardFront));
            isFlipped = true;
        }
        else
        {
            yield return StartCoroutine(FlipAnimation(cardBack));
            isFlipped = false;
        }

        isFlipping = false;
    }

    private IEnumerator FlipAnimation(Sprite toSprite)
    {
        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / flipDuration;
            float curveValue = flipCurve.Evaluate(progress);

            // Scale effect for flip animation
            transform.localScale = new Vector3(1f - curveValue * 0.1f, 1f, 1f);

            yield return null;
        }

        // Change sprite at the middle of animation
        cardImage.sprite = toSprite;

        // Reset scale
        transform.localScale = Vector3.one;
    }

    [ContextMenu("Match")]
    public void SetMatched()
    {
        isMatched = true;
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
    }

    [ContextMenu("Reset")]
    public void ResetCard()
    {
        isFlipped = false;
        isMatched = false;
        isFlipping = false;
        cardButton.interactable = true;
        cardImage.sprite = cardBack;
        cardImage.color = Color.white;
        transform.localScale = Vector3.one;
    }
    
    public int GetCardId() => cardId;
}

//TODO : Remove Helper ContextMenu