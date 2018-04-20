using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Scores : MonoBehaviour
{
    private GameManager gm;

    public GameData gameData;

    public Image[] playerImages;
    public Text[] playerTexts;

    public void Start()
    {
        gm = GameManager.instance;   

        for (int i=gameData.numberOfPlayers; i<playerImages.Length; i++)
        {
            playerImages[i].gameObject.SetActive(false);
            playerTexts[i].gameObject.SetActive(false);
        }
    }

    public void UpdateText()
    {
        if (gm != null)
        {
            for (int i = 0; i < gameData.numberOfPlayers; i++)
            {
                playerTexts[i].text = gm.GetPlayerScore(i).ToString();
            }
        }
        else
        {
            for (int i = 0; i < gameData.numberOfPlayers; i++)
            {
                playerTexts[i].text = "0";
            }
        }
    }
}
