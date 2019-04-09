using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeanCalculator : MonoBehaviour
{
    private CarArray carArray; //Reference to CarArray.cs, used for calling functions and variables
    [SerializeField] private float mean = 0; //Overall average travel time recorded per simulation, initialized as 0
    [SerializeField] private float std = 0; //Standard deviation of travel times recorded per simulation, initialized as 0
    [SerializeField] private float[] meanArr; //Array of average travel times separated by intended destination
    private float[] totalTimeArr; //Input value for finding average travel times separated by intended destination
    private int[] totalCarsArr; //Input value for finding average travel times separated by intended destination

    //Start function is called at the beginning of a program run 
    void Start()
    {
        //Initialization
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
        meanArr = new float[carArray.destNum];
        totalTimeArr = new float[carArray.destNum];
        totalCarsArr = new int[carArray.destNum];
    }
    
    //Calculates individual mean travel times in meanArr[] based on updates in Pathfinder.cs
    public void SetMean(float t, GameObject dest)
    {
        int x = 0;
        for (int i = 0; i < carArray.destNum; i++)
        {
            if (carArray.destinationArray[i].name == dest.name)
            {
                x = i;
                break;
            }
            else x = 0;
        }
        totalTimeArr[x] += t;
        totalCarsArr[x] += 1;
        meanArr[x] = totalTimeArr[x] / ((float)totalCarsArr[x]);

        FindTotalMean(); //Function call
    }

    //Calculates value for mean
    void FindTotalMean()
    {
        mean = 0;
        int x = 0;
        for (int i = 0; i < meanArr.Length; i++)
        {
            if (meanArr[i] != 0)
            {
                mean += meanArr[i];
                x += 1;
            }
        }
        mean /= x;
        FindStandardDeviation(); //Function call
    }
    
    //Calculates standard deviation for meanArr[] data set
    void FindStandardDeviation()
    {
        float variance = 0;
        for (int i = 0; i < meanArr.Length; i++)
        {
            if (meanArr[i] != 0) variance += (meanArr[i] - mean) * (meanArr[i] - mean);
        }
        variance /= (meanArr.Length - 1);
        std = (float)Math.Sqrt(variance);
    }
}
//View this script during simulations to gather data
//*ATTACH TO MANAGER OBJECT*
