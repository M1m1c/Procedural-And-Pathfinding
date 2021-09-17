using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentScript : MonoBehaviour
{
    public static int TotalScore { get; set; }

    public static int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    private static int currentHealth = 3;

    private AudioSource myAudioSource;

    private static bool isPlayingMusic = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (!isPlayingMusic)
        {
            isPlayingMusic = true;
            myAudioSource = GetComponent<AudioSource>();
            myAudioSource.Play();
        }   
    }
}
