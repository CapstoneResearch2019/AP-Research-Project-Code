using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointNetwork : MonoBehaviour
{
    public GameObject current;
    public GameObject nextPos;
    public GameObject leftMerge;
    public GameObject rightMerge;
    public GameObject altNextPos;
    public GameObject[] possibleDestArray; 
    [HideInInspector] public bool stopSpace = false;
    [HideInInspector] public int destSize;

    void Start()
    {
        current = gameObject;
        destSize = possibleDestArray.Length;
    }

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
