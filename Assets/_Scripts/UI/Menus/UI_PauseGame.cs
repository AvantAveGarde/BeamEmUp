using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_PauseGame : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject defaultPauseGO;

    private Player player;
    
    public void Update()
    {
        foreach (Player p in ReInput.players.Players)
        {
            if (p.GetButtonDown("Middle2") && !pausePanel.activeSelf)
            {
                player = p;
                PuaseGame();
                break;
            }
            else if (player != null && player.GetButtonDown("Middle2"))
            {
                ResumeGame();
            }
        }
    }

    public void PuaseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(defaultPauseGO);
    }

    public void ResumeGame()
    {
        player = null;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("Start Menu");
    }
}
