using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [SerializeField]
    private int bpm;
    [SerializeField]
    private int beatsPerMeasure;
    [SerializeField]
    private int fullBeat;

    [SerializeField]
    private List<AttackGenerator> attackGenerators;

    [SerializeField]
    private List<Attack> attacks;

    // Update is called once per frame
    void Start()
    {
        float secondsPerBeat = 60.0f/bpm;
        float totalNotes = beatsPerMeasure * (16.0f/fullBeat);
        float rhythmLength = secondsPerBeat * beatsPerMeasure;
        float secondsPerNote = rhythmLength/totalNotes;

        Debug.Log($"Total Notes: {totalNotes}\nRhythm Length: {rhythmLength}\nNote Length: {secondsPerNote}");

        if(attackGenerators != null)
        {
            attacks = new List<Attack>();
            for(int i = 0; i < attackGenerators.Count; i++)
            {
                List<Beat> beats = new List<Beat>();
                for(int j = 0; j < attackGenerators[i].beatStrings.Count; j++)
                {
                    string beatString = attackGenerators[i].beatStrings[j];
                    if(beatString == null)
                    {
                        Debug.LogError($"Beat {i} does not have a beat string");
                    }
                    int beatLength = beatString.Length;
                    if(beatLength != totalNotes)
                    {
                        Debug.LogWarning($"Beat String {i} ({beatLength} chars) is not the same length as the total notes ({totalNotes})");
                    }
                    float delayBeforeNextBeat = secondsPerNote;
                    Beat beat = new Beat();
                    for(int k = 0; k < beatLength; k++)
                    {
                        if(beatString[k] == 'x')
                        {
                            beat.times.Add(delayBeforeNextBeat);
                            delayBeforeNextBeat = secondsPerNote;
                        }
                        else if(beatString[k] == 'o')
                        {
                            delayBeforeNextBeat += secondsPerNote;
                        }
                    }

                    beats.Add(beat);
                }
                attacks.Add(new Attack(attackGenerators[i].attackName, beats, attackGenerators[i].beatStrength, secondsPerBeat * beatsPerMeasure));
            }
        }
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