using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortraitLoader : MonoBehaviour
{
    public GameObject portrait;
    private string text;
    public Sprite villager;
    public Sprite guard;
    public Sprite captain;
    public Sprite wolf; 

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>().text;
    }

    // Update is called once per frame
    void Update()
    {
        //Detects if a new character is being displayed and updates the image.
        if (gameObject.GetComponent<TextMeshProUGUI>().text != text)
        {
            text = gameObject.GetComponent<TextMeshProUGUI>().text;
            Debug.Log(text);

            if (text == "Villager")
            {
                portrait.GetComponent<Image>().sprite = villager;
            }
            else if (text == "Captain")
            {
                portrait.GetComponent<Image>().sprite = captain;
            }
            else if (text == "Guard")
            {
                portrait.GetComponent<Image>().sprite = guard;
            }
            else if (text == "Wolf")
            {
                portrait.GetComponent<Image>().sprite = wolf;
            }
        }
    }
}
