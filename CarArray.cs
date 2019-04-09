using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarArray : MonoBehaviour
{
    public GameObject[] carArray; //Contains each variation of the car object, assigned manually in inspector
    public GameObject[] destinationArray; //Contains each destination object for a given path, assigned manually in inspector
    [HideInInspector] public int destNum; //Length of destinationArray

    void Start()
    {
        destNum = destinationArray.Length;
    }
}
