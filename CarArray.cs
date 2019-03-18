using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarArray : MonoBehaviour
{
    public GameObject[] carArray; 
    public GameObject[] destinationArray;
    [HideInInspector] public int destNum;

    void Start()
    {
        destNum = destinationArray.Length;
    }
}
