using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TMann : MonoBehaviour {
    public Text alertMessage;
    public TMannSpawner TmannSpawner;
    private GameManager gm;

	// Use this for initialization
	private void Update()
    {
        alertMessage.text = TmannSpawner.messageText;
    }
}
