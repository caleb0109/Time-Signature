using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager singleton;
    public AudioSource[] audioSources;
    public int audioSourceIndex;
    public AudioClip backgroundMusic;
    // Start is called before the first frame update

    private double musicLength;
    private double nextFlip;
    void Start()
    {
        if(singleton != null)
        {
            Debug.LogError("More than one Loop Manager");
        }

        singleton = this;
        audioSources = new AudioSource[2];
        for(int i = 0; i < audioSources.Length; i++)
        {
            GameObject audioSource = new GameObject("BackgroundMusicSource");
            audioSource.transform.parent = transform;
            audioSources[i] = audioSource.AddComponent<AudioSource>();
            audioSources[i].clip = backgroundMusic;
        }

        musicLength = backgroundMusic.length;
        audioSources[audioSourceIndex].Play();
        nextFlip = AudioSettings.dspTime + musicLength;
    }

    // Update is called once per frame
    void Update()
    {
        if(AudioSettings.dspTime + 1.0 > nextFlip)
        {
            audioSourceIndex ^= 1;
            audioSources[audioSourceIndex].PlayScheduled(nextFlip);
            nextFlip += musicLength;
        }
    }
}
