using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI loadingText;          // Assign in Inspector
    public float dotDelay = 1f;        // 1 second delay
    public int totalLoadingTime = 4;   // Seconds before loading next scene
    public GameObject CurrentScene;
    public GameObject nextSceneName;

    void Start()
    {
        StartCoroutine(LoadingRoutine());
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

        nextSceneName.SetActive(true);
        CurrentScene.SetActive(false);
    }
}
