using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHpManager : MonoBehaviour
{
    public PlayerControll player;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;


    private void Update()
    {
        ShowHearts();
    }
    public void ShowHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (player.maxHealth / 2 > i)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;

            if (player.currentHealth / 2 > i)
                hearts[i].sprite = fullHeart;
            else if (player.currentHealth / 2 == i && player.currentHealth % 2 == 1)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
