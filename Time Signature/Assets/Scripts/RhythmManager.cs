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
    private Vector2 exampleOffset;
    //controls where the player's beats appear
    [SerializeField]
    private Vector2 playerStartPos;
    [SerializeField]
    private Vector2 playerOffset;

    //visible in inspector for testing
    public float score;

    //list of spawned objects to destroy before starting next beat
    [SerializeField]
    private List<GameObject> indicators;

    //objects for playing sounds and the sounds to play
    private AudioSource audioController;
    [SerializeField]
    private AudioClip[] beatSounds;
    //list of beats used by the example
    [SerializeField]
    private List<int> chosenBeatSounds;

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
        chosenBeatSounds = new List<int>();

        //set isDone to true so we can actually do rhythm
        isDone = true;
    }


    public void SetBeat(Attack attack)
    {
        currentAttack = attack;
    }

    public void ShowExample()
    {
        //set so combat script can keep track of progress
        isDone = false;
        //make sure a beat is set
        if(currentAttack == null)
        {
            Debug.LogError("Tried to begin example, but no beat is set");
            return;
        }
        //begin displaying the example
        StartCoroutine(DisplayBeat());
    }


    public void BeginInput(BeatFinishedCallback beatFinishedCallback)
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
    }

    IEnumerator DisplayBeat()
    {
        float currentTime = backgroundAudioSource.time;
        float waitTime = (MathF.Ceiling(currentTime/currentAttack.backgroundMusicStartInterval) * currentAttack.backgroundMusicStartInterval) - currentTime;
        Debug.Log("Current Time: " + currentTime);
        Debug.Log("Attack Interval: " + currentAttack.backgroundMusicStartInterval);
        Debug.Log("Wait Time: " + waitTime);
        yield return new WaitForSeconds(waitTime);
        //get timing list for convenience
        List<float> beatTimes = currentAttack.beat.times;
        for(int i = 0; i < beatTimes.Count; i++)
        {
            //wait for the next beat
            yield return new WaitForSeconds(beatTimes[i]);
            //spawn an indicator at the next position and add it to the list
            Vector2 position = exampleStartPos + exampleOffset * i;
            indicators.Add(Instantiate(indicator, position, Quaternion.identity));

            //pick a clap sound to play, and store it for the player inputs
            int index = UnityEngine.Random.Range(0, beatSounds.Length);
            audioController.clip = beatSounds[index];
            audioController.Play();
            chosenBeatSounds.Add(index);
        }
        //set variables for keeping track of inputs
        finishedDisplaying = true;
        justStarted = true;
        yield return null;
    }

    void PlayNote(InputAction.CallbackContext ctx)
    {
        //if example is running, return
        if(!finishedDisplaying)
        {
            return;
        }
        //calculate the time since the last input and update the time to the last input
        DateTime current = DateTime.Now;
        float timeElapsed = justStarted ? 0 : (float)(current - prevInputTime).TotalMilliseconds/1000.0f;
        prevInputTime = current;

        //spawn an indicator at the next postion and add it to the list
        Vector2 position = playerStartPos + playerOffset * currentAttack.beat.beatIndex;
        indicators.Add(Instantiate(indicator, position, Quaternion.identity));
        //Play the same sound that was used for the example
        audioController.clip = beatSounds[chosenBeatSounds[currentAttack.beat.beatIndex]];
        audioController.Play();
        //increase the score based on the time since the last input
        score += currentAttack.beat.GetScore(timeElapsed);


        //indicate time should start counting
        justStarted = false;

        //if the player has performed enough inputs for each beat
        if(currentAttack.beat.IsDone())
        {
            //delete all spawned indicators
            for(int i = 0; i < indicators.Count; i++)
            {
                Destroy(indicators[i]);
            }
            indicators.Clear();
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
            //reset the sounds so new ones can be chosen next time
            chosenBeatSounds.Clear();
        }
    }
}
