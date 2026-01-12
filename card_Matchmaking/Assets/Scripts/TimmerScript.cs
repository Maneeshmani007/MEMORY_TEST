using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimmerScript : MonoBehaviour
{
    [SerializeField] private float Countdown = 10f;
    [SerializeField] private bool IsTimmerRunning = false;
    public TextMeshProUGUI timmer;
    private void Awake()
    {
        
    }

    private void Start()
    {
        Countdown = gameManagerCard.Instance.maxtime;
        StartCOuntdown();

    }
    public void StartCOuntdown()
    {
        if (!IsTimmerRunning)
        {
            //StartCoroutine(coountDown());
        }
    }

    IEnumerator coountDown()
    {
        IsTimmerRunning = true;
        float countDowntimmer = Countdown;
        
        while (countDowntimmer > 0)
        {
          
            yield return new WaitForSeconds(1f);
            countDowntimmer--;
            timmer.text = countDowntimmer.ToString();
            Debug.Log(countDowntimmer);
        }

        IsTimmerRunning =false;  

    }



}
