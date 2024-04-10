using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Beat
{
    public List<float> times;
    public int beatIndex;
    private float totalTime;
    private int lastBeatScored;

    public Beat()
    {
        times = new List<float>();
        beatIndex = 0;
        totalTime = 0;
        lastBeatScored = -1;
    }

    public float GetScore(float time)
    {
        totalTime += time;
        while(beatIndex < times.Count && totalTime > times[beatIndex])
        {
            totalTime -= times[beatIndex];
            beatIndex++;
        }

        if(beatIndex >= times.Count)
        {
            beatIndex = times.Count - 1;
        }

        int beatToScore = beatIndex;
        if(beatIndex != 0 && totalTime < times[beatIndex]/2.0f)
        {
            beatToScore--;
        }

        float score = beatToScore == lastBeatScored ? 0 : Mathf.Max(0, 1 - Mathf.Abs(times[beatToScore] - time));
        lastBeatScored = beatToScore;
        return score * score;
    }

    public bool IsDone()
    {
        return lastBeatScored >= times.Count - 1;
    }

    public void Reset()
    {
        beatIndex = 0;
        lastBeatScored = -1;
        totalTime = 0;
    }

}