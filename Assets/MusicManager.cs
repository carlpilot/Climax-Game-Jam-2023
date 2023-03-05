using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip day;
    public AudioClip night;

    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        audioSource.clip = day;
        audioSource.Play();
    }
    
    void Update()
    {
        if (GameManager.isCurrentlyDay)
        {
            if (audioSource.clip != day) {
                audioSource.clip = day;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip != night){
                audioSource.clip = night;
                audioSource.Play();
            }
        }
    }
}
