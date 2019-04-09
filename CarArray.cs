using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarArray : MonoBehaviour
{
    public GameObject[] carArray; //Contains each variation of the car object, assigned manually in inspector
    public GameObject[] destinationArray; //Contains each destination object for a given path, assigned manually in inspector
    [HideInInspector] public int destNum; //Length of destinationArray, initialized in Start function

    //Start function is called at the beginning of a program run
    void Start()
    {
        destNum = destinationArray.Length;
    }
}
//Used to define global variables in the inspector
//*ATTACH SCRIPT TO MANAGER*
