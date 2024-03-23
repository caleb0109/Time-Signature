using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [SerializeField]
    private List<Attack> attacks;

    [SerializeField]
    private AudioClip audioToTurnIntoBeat;
    [SerializeField]
    private int fftWindowSize;

    [SerializeField]
    private Beat outputBeat;

    [SerializeField]
    private AudioSource audioSource;

    // Update is called once per frame
    void Start()
    {
        // int sampleCount = audioToTurnIntoBeat.samples;
        // float[] samples = new float[sampleCount];
        // audioToTurnIntoBeat.GetData(samples, 0);
        // int fftSteps = sampleCount/fftWindowSize;
        // audioSource.clip = audioToTurnIntoBeat;
        // audioSource.Play();
    }

    IEnumerator CalculateBeat()
    {
        
        yield return null;
    }

    public Attack GetAttack(int attackIndex)
    {
        return attacks[(int)attackIndex];
    }

    public string[] GetAllAttackNames()
    {
        string[] names = new string[attacks.Count()];
        for(int i = 0; i < attacks.Count(); i++)
        {
            names[i] = attacks[i].name;
        }
        return names;
    }
}