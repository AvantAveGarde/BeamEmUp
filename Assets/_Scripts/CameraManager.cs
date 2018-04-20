using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameData GameData;
    public GameManager gm;

    public Camera[] cameraArray;
    private int numberOfPlayers;

    public bool HorizontalMode;

	// Use this for initialization
    
	void Awake () {
        numberOfPlayers = GameData.numberOfPlayers;
        HorizontalMode = GameData.HorizontalMode;
        
        if (numberOfPlayers == 1)
        {
            cameraArray[0].rect = new Rect(0, 0, 1, 1);
            cameraArray[2].enabled = false;
            cameraArray[3].enabled = false;
            cameraArray[1].enabled = false;
        }
        else if (numberOfPlayers == 2)
        {
            if(HorizontalMode == true)
            {
                cameraArray[0].rect = new Rect(0f, .5f, 1f, .5f); //Top Half
                cameraArray[1].rect = new Rect(0f, 0f, 1f, .5f); //Bottom Half
            }
            else
            {
                cameraArray[0].rect = new Rect(0, 0, .5f, 1f);
                cameraArray[1].rect = new Rect(.5f, 0, .5f, 1);
            }
            cameraArray[2].enabled = false;
            cameraArray[3].enabled = false;

        }
        else if (numberOfPlayers == 3)
        {
            cameraArray[0].rect = new Rect(0, .5f, .5f, .5f);
            cameraArray[1].rect = new Rect(.5f, .5f, .5f, .5f);
            cameraArray[2].rect = new Rect(.25f, 0, .5f, .5f);
            cameraArray[3].enabled = false;
        }
        else if (numberOfPlayers == 4)
        {
            cameraArray[0].rect = new Rect(0f, .5f, .5f, .5f);
            cameraArray[1].rect = new Rect(.5f, .5f, .5f, .5f);
            cameraArray[2].rect = new Rect(0f, 0f, .5f, .5f);
            cameraArray[3].rect = new Rect(.5f, 0f, .5f, .5f);
        }
        
    }
}
