using System.Collections;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float matchDelay = 1f;

    [Header("Manager References")]
    public GameProgressManager gameProgressManager;
    public AudioManager audioManager;

    private Card lastFlippedCard = null;

    public void OnCardClicked(Card card)
    {
        if (!card.CanClick()) return;

        audioManager.PlayCardFlipSound();

        card.FlipCard();

        if (lastFlippedCard == null)
        {
            lastFlippedCard = card;
        }
        else
        {
            gameProgressManager.RegisterMove();

            // Copy to local vars to avoid race conditions
            Card first = lastFlippedCard;
            Card second = card;
            lastFlippedCard = null;

            StartCoroutine(CheckPairMatch(first, second));
        }
    }

    private IEnumerator CheckPairMatch(Card card1, Card card2)
    {
        yield return new WaitForSeconds(card2.flipDuration + matchDelay);

        if (card1.CardId == card2.CardId)
        {
            card1.SetMatched();
            card2.SetMatched();
            gameProgressManager.RegisterMatch();
            audioManager.PlayCardMatchSound();

            if (gameProgressManager.IsGameComplete())
            {
                yield return new WaitForSeconds(1f);
                audioManager.PlayGameCompleteSound();
                gameProgressManager.ShowGameCompletePanel();
            }
        }
        else
        {
            gameProgressManager.RegisterMismatch();
            audioManager.PlayCardMismatchSound();

            card1.FlipCard();
            card2.FlipCard();
        }
    }
}