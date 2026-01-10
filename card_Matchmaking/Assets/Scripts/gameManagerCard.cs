using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using UnityEngine.UIElements.Image;



[System.Serializable]
public class CardState
{
    public int id;
    public bool isMatched;
    public bool isFliped;
}

[System.Serializable]
public class GameData
{
    public int rows;
    public int columns;
    public int score;
    public int tries;
    public int matchedPairs;
    public int totalPairs;
    public float timer;
    public List<CardState> cards;
}


public class gameManagerCard : MonoBehaviour
{
    public static gameManagerCard Instance;
    [SerializeField] private GridLayoutGroup gridLayout;
    [Header("Card Settings")]
    public Card prefabcard;
    public Sprite cardback;
    public Sprite[] cardfaces;

    [Header("Grid Settings")]
    public Transform cardHolder;
    public int rows = 2;
    public int columns = 2;
    public Vector2 spacing = new Vector2(10, 10);
    public int totalCards;


    [Header("UI Elements")]
    public GameObject FinalUi;
    public TextMeshProUGUI TimmerText;
    public TextMeshProUGUI Finaltext;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TriesText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI MatchedText; // NEW — matched pairs UI

    [Header("Game Settings")]
    public float maxtime = 60f;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;

    [HideInInspector] public Card firstcard, secondCard;

    private List<Card> Cards;
    private List<int> cardIds;
    private int Pairsmatched;
    private int Totalpairs;
    public float Timmer;
    private int score;
    private int tries;
    private bool isGameover;
    private bool isGamefinished;
    public bool ispause;


    private Vector2 lastScreenSize;
    private int lastChildCount;

    public GameObject PauseScreen;
    public GameObject Gamewon;
    public int timmerBonous = 5;
    private const string SaveKeyScore = "CardGame_Score";
    private const string SaveKeyTime = "CardGame_Time";
    private const string SaveKeyHighScore = "CardGame_HighScore";
    //[SerializeField] private GridLayoutGroup gridLayout;
    private Vector2 baseSpacing;
    string SavePath => Application.persistentDataPath + "/memory_save.json";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        baseSpacing = gridLayout.spacing; // store inspector spacing ONCE
    }

    void Start()
    {
        // 1️⃣ Initialize lists
        Cards = new List<Card>();
        cardIds = new List<int>();

        // 2️⃣ Reset game values (fresh start)
        score = 0;
        tries = 0;
        Pairsmatched = 0;
        Timmer = maxtime;

        isGamefinished = false;
        isGameover = false;

        // 3️⃣ Safety check
        if (cardHolder == null)
        {
            Debug.LogError("cardHolder missing");
            return;
        }

        // 4️⃣ Setup GridLayout
        gridLayout = cardHolder.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = cardHolder.gameObject.AddComponent<GridLayoutGroup>();

        gridLayout.spacing = spacing;

        // 5️⃣ ALWAYS CREATE A NEW GAME


        // 6️⃣ UI reset
        FinalUi.SetActive(false);
        Finaltext.gameObject.SetActive(false);

        UpdateScoreUI();
        UpdateTriesUI();
        UpdateMatchedUI();

        totalCards = Cards.Count;
    }

    void SetGridConstraint(int rows, int cols)
    {
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = cols;

        // 🔥 FORCE UI TO APPLY CHANGE
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            gridLayout.GetComponent<RectTransform>()
        );
    }


    public void Initializecard(int rows, int columns)
    {
        //SetupGrid(rows, columns);
        //ClearGrid();
        SetupGrid(rows, columns);
        CreateCard(rows * columns);
        SetGridConstraint(rows, columns);
    }
    




    public void onclickHome()
    {
        foreach (Card card in Cards)
        {
            Destroy(card.gameObject);
        }
        Cards.Clear();
        Time.timeScale = 1;
    }

    public void OnClickSave()
    {
        GameData data = new GameData
        {
            rows = rows,
            columns = columns,
            score = score,
            tries = tries,
            totalPairs = Totalpairs,
            matchedPairs = Pairsmatched,
            timer = Timmer,
            cards = new List<CardState>()
        };

        foreach (Card c in Cards)
        {
            data.cards.Add(new CardState
            {
                id = c.cardId,
                isMatched = c.isMatched
            });
        }

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString("SavedGame", json);
        PlayerPrefs.Save();

        Debug.Log("Game Saved");
    }


    public void OnClickLoad()
    {
        if (!PlayerPrefs.HasKey("SavedGame"))
        {
            Debug.Log("No saved game found");
            return;
        }

        string json = PlayerPrefs.GetString("SavedGame");
        GameData data = JsonUtility.FromJson<GameData>(json);

        // 1️⃣ CLEAR CURRENT GRID
        foreach (Card c in Cards)
            Destroy(c.gameObject);

        Cards.Clear();
        cardIds.Clear();

        // 2️⃣ RESTORE CORE DATA
        rows = data.rows;
        columns = data.columns;
        score = data.score;
        tries = data.tries;
        Pairsmatched = data.matchedPairs;
        Timmer = data.timer;
        Totalpairs = data.totalPairs;

        //Pairsmatched = 0;

        // 3️⃣ SETUP GRID
        //SetupGrid(rows, columns);
        SetupGrid(rows, columns);



        for (int i = 0; i < data.cards.Count; i++)
        {
            CardState state = data.cards[i];

            Card card = Instantiate(prefabcard, cardHolder);
            card.IsRestoring = true;                // 🔑 KEY LINE
            card.cardId = state.id;
            card.cardIndex = i;
            card.gameManager = this;
            card.cardImage.sprite = cardback;

            Cards.Add(card);
        }

        //Pairsmatched = 0;

        for (int i = 0; i < Cards.Count; i++)
        {
            CardState state = data.cards[i];
            Card card = Cards[i];

            if (state.isMatched)
            {
                card.SetMatched();        // ✅ stays OPEN
                //Pairsmatched++;
            }
        }







        // 6️⃣ UPDATE UI
        UpdateScoreUI();
        UpdateTriesUI();
        UpdateMatchedUI();

        Debug.Log("Game Loaded");
    }





    void SetRandomRowsAndColumns()
    {
        //rows = Random.Range(2, 5);
        //columns = Random.Range(3, 5);
    }

    void Update()
    {
        if (!isGamefinished && !isGameover)
        {
            if (Timmer > 0)
            {
                Timmer -= Time.deltaTime;
                UpdateTimmerText();
            }
            else
            {
                Gameover();
            }
        }

        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || cardHolder.childCount != lastChildCount)
        {
            SetupGrid(rows, columns);
            //OnGridSelected(rows, columns);
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            lastChildCount = cardHolder.childCount;
        }
    }

    public void ClearGrid()
    {
        // Stop any running grid resize coroutine
        StopAllCoroutines();

        // Destroy all card instances
        for (int i = cardHolder.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cardHolder.transform.GetChild(i).gameObject);
        }

        // Optional safety reset (does NOT break layout)
        gridLayout.cellSize = Vector2.zero;
    }

    public void OnGridSelected(int rows, int cols)
    {
        StopAllCoroutines();

        ClearGrid();              // destroy old cards
        //SetupGrid(rows, cols);   // MUST spawn rows * cols cards
        CreateCard(rows * columns);

        StartCoroutine(SetupGrid(rows, cols)); // 🔥 correct place
    }




    IEnumerator SetupGrid(int rows, int cols)
    {
        // 🔒 Wait until ALL cards are spawned & layout is ready
        yield return new WaitForEndOfFrame();


        RectTransform holder = cardHolder.GetComponent<RectTransform>();
        if (!holder) yield break;

        GridLayoutGroup grid = gridLayout;

        float holderWidth = holder.rect.width;
        float holderHeight = holder.rect.height;

        // ----------------------------
        // 1. FORCE COLUMN CONSTRAINT
        // ----------------------------

        //grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //grid.constraintCount = cols;

        // ----------------------------
        // 2. AUTO-SCALED SPACING (NON-CUMULATIVE)
        // ----------------------------
        float density = Mathf.Max(rows, cols);
        float spacingFactor = Mathf.Clamp01(6f / density);
        Vector2 finalSpacing = baseSpacing * spacingFactor;
        grid.spacing = finalSpacing;

        // ----------------------------
        // 3. AVAILABLE SPACE
        // ----------------------------
        float paddingX = grid.padding.left + grid.padding.right;
        float paddingY = grid.padding.top + grid.padding.bottom;

        float availableWidth =
            holderWidth - paddingX - finalSpacing.x * (cols - 1);

        float availableHeight =
            holderHeight - paddingY - finalSpacing.y * (rows - 1);

        // ----------------------------
        // 4. CELL SIZE (FIT BOTH AXES)
        // ----------------------------
        float cellWidth = availableWidth / cols;
        float cellHeight = availableHeight / rows;

        float aspect = Mathf.Max(0.01f, GetCardAspect());

        if (cellWidth / aspect > cellHeight)
            cellWidth = cellHeight * aspect;
        else
            cellHeight = cellWidth / aspect;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.childAlignment = TextAnchor.MiddleCenter;

        // ----------------------------
        // 5. FORCE CHILD SIZE (SAFE)
        // ----------------------------
        foreach (RectTransform child in grid.transform)
        {
            child.sizeDelta = grid.cellSize;
            child.localScale = Vector3.one;
        }

        SetGridConstraint(rows, cols);
        // ----------------------------
        // 6. FINAL REBUILD (ONCE)
        // ----------------------------
        Canvas.ForceUpdateCanvases();
    }


    //void SetupGrid(int rows, int cols)
    //{
    //    if (cardHolder == null || gridLayout == null) return;

    //    RectTransform holderRect = cardHolder.GetComponent<RectTransform>();
    //    if (holderRect == null) return;

    //    float totalSpacingX = spacing.x * (cols - 1);
    //    float totalSpacingY = spacing.y * (rows - 1);

    //    float maxCellWidth = (holderRect.rect.width - totalSpacingX - gridLayout.padding.left - gridLayout.padding.right) / Mathf.Max(1, cols);
    //    float maxCellHeight = (holderRect.rect.height - totalSpacingY - gridLayout.padding.top - gridLayout.padding.bottom) / Mathf.Max(1, rows);

    //    float aspect = GetCardAspect();
    //    if (aspect <= 0f) aspect = 1f;

    //    float finalCellWidth = maxCellWidth;
    //    float finalCellHeight = finalCellWidth / aspect;

    //    if (finalCellHeight > maxCellHeight)
    //    {
    //        finalCellHeight = maxCellHeight;
    //        finalCellWidth = finalCellHeight * aspect;
    //    }

    //    gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    //    gridLayout.constraintCount = cols;
    //    gridLayout.cellSize = new Vector2(finalCellWidth, finalCellHeight);
    //}

    float GetCardAspect()
    {
        if (prefabcard != null && prefabcard.cardImage != null && prefabcard.cardImage.sprite != null)
        {
            Sprite s = prefabcard.cardImage.sprite;
            if (s.rect.height != 0) return s.rect.width / s.rect.height;
        }

        if (cardfaces != null && cardfaces.Length > 0 && cardfaces[0] != null)
        {
            Sprite s = cardfaces[0];
            if (s.rect.height != 0) return s.rect.width / s.rect.height;
        }

        //Image img = prefabcard != null ? prefabcard.GetComponentInChildren<Image>() : null;

        UnityEngine.UI.Image img = prefabcard != null ? prefabcard.GetComponentInChildren<UnityEngine.UI.Image>() : null;

        if (img != null && img.sprite != null && img.sprite.rect.height != 0)
            return img.sprite.rect.width / img.sprite.rect.height;

        RectTransform rt = prefabcard != null ? prefabcard.GetComponent<RectTransform>() : null;
        if (rt != null && rt.rect.height != 0)
            return rt.rect.width / rt.rect.height;

        return 1f;
    }

    void CreateCard(int totalCards)
    {
        Totalpairs = Mathf.Max(1, totalCards / 2);
        Pairsmatched = 0;
        UpdateMatchedUI(); // NEW — reset match count

        cardIds.Clear();

        for (int i = 0; i < Totalpairs; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        Shuffle(cardIds);

        foreach (int id in cardIds)
        {
            Card newCard = Instantiate(prefabcard, cardHolder);
            newCard.gameManager = this;
            newCard.cardId = id;
            if (newCard.cardImage != null && cardback != null)
                newCard.cardImage.sprite = cardback;
            Cards.Add(newCard);
        }
        SetGridConstraint(rows, columns);
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //int rnd = Random.Range(0, list.Count);
            int rnd = UnityEngine.Random.Range(0, list.Count);

            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    public void CardFlipped(Card flippedCard)
    {
        PlaySound(flipSound);

        if (firstcard == null)
        {
            firstcard = flippedCard;
        }
        else if (secondCard == null)
        {
            secondCard = flippedCard;

            tries++;
            UpdateTriesUI();

            CheckMatch();
        }
    }

    void CheckMatch()
    {
        if (firstcard == null || secondCard == null) return;

        if (firstcard.cardId == secondCard.cardId)
        {
            score += 10;
            //timmerBonous = +5;
            Timmer += timmerBonous;
            UpdateScoreUI();
            SaveProgress();
            PlaySound(matchSound);

            Pairsmatched++;
            UpdateMatchedUI(); // NEW — refresh matched UI
            firstcard.isMatched = true;
            secondCard.isMatched = true;
            firstcard = null;
            secondCard = null;
            //SaveGame();
            if (Pairsmatched == Totalpairs)
                LevelFinished();
        }
        else
        {
            score -= 2;
            UpdateScoreUI();
            SaveProgress();
            PlaySound(mismatchSound);
            StartCoroutine(FlipBackCards());
        }
    }

    IEnumerator FlipBackCards()
    {
        yield return new WaitForSeconds(2f);
        if (firstcard != null) firstcard.HideCard();
        if (secondCard != null) secondCard.HideCard();
        firstcard = null;
        secondCard = null;
    }

    void LevelFinished()
    {
        isGamefinished = true;
        FinalPanel();
    }

    void Gameover()
    {
        isGameover = true;
        PlaySound(gameOverSound);
        FinalPanel();
        TogglePause();
    }

    void FinalPanel()
    {
        FinalUi.SetActive(true);

        int highScore = PlayerPrefs.GetInt(SaveKeyHighScore, 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(SaveKeyHighScore, highScore);
            PlayerPrefs.Save();
        }

        if (HighScoreText != null)
            HighScoreText.text = "High Score: " + highScore;

        //if (isGamefinished)
        //    Finaltext.text = $"Level Finished!\nTime Left: {Mathf.Round(Timmer)}s\nScore: {score}\nTries: {tries}\nMatches: {Pairsmatched}/{Totalpairs}";
        //else if (isGameover)
        //    Finaltext.text = $"GAME OVER!\nScore: {score}\nTries: {tries}\nMatches: {Pairsmatched}/{Totalpairs}";
        if (isGamefinished)
        {

            Gamewon.SetActive(true);
            PauseScreen.SetActive(false);
            Finaltext.transform.gameObject.SetActive(false);
        }
        else if (isGameover)
        {
            Finaltext.transform.gameObject.SetActive(true);
            Gamewon.SetActive(false);
            PauseScreen.SetActive(false);
            Finaltext.text = $"GAME OVER!\nScore: {score}\nTries: {tries}\nMatches: {Pairsmatched}/{Totalpairs}";
        }
    }

    void UpdateTimmerText()
    {
        if (TimmerText != null)
            TimmerText.text = "Time Left: " + Mathf.Round(Timmer) + "s";
    }

    void UpdateScoreUI()
    {
        if (ScoreText != null)
            ScoreText.text = "Score: " + score;
    }

    void UpdateTriesUI()
    {
        if (TriesText != null)
            TriesText.text = "Tries: " + tries;
    }

    void UpdateMatchedUI() // NEW
    {
        if (MatchedText != null)
            MatchedText.text = "Matches: " + Pairsmatched + "/" + Totalpairs;
    }

    public void Restart()
    {
        TogglePause();
        Time.timeScale = 1;
        Pairsmatched = 0;
        Timmer = maxtime;
        score = 0;
        tries = 0;
        isGameover = false;
        isGamefinished = false;
        FinalUi.SetActive(false);

        foreach (var card in Cards)
            if (card != null) Destroy(card.gameObject);

        Cards.Clear();
        SetRandomRowsAndColumns();
        SetupGrid(rows, columns);
        //OnGridSelected(rows, columns);
        CreateCard(rows * columns);

        UpdateScoreUI();
        UpdateTriesUI();
        UpdateMatchedUI(); // NEW
        SaveProgress();

    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(SaveKeyScore, score);
        PlayerPrefs.SetFloat(SaveKeyTime, Timmer);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        score = PlayerPrefs.GetInt(SaveKeyScore, 0);
        Timmer = PlayerPrefs.GetFloat(SaveKeyTime, maxtime);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("Quit called - Game would close in a build.");
        UnityEditor.EditorApplication.isPlaying = false;
        //SaveGame();
#else
    if (Application.platform == RuntimePlatform.Android)
    {
        Debug.Log("Closing on Android...");
        Application.Quit();
        //SaveGame();
    }
    else if (Application.platform == RuntimePlatform.WindowsPlayer)
    {
        Debug.Log("Closing on Windows...");
        Application.Quit();
        //SaveGame();
    }
    else
    {
        Debug.Log("Quit called on: " + Application.platform);
        Application.Quit();
        //SaveGame();
    }
#endif
    }

    public void TogglePause()
    {
        ispause = !ispause;

        if (ispause)
        {
            Time.timeScale = 0f;  // Stop game updates
        }
        else
        {
            Time.timeScale = 1f;  // Resume game updates
        }
    }


    public void pause()
    {
        FinalUi.SetActive(true);
        Finaltext.transform.gameObject.SetActive(false);
        PauseScreen.SetActive(true);
        Finaltext.transform.gameObject.SetActive(false);
        TogglePause();
    }

}
