using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DistanceWolf : MonoBehaviour
{

    [SerializeField]
    private GameObject playerGO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y > playerGO.transform.position.y - 1.2){
            this.GetComponent<SortingGroup>().sortingOrder = 0;
        }
        else{
            this.GetComponent<SortingGroup>().sortingOrder = 10;
        }
        
    }
}
