using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string name;
    public List<Beat> beats;
    public float attack;
    public float musicLoopInterval;
    public bool magic;

    public Attack()
    {
    }

    public Attack(string name, List<Beat> beats, float attack, float loopInterval)
    {
        magic = true;
        this.name = name;
        this.beats = beats;
        this.attack = attack;
        this.musicLoopInterval = loopInterval;
    }

    public Attack(string name, Beat beat, float attack, float loopInterval)
    {
        magic = false;
        this.name = name;
        beats = new List<Beat>();
        beats.Add(beat);
        this.attack = attack;
        this.musicLoopInterval = loopInterval;
    }

    public float Score(float time, int index)
    {
        if(!magic)
        {
            return beats[0].GetScore(time);
        }
        else
        {
            return beats[index].GetScore(time);
        }
    }
}