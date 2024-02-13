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
            float offset = times[beatIndex++] - time;
            return Mathf.Max(0, 1 - (offset * offset));
        }

        return float.NaN;
    }

    public bool IsDone()
    {
        return beatIndex >= times.Count;
    }

}