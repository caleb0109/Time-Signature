using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class RhythmManager : MonoBehaviour
{
    //list of beats for testing
    [SerializeField]
    private List<Beat> beats;

    //keep track of display progress
    private bool startDisplaying;
    private bool finishedDisplaying;
    //keep track of first note of a beat
    private bool justStarted;
    //time since last note played
    private float timer;
    //keep track of progress in beat and which beat to play next
    private int beatIndex;

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
    private float score;

    [SerializeField]
    TMP_Text scoreBox;
    [SerializeField]
    TMP_Text timeBox;

    //list of spawned objects to destroy before starting next beat
    [SerializeField]
    private List<GameObject> indicators;

    //for when the test beats are out
    private bool finished;

    private AudioSource audioController;
    [SerializeField]
    private AudioClip beatSound;


    InputManager inputManager;
    void Start()
    {
        //enable input and set startDisplaying to start
        inputManager = new InputManager();
        inputManager.Enable();
        inputManager.Character.PlayNote.performed += ctx => PlayNote(ctx);
        startDisplaying = true;
        audioController = GetComponent<AudioSource>();
        audioController.clip = beatSound;
    }

    // Update is called once per frame
    void Update()
    {
        timeBox.text = timer.ToString();
        //if our of beats, stop
        if(finished)
        {
            return;
        }
        if(startDisplaying)
        {
            //begins example beat
            StartCoroutine(DisplayBeat());
            startDisplaying = false;
        }
        else if(finishedDisplaying)
        {
            //time is only counted after the first beat
            //this allows the player to start whenever they are ready
            if(!justStarted)
            {
                timer += Time.deltaTime;
            }
            if(beats[beatIndex].IsDone())
            {
                //move on to the next beat, reset the timer, and set flags for display
                beatIndex++;
                finished = beatIndex >= beats.Count;
                timer = 0;
                finishedDisplaying = false;
                startDisplaying = true;
                //delete all spawned indicators
                for(int i = 0; i < indicators.Count; i++)
                {
                    Destroy(indicators[i]);
                }
                indicators.Clear();
            }
        }
    }

    IEnumerator DisplayBeat()
    {
        //get timing list for convenience
        List<float> beatTimes = beats[beatIndex].times;
        for(int i = 0; i < beatTimes.Count; i++)
        {
            //wait for the next beat
            yield return new WaitForSeconds(beatTimes[i]);
            //spawn an indicator at the next position and add it to the list
            Vector2 position = exampleStartPos + exampleOffset * i;
            indicators.Add(Instantiate(indicator, position, Quaternion.identity));
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
        if(finished || !finishedDisplaying)
        {
            return;
        }
        //spawn an indicator at the next postion and add it to the lisr
        Vector2 position = playerStartPos + playerOffset * beats[beatIndex].beatIndex;
        indicators.Add(Instantiate(indicator, position, Quaternion.identity));
        //increase the score based on the time since the last input
        score += beats[beatIndex].GetScore(timer);
        scoreBox.text = score.ToString();
        audioController.Play();
        //indicate time should start counting and reset it
        justStarted = false;
        timer = 0;
    }
}
