using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoplight : MonoBehaviour
{
    public float changeStartTime; //Determines when stoplight cycle begins
    public float changeRate; //Determines how fast stoplights change
    public ArrayLayout arrLayout; //Two-dimensional array to store arrays of stoplight points

    private GameObject[] cycles; //Stores red stoplights
    private int select = 0; //Determines which array inside arrLayout is stopped at any given time

    //Start function is called at the beginning of a program run
    void Start()
    {
        InvokeRepeating("Change", changeStartTime, changeRate); //Repeats Change function at a set start time and rate
    }

    //Cycles through GameObject arrays in arrLayout, 
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
