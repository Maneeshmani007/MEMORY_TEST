using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Data")]
    public int cardId;            // pair id
    public int cardIndex;         // visual index (optional but kept)
    public Image cardImage;
    public gameManagerCard gameManager;
    public bool IsRestoring { get; set; }

    [Header("Animation")]
    [SerializeField] float flipDuration = 0.45f;

    // ───────── INTERNAL STATE ─────────
    public bool isFlipped;
    bool isAnimating;
    bool isInInitialReveal;
    public bool isMatched;

    // ───────── SAVE SYSTEM ACCESS ─────────
    public bool IsFlipped => isFlipped;
    //public bool IsMatched => isMatched;
    //public bool IsMatched = false;

    void Start()
    {
        // If already restored by load, DO NOTHING
        if (isMatched == true)
            return;

        isFlipped = false;
        isAnimating = false;
        isInInitialReveal = true;

        transform.localScale = new Vector3(-1f, 1f, 1f);
        cardImage.sprite = gameManager.cardback;

        StartCoroutine(InitialReveal());
    }




    // ───────── INITIAL REVEAL ─────────
    IEnumerator InitialReveal()
    {
        // OPEN WAVE (based on hierarchy index – SAFE)
        int index = transform.GetSiblingIndex();
        float openDelay = index * 0.08f;
        yield return new WaitForSeconds(openDelay);

        isAnimating = true;
        FlipVisual(true);
        cardImage.sprite = GetFaceSprite();

        // HOLD OPEN
        yield return new WaitForSeconds(1.0f);

        // CLOSE WAVE (reverse order)
        int total = transform.parent.childCount;
        float closeDelay = (total - index - 1) * 0.06f;
        yield return new WaitForSeconds(closeDelay);

        FlipVisual(false);
        cardImage.sprite = gameManager.cardback;

        isFlipped = false;
        isAnimating = false;
        isInInitialReveal = false;
    }

    // ───────── INPUT ─────────
    public void Flippedcard()
    {
        if (isInInitialReveal) return;
        if (isAnimating) return;
        if (isFlipped) return;
        if (isMatched) return;
        if (gameManager.firstcard && gameManager.secondCard) return;

        GameplayFlip();
    }

    void GameplayFlip()
    {
        isAnimating = true;
        isFlipped = true;

        FlipVisual(true);
        cardImage.sprite = GetFaceSprite();

        gameManager.CardFlipped(this);
    }

    // ───────── GAMEPLAY ACTIONS ─────────
    public void HideCard(bool instant = false)
    {
        isFlipped = false;

        if (instant)
        {
            transform.DOKill();
            transform.localScale = new Vector3(-1f, 1f, 1f);
            cardImage.sprite = gameManager.cardback;
            isAnimating = false;
            return;
        }

        isAnimating = true;
        FlipVisual(false);
        cardImage.sprite = gameManager.cardback;
    }

    public void SetMatched()
    {
        isMatched = true;
        isFlipped = true;

        transform.DOKill();

        cardImage.enabled = true;
        cardImage.sprite = GetFaceSprite();
        transform.localScale = new Vector3(1f, 1f, 1f);

        GetComponent<Button>().interactable = false;

    }


    //public void SetMatched()
    //{
    //    isMatched = true;
    //    isFlipped = false;

    //    transform.DOKill();
    //    cardImage.enabled = false;
    //    GetComponent<Button>().interactable = false;
    //}

    // ───────── SAVE / LOAD RESTORE ─────────
    public void RestoreInstant(bool flipped, bool matched)
    {
        transform.DOKill();

        isAnimating = false;
        isInInitialReveal = false;

        isFlipped = flipped;
        isMatched = matched;

        // Matched cards should stay OPEN
        if (matched)
        {
            cardImage.enabled = true;
            cardImage.sprite = GetFaceSprite();
            transform.localScale = new Vector3(1f, 1f, 1f);
            GetComponent<Button>().interactable = false;
            return;
        }

        // Non-matched cards
        cardImage.enabled = true;
        GetComponent<Button>().interactable = true;

        transform.localScale = new Vector3(flipped ? 1f : -1f, 1f, 1f);
        cardImage.sprite = flipped ? GetFaceSprite() : gameManager.cardback;
    }



    //public void RestoreInstant(bool flipped, bool matched)
    //{
    //    transform.DOKill();

    //    isAnimating = false;
    //    isInInitialReveal = false;

    //    isFlipped = flipped;
    //    isMatched = matched;

    //    if (matched)
    //    {
    //        cardImage.enabled = false;
    //        GetComponent<Button>().interactable = false;
    //        return;
    //    }

    //    cardImage.enabled = true;
    //    GetComponent<Button>().interactable = true;

    //    transform.localScale = new Vector3(flipped ? 1f : -1f, 1f, 1f);
    //    cardImage.sprite = flipped ? GetFaceSprite() : gameManager.cardback;
    //}

    // ───────── VISUAL FLIP ─────────
    void FlipVisual(bool show)
    {
        float x = show ? 1f : -1f;

        transform.DOScaleX(x, flipDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => isAnimating = false);
    }

    // ───────── SAFE SPRITE ACCESS ─────────
    Sprite GetFaceSprite()
    {
        if (gameManager.cardfaces == null || gameManager.cardfaces.Length == 0)
        {
            Debug.LogError("Card faces not assigned!");
            return null;
        }

        int index = cardId % gameManager.cardfaces.Length;
        return gameManager.cardfaces[index];
    }

    public void Initialize(int a_id, Sprite a_cardSprite)
    {
        cardId = a_id;
        cardImage.sprite = a_cardSprite;
        isFlipped = false;
        isMatched = false;
    }

}
