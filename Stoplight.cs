using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoplight : MonoBehaviour
{
    public float changeStartTime;
    public float changeRate;
    public ArrayLayout arrLayout;

    private GameObject[] cycles;
    private int select = 0;

    void Start()
    {
        InvokeRepeating("Change", changeStartTime, changeRate);
    }

    public void Change()
    {
        cycles = arrLayout.lanes[select].lane; 

        for (int i = 0; i < arrLayout.lanes.Length; i++)
        {
            for (int j = 0; j < arrLayout.lanes[i].lane.Length; j++)
            {
                GameObject point = arrLayout.lanes[i].lane[j];
                PointNetwork intersection = point.GetComponent<PointNetwork>();
                intersection.stopSpace = false;
            }
        }
        for (int i = 0; i < arrLayout.lanes.Length; i++)
        {
            if (cycles == arrLayout.lanes[i].lane)
            {
                for (int j = 0; j < cycles.Length; j++)
                {
                    GameObject point = cycles[j];
                    PointNetwork intersection = point.GetComponent<PointNetwork>();
                    intersection.stopSpace = true;
                }
            }
        }
        
        if (select < arrLayout.lanes.Length - 1) select += 1;
        else select = 0;
    }
}
