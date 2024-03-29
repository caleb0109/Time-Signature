using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string name;
    public Beat beat;
    public List<Beat> beats;
    public int attack;
    public float backgroundMusicStartInterval;
    public bool magic;

    public float Score(float time, int index)
    {
        if(!magic)
        {
            return beat.GetScore(time);
        }
        else
        {
            return 0;
        }
    }
}