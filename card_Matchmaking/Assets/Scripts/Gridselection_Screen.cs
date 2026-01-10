using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridselection_Screen : MonoBehaviour
{
    [SerializeField] private GameObject Mainmenu;
    public void OnClickTwoByTwo()
    {
        gameManagerCard.Instance.Initializecard(2, 2);
        Mainmenu.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void OnClickTwoByThree()
    {
        gameManagerCard.Instance.Initializecard(2, 3);
        Mainmenu.SetActive(false);
        this.gameObject.SetActive(false);
    }
    public void OnClickFivebySix()
    {
        gameManagerCard.Instance.Initializecard(5, 6);
        Mainmenu.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
