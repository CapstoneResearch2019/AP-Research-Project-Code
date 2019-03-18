using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeanCalculator : MonoBehaviour
{
    private CarArray carArray;
    [SerializeField] private float mean = 0;
    [SerializeField] private float std = 0;
    [SerializeField] private float[] meanArr;
    private float[] totalTimeArr;
    private int[] totalCarsArr;

    void Start()
    {
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
        meanArr = new float[carArray.destNum];
        totalTimeArr = new float[carArray.destNum];
        totalCarsArr = new int[carArray.destNum];
    }

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

        FindTotalMean();
    }

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
        FindStandardDeviation();
    }

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
