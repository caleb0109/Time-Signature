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
    private Beat currentBeat;

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

    [SerializeField]
    TMP_Text scoreBox;
    [SerializeField]
    TMP_Text timeBox;

    //list of spawned objects to destroy before starting next beat
    [SerializeField]
    private List<GameObject> indicators;

    private AudioSource audioController;
    [SerializeField]
    private AudioClip[] beatSounds;

    private DateTime prevInputTime;
    private BeatFinishedCallback callback;

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


    public void SetBeat(Beat beat)
    {
        currentBeat = beat;
    }

    public void ShowExample()
    {
        isDone = false;
        if(currentBeat == null)
        {
            Debug.LogError("Tried to begin, but no beat is set");
        }
        StartCoroutine(DisplayBeat());
    }


    public void BeginInput(BeatFinishedCallback beatFinishedCallback)
    {
        if(currentBeat == null)
        {
            Debug.LogError("Tried to begin, but no beat is set");
        }
        score = 0;
        justStarted = true;
        inputManager.Enable();
        callback = beatFinishedCallback;
    }

    IEnumerator DisplayBeat()
    {
        //get timing list for convenience
        List<float> beatTimes = currentBeat.times;
        for(int i = 0; i < beatTimes.Count; i++)
        {
            //wait for the next beat
            yield return new WaitForSeconds(beatTimes[i]);
            //spawn an indicator at the next position and add it to the list
            Vector2 position = exampleStartPos + exampleOffset * i;
            indicators.Add(Instantiate(indicator, position, Quaternion.identity));
            int index = UnityEngine.Random.Range(0, beatSounds.Length);
            audioController.clip = beatSounds[index];
            audioController.Play();
        }
        //set variables for keeping track of inputs
        finishedDisplaying = true;
        justStarted = true;
        yield return null;
    }

    void PlayNote(InputAction.CallbackContext ctx)
    {
        //if out of notes or the example is running, return
        if(!finishedDisplaying)
        {
            return;
        }
        DateTime current = DateTime.Now;
        float timeElapsed = justStarted ? 0 : (float)(current - prevInputTime).TotalMilliseconds/1000.0f;
        prevInputTime = current;
        //spawn an indicator at the next postion and add it to the lisr
        Vector2 position = playerStartPos + playerOffset * currentBeat.beatIndex;
        indicators.Add(Instantiate(indicator, position, Quaternion.identity));
        //increase the score based on the time since the last input
        score += currentBeat.GetScore(timeElapsed);

        if(scoreBox != null)
        {
            scoreBox.text = score.ToString();
        }

        if(timeBox != null)
        {
            timeBox.text = timeElapsed.ToString();
        }

        int index = UnityEngine.Random.Range(0, beatSounds.Length);
        audioController.clip = beatSounds[index];
        audioController.Play();
        //indicate time should start counting
        justStarted = false;
        if(currentBeat.IsDone())
        {
            //delete all spawned indicators
            for(int i = 0; i < indicators.Count; i++)
            {
                Destroy(indicators[i]);
            }
            indicators.Clear();
            inputManager.Disable();
            isDone = true;
            if(callback != null)
            {
                callback(score);
            }
            currentBeat.Reset();
        }
    }
}
