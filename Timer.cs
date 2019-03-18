using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [HideInInspector] public float spawnTimer = 0.0f;
    [HideInInspector] public int intSpawnTimer = 0; 

    void Update()
    {
        spawnTimer += Time.deltaTime;
        intSpawnTimer = (int)Time.deltaTime;
    }
}
