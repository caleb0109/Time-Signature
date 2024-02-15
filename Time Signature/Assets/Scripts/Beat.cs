using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Beat
{
    public List<float> times;
    public int beatIndex;

    public float GetScore(float time)
    {
        if(!IsDone())
        {
            float score = Mathf.Max(0, 1 - Mathf.Abs(times[beatIndex] - time));
            beatIndex++;
            return score * score;
        }

        return float.NaN;
    }

    public bool IsDone()
    {
        return beatIndex >= times.Count;
    }

    public void Reset()
    {
        beatIndex = 0;
    }

}