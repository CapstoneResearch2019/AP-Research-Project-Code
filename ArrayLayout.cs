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
