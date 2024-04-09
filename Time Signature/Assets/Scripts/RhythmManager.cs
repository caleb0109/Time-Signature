using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void BeatFinishedCallback(float score);

[RequireComponent(typeof(AudioSource))]
public class RhythmManager : MonoBehaviour
{
    [SerializeField]
    private Attack currentAttack;

    //keep track of display progress
    private bool finishedDisplaying;
    //keep track of first note of a beat
    private bool justStarted;

    //object to display for beats
    [SerializeField]
    private GameObject indicator;
    [SerializeField]
    //controls the positioning of the example
    private Vector2 exampleStartPos;
    [SerializeField]
    private float exampleSpeed;

    //visible in inspector for testing
    public float score;

    //list of spawned objects to destroy before starting next beat
    [SerializeField]
    private List<GameObject> indicators;

    //objects for playing sounds and the sounds to play
    private AudioSource audioController;
    [SerializeField]
    private AudioClip[] beatSounds;

    [SerializeField]
    private AudioSource backgroundAudioSource;

    //keep track of time since last input, since update isn't used
    private DateTime prevInputTime;

    //function to call once the player is finished
    private BeatFinishedCallback callback;

    //combat system uses this to check progress
    public bool isDone;

    InputManager inputManager;

    void Start()
    {
        //enable input and set startDisplaying to start
        inputManager = new InputManager();
        inputManager.Character.PlayNote.performed += ctx => PlayNote(ctx);
        audioController = GetComponent<AudioSource>();

        //set isDone to true so we can actually do rhythm
        isDone = true;
    }


    public void SetBeat(Attack attack)
    {
        currentAttack = attack;
    }

    public void BeginBeat(BeatFinishedCallback beatFinishedCallback)
    {
        //make sure a beat is set
        if(currentAttack == null)
        {
            Debug.LogError("Tried to begin input, but no beat is set");
            return;
        }
        //reset score
        score = 0;
        //set justStarted so player chooses when to start
        justStarted = true;
        //enable player input and set the function to be called once the player finishes the beat
        inputManager.Enable();
        callback = beatFinishedCallback;

        //set so combat script can keep track of progress
        isDone = false;
        //make sure a beat is set
        if(currentAttack == null)
        {
            Debug.LogError("Tried to begin example, but no beat is set");
            return;
        }

        for(int i = 0; i < currentAttack.beat.times.Count; i++)
        {
            indicators.Add(Instantiate(indicator));
        }

        //begin displaying the example
        prevInputTime = DateTime.Now;
        double currentTime = backgroundAudioSource.time;
        double waitTime = (Math.Ceiling(currentTime/currentAttack.backgroundMusicStartInterval) * currentAttack.backgroundMusicStartInterval) - currentTime;
        prevInputTime = prevInputTime.AddSeconds(waitTime);
        StartCoroutine(DisplayBeat(waitTime));
    }

    IEnumerator DisplayBeat(double waitTime)
    {
        //get timing list for convenience
        List<float> beatTimes = currentAttack.beat.times;
        float totalTime = (float)waitTime;
        for(int i = 0; i < beatTimes.Count; i++)
        {
            totalTime += beatTimes[i];
            indicators[i].transform.position = exampleStartPos + new Vector2(totalTime * exampleSpeed, 0);
        }

        while(!isDone)
        {
            for(int i = 0; i < beatTimes.Count; i++)
            {
                indicators[i].transform.Translate(-Time.deltaTime * exampleSpeed, 0, 0);
            }
            yield return null;
        }
        //set variables for keeping track of inputs
        finishedDisplaying = true;

        //delete all spawned indicators
        for(int i = 0; i < indicators.Count; i++)
        {
            Destroy(indicators[i]);
        }
        indicators.Clear();
        yield return null;
    }

    void PlayNote(InputAction.CallbackContext ctx)
    {
        //calculate the time since the last input and update the time to the last input
        DateTime current = DateTime.Now;
        float timeElapsed = (float)(current - prevInputTime).TotalMilliseconds/1000.0f;
        prevInputTime = current;

        //Play the same sound that was used for the example
        audioController.clip = beatSounds[(int)UnityEngine.Random.Range(0, beatSounds.Length)];
        audioController.Play();
        //increase the score based on the time since the last input
        score += currentAttack.beat.GetScore(timeElapsed);

        //if the player has performed enough inputs for each beat
        if(currentAttack.beat.IsDone())
        {
            //disable input
            inputManager.Disable();
            isDone = true;

            //if a callback was passed in, call it
            //and pass it the score
            if(callback != null)
            {
                callback(score);
            }

            //reset the beat so it can be played again
            currentAttack.beat.Reset();
        }
    }
}
