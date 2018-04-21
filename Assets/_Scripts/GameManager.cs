using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameData gameData;

    public GameEvent onPlayerScored;
    public GameEvent onGameOver;
    
    private GameObject[] players;
    public CameraManager camManager;
    public Transform[] playerSpawns;
    
    private bool gameRunning = true;
    private int[] playerScores;
    private float timeLimit;
    private List<ItemData>[] playerItems;
    
    public static GameManager instance;

    #region Monobehaviours
    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Duplicate Game Managers Found.");
            Destroy(gameObject);
        }
        instance = this;

        InstantiateManager();

        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void Start()
    {
        SpawnPlayers();
        onPlayerScored.Raise();
    }

    public void Update()
    {
        if (gameRunning)
            TimerTick();
    }
    #endregion

    private void SpawnPlayers()
    {
        foreach(GameObject go in players)
        {
            PlayerController pc = go.GetComponent<PlayerController>();

            if (pc.playerID < gameData.numberOfPlayers)
            {
                pc.InstantiatePlayer(playerSpawns[pc.playerID].transform, camManager.cameraArray[pc.playerID]);
                SkinnedMeshRenderer rend = pc.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();
                rend.materials = new Material[] { rend.materials[0], gameData.playerColors[pc.playerID] };
                ShooterGameCamera sgc = camManager.cameraArray[pc.playerID].GetComponent<ShooterGameCamera>();
                sgc.SetTarget(pc.transform);
                
                pc.GetComponent<Zapper>().SetShooterGameCamera(sgc);
            }
            else
            {
                pc.gameObject.SetActive(false);
            }
        }
    }

    private void InstantiateManager()
    {
        // Create arrays
        playerScores = new int[gameData.numberOfPlayers];
        playerItems = new List<ItemData>[gameData.numberOfPlayers];

        // Fill up arrays
        for (int i = 0; i < gameData.numberOfPlayers; i++)
        {
            playerItems[i] = new List<ItemData>();
            playerScores[i] = 0;
        }

        // Starting variable values
        timeLimit = gameData.timeLimit;
    }

    private void TimerTick()
    {
        timeLimit -= Time.deltaTime;

        if (timeLimit <= 0)
        {
            EndGame();
            timeLimit = 0;
        }
    }

    private void EndGame()
    {
        gameRunning = false;
        gameData.playerScores = playerScores;
        Time.timeScale *= .3f;
        onGameOver.Raise();

        // Timer for a couple seconds
        StartCoroutine(EndGameTimer());
     }

    IEnumerator EndGameTimer()
    {
        // TODO: Add a fade out and a fade in
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Endgame Menu");
    }

    public bool AddItemToPlayer(int playerID, ItemData data)
    {
        if (gameRunning)
        {
            playerItems[playerID].Add(data);
            playerScores[playerID] += data.pointValue;
            onPlayerScored.Raise();
            return true;
        }
        return false;
    }

    public int GetPlayerScore(int playerID)
    {
        return playerScores[playerID];
    }

    public float GetTimeLeft()
    {
        return timeLimit;
    }
}
