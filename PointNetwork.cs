using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointNetwork : MonoBehaviour
{
    //Initialized in inspector
    public GameObject current; //Reference point car is moving from
    public GameObject nextPos; //Reference point car is moving towards
    public GameObject leftMerge; //Reference point for left merge
    public GameObject rightMerge; //Reference point for right merge
    public GameObject altNextPos; //Reference point car may move towards in the case of a branching path
    public GameObject[] possibleDestArray; //Array of destination points reachable by a car at any given location
    
    [HideInInspector] public bool stopSpace = false; //Determines if a point is a stoplight
    [HideInInspector] public int destSize; //Length of possibleDestArray

    //Start function is called at the beginning of a program run
    void Start()
    {
        //Initialization
        current = gameObject;
        destSize = possibleDestArray.Length;
    }

    //Update function is called before each frame
    void Update()
    {
        if (stopSpace == false) IconManager.SetIcon(gameObject, IconManager.Icon.DiamondGray);
        else IconManager.SetIcon(gameObject, IconManager.Icon.DiamondRed);
    }

    void OnDrawGizmos()
    {
        if (nextPos != null)
        {
            Gizmos.DrawLine(transform.position, nextPos.transform.position);
        }
        if (altNextPos != null)
        {
            Gizmos.DrawLine(transform.position, altNextPos.transform.position);
        }
    }
}
