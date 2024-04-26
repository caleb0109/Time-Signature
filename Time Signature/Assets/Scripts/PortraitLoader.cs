using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortraitLoader : MonoBehaviour
{
    public GameObject portrait;
    private string text;
    public Sprite villagerMan;
    public Sprite villagerWoman;
    public Sprite guardOne;
    public Sprite guardTwo;
    public Sprite captain;
    public Sprite wolf;
    public Sprite boy;

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

            portrait.GetComponent<Image>().color = new Color(1, 1, 1, 1);

            switch (text)
            {
                case "Villager":
                    {
                        portrait.GetComponent<Image>().sprite = villagerMan;

                        break;
                    }
                case "Boy":
                    {
                        portrait.GetComponent<Image>().sprite = boy;

                        break;
                    }
                case "Lady":
                    {
                        portrait.GetComponent<Image>().sprite = villagerWoman;

                        break;
                    }
                case "Guard 1":
                    {
                        portrait.GetComponent<Image>().sprite = guardOne;

                        break;
                    }
                case "Guard 2":
                    {
                        portrait.GetComponent<Image>().sprite = guardTwo;

                        break;
                    }
                case "Captain":
                    {
                        portrait.GetComponent<Image>().sprite = captain;

                        break;
                    }
                case "Wolf":
                    {
                        portrait.GetComponent<Image>().sprite = wolf;

                        break;
                    }
                default:
                    {
                        portrait.GetComponent<Image>().color = new Color(1, 1, 1, 0);

                        break;
                    }
            }
        }
    }
}
