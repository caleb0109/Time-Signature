using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    [SerializeField]
    private float beatHeights;

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
    private List<DateTime> prevInputTime;

    //function to call once the player is finished
    private BeatFinishedCallback callback;

    //combat system uses this to check progress
    public bool isDone;

    public GameObject feedbackText;

    public int beatsFinished;

    InputManager inputManager;

    void Start()
    {


        //enable input and set startDisplaying to start
        inputManager = new InputManager();
        inputManager.Character.PlayNote.performed += ctx => PlayNote(ctx, 0);
        inputManager.Character.PlayMagicNoteA.performed += ctx => PlayNote(ctx, 1);
        inputManager.Character.PlayMagicNoteB.performed += ctx => PlayNote(ctx, 2);
        inputManager.Character.PlayMagicNoteC.performed += ctx => PlayNote(ctx, 3);
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
        float currentTime = LoopManager.singleton.audioSources[LoopManager.singleton.audioSourceIndex].time;
        float waitTime = ((MathF.Ceiling(currentTime/currentAttack.musicLoopInterval) + 1) * currentAttack.musicLoopInterval) - currentTime;
        DateTime startTime = DateTime.Now.AddSeconds(waitTime);
        //begin displaying the example
        prevInputTime = new List<DateTime>();
        for(int i = 0; i < currentAttack.beats.Count; i++)
        {
            prevInputTime.Add(startTime);
        }
        StartCoroutine(DisplayBeat(waitTime));
    }

    IEnumerator DisplayBeat(float waitTime)
    {

        for(int i = 0; i < currentAttack.beats.Count; i++)
        {
            List<float> beatTimes = currentAttack.beats[i].times;
            float totalTime = waitTime;
            for(int j = 0; j < beatTimes.Count; j++)
            {
                totalTime += beatTimes[j];
                indicators.Add(Instantiate(indicator));
                indicators[indicators.Count - 1].transform.position = exampleStartPos + new Vector2(totalTime * exampleSpeed, i * beatHeights);
                indicators[indicators.Count - 1].transform.Rotate(new Vector3(0, 0, i * 90));
                indicators[indicators.Count - 1].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        while(!isDone)
        {
            for(int i = 0; i < indicators.Count; i++)
            {
                indicators[i].transform.position += new Vector3(-Time.deltaTime * exampleSpeed, 0, 0);
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

    void PlayNote(InputAction.CallbackContext ctx, int index)
    {
        //calculate the time since the last input and update the time to the last input
        DateTime current = DateTime.Now;
        float timeElapsed = (float)(current - prevInputTime[index]).TotalMilliseconds/1000.0f;
        prevInputTime[index] = current;

        //Play the same sound that was used for the example
        audioController.clip = beatSounds[(int)UnityEngine.Random.Range(0, beatSounds.Length)];
        audioController.Play();
        //increase the score based on the time since the last input
        float temp = currentAttack.beats[index].GetScore(timeElapsed);
        
        //Perfect
        if (temp >= 0.9)
        {
            feedbackText.GetComponent<TextMeshProUGUI>().text = "Perfect";
        }
        //Good
        else if (temp >= 0.5)
        {
            feedbackText.GetComponent<TextMeshProUGUI>().text = "Good";
        }
        //Bad
        else if (temp > 0)
        {
            feedbackText.GetComponent<TextMeshProUGUI>().text = "Bad";
        }
        //Miss
        else
        {
            feedbackText.GetComponent<TextMeshProUGUI>().text = "Miss";
        }

        Debug.Log(temp);
        score += temp;
        if(currentAttack.beats[index].IsDone())
        {
            beatsFinished++;
        }
        //if the player has performed enough inputs for each beat
        if(beatsFinished >= currentAttack.beats.Count)
        {
            //disable input
            inputManager.Disable();
            isDone = true;

            //reset the beat so it can be played again
            currentAttack.beats[index].Reset();
            beatsFinished = 0;
            //if a callback was passed in, call it
            //and pass it the score
            if(callback != null)
            {
                callback(score * currentAttack.attack);
            }

        }
    }
}
