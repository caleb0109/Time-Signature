using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [SerializeField]
    private List<Attack> attacks;
    // Update is called once per frame
    void Update()
    {

    }

    public Attack GetAttack(int attackIndex)
    {
        return attacks[(int)attackIndex];
    }

    public string[] GetAllAttackNames()
    {
        string[] names = new string[attacks.Count()];
        for(int i = 0; i < attacks.Count(); i++)
        {
            names[i] = attacks[i].name;
        }
        return names;
    }
}