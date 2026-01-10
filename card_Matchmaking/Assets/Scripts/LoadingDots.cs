using UnityEngine;
using System.Collections;
using TMPro;

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public float dotDelay = 1f;
    public int totalLoadingTime = 4;

    public GameObject CurrentScene;
    public GameObject nextSceneName;

    Coroutine loadingCoroutine;

    void OnEnable()
    {
        // Reset text immediately
        loadingText.text = "Loading";

        // Stop previous coroutine if any
        if (loadingCoroutine != null)
            StopCoroutine(loadingCoroutine);

        // Start again whenever enabled
        loadingCoroutine = StartCoroutine(LoadingRoutine());
    }

    void OnDisable()
    {
        if (loadingCoroutine != null)
            StopCoroutine(loadingCoroutine);
    }

    IEnumerator LoadingRoutine()
    {
        float timer = 0f;
        int dotCount = 0;

        while (timer < totalLoadingTime)
        {
            dotCount = (dotCount % 3) + 1;
            loadingText.text = "Loading" + new string('.', dotCount);

            yield return new WaitForSeconds(dotDelay);
            timer += dotDelay;
        }

        // Switch UI
        nextSceneName.SetActive(true);
        CurrentScene.SetActive(false);
    }
}
