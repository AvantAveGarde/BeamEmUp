using Assets.MultiAudioListener;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMannSFX : MonoBehaviour {
    public List<AudioClip> audioList;
    public MultiAudioSource audioSource;
    public MultiAudioSource musicSource;

    public float count;
    public float countTimer;

    private int currentClip;
    private int previousClip;
	// Use this for initialization
	void Start () {
        countTimer = count;
	}
	
	// Update is called once per frame
	void Update () {
        //Every 10 Seconds play a random audio clip
        if(countTimer <= 0)
        {
            int currentClip = Random.Range(0, 4);
            
            while(currentClip == previousClip)
            {
                currentClip = Random.Range(0, 4);
            }
            ChangeSoundClip(currentClip);

            audioSource.Play();
            countTimer = count;

            previousClip = currentClip;
        }
        countTimer -= Time.deltaTime;

        if(audioSource.IsPlaying == true)
        {
            musicSource.Mute = true;
        }
        else
        {
            musicSource.Mute = false;
        }
    }

    void ChangeSoundClip(int i)
    {
        audioSource.AudioClip = audioList[i];
    }
}
