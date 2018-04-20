using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    public Text timerText;

    private GameManager gm;

    public void Start()
    {
        gm = GameManager.instance;
    }

    public void Update()
    {
        int seconds = (int)(gm.GetTimeLeft() % 60);
        int minutes = (int)(gm.GetTimeLeft() / 60);
        string time = minutes + ":" + seconds;

        timerText.text = time;
    }
}
