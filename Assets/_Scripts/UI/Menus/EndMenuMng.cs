using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenuMng : MonoBehaviour
{
    public GameObject defaultSelectedObject;

    public GameData gameData;

    public GameObject[] players;
    public Text[] playerScoreTexts;

    public Image fadeImage;

    public void Awake()
    {
        fadeImage.GetComponent<Animator>().SetTrigger("fadeIn");
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);

        foreach (GameObject go in players)
        {
            go.SetActive(false);
        }
        
        // Find the Winner
        int winnerID = 0;
        int highScore = gameData.playerScores[0];

        for (int i = 0; i <= gameData.numberOfPlayers - 1; i++)
        {
            players[i].SetActive(true);
            playerScoreTexts[i].text = gameData.playerScores[i].ToString();

            if (gameData.playerScores[i] > highScore)
            {
                highScore = gameData.playerScores[i];
                winnerID = i;
            }
        }

        // Display Winner
        playerScoreTexts[winnerID].fontSize = 60;
        playerScoreTexts[winnerID].color = Color.green;

        players[winnerID].transform.Translate(0, 0, 2.5f);
        players[winnerID].transform.Rotate(new Vector3(-19, 0, 0));
        players[winnerID].transform.Find("AlienPlayer").GetComponent<Animator>().SetBool("isGrounded", false);
    }

    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync("WhiteboxEditing");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("Start Menu");
    }
}
