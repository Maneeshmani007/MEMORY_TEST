using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardId;
    public Image cardImage;
    public gameManagerCard gameManager;

    [SerializeField] float flipDuration = 0.45f;

    bool isFlipped;
    bool isAnimating;
    bool isInInitialReveal;

    void Start()
    {
        isFlipped = false;
        isAnimating = false;
        isInInitialReveal = true;

        transform.localScale = new Vector3(-1f, 1f, 1f);
        cardImage.sprite = gameManager.cardback;

        StartCoroutine(InitialReveal());
    }

    IEnumerator InitialReveal()
    {
        // 🔹 Staggered open (wave)
        float openDelay = cardId * 0.08f;   // controls wave speed
        yield return new WaitForSeconds(openDelay);

        isAnimating = true;
        FlipVisual(true);
        cardImage.sprite = gameManager.cardfaces[cardId];

        // 🔹 Keep all cards open for a moment
        yield return new WaitForSeconds(1.0f);

        // 🔹 Staggered close (reverse wave)
        float closeDelay = (gameManager.totalCards - cardId - 1) * 0.06f;
        yield return new WaitForSeconds(closeDelay);

        FlipVisual(false);
        cardImage.sprite = gameManager.cardback;

        isFlipped = false;
        isAnimating = false;
        isInInitialReveal = false;
    }


    // Button calls this
    public void Flippedcard()
    {
        Debug.Log($"CLICK {cardId}");

        if (isInInitialReveal) return;
        if (isAnimating) return;
        if (isFlipped) return;
        if (gameManager.firstcard && gameManager.secondCard) return;

        GameplayFlip();
    }

    void GameplayFlip()
    {
        isAnimating = true;
        isFlipped = true;

        FlipVisual(true);
        cardImage.sprite = gameManager.cardfaces[cardId];

        gameManager.CardFlipped(this);
    }

    public void HideCard()
    {
        isAnimating = true;
        isFlipped = false;

        FlipVisual(false);
        cardImage.sprite = gameManager.cardback;
    }

    void FlipVisual(bool show)
    {
        float x = show ? 1f : -1f;

        transform.DOScaleX(x, flipDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => isAnimating = false);
    }
}
