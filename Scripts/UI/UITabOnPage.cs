using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabOnPage : MonoBehaviour
{
    public GameObject[] Page;
    public Button[] Buttons;

    public Sprite ClickImages;
    public Sprite IdleImages;


    public void TabClick(int n)
    {
        for (int i = 0; i < Page.Length; i++)
        {
            if (i == n)
            {
                Page[i].SetActive(true);
                Buttons[i].image.sprite = ClickImages;
            }
            else
            {
                Page[i].SetActive(false);
                Buttons[i].image.sprite = IdleImages;
            }
        }
    }

}
