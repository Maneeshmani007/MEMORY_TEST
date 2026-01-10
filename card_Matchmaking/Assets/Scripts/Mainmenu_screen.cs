using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mainmenu_screen : MonoBehaviour
{
    public GameObject NextSCreen;
    public GameObject CurrentScreen;
    public void OnclickPlayBtn()
    {
        NextSCreen.SetActive(true);
        CurrentScreen.SetActive(false);
    }

    public void OnclickQuitBtn()
    {
        Application.Quit();
    }
}
