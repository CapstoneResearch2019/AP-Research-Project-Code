using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout
{
    [System.Serializable]
    public struct rowData
    {
        public GameObject[] lane; 
    }

    public rowData[] lanes;
}
//All variables of this class contain two-dimensional GameObject arrays
//Class is designed to be displayed in the inspector, cannot be attached to any game objects
