using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TimedSpawn : MonoBehaviour
{
    public GameObject spawnPoint; //GameObject where cars are spawned, initialized in inspector
    public bool stopSpawning = false; //Determines if cars will spawn from a given spawn point
    public float spawnStartTime; //Determines when cars will begin to spawn, initialized in inspector
    public float spawnRate; //Determines how frequently cars will spawn, initialized in inspector
    public float spawnLimit; //Determines how long a spawner will continue to work, initialized in inspector
    public float topSAE; //Determines the highest SAE level a spawned car will have, initialized in inspector
    public float bottomSAE; //Determines the lowest SAE level a spawned car will have, initialized in inspector

    private int saeLevel; //Ranges from 0-5, determines if/when cars will be automated, initialized in inspector
    private GameObject spawnee; //Object spawned from spawnPoint (cars), initialized in inspector
    private Transform spawnPos; //Physical position of spawnPoint
    private CarArray carArray; //Reference to CarArray.cs
    private Pathfinder pathfinder; //Reference to Pathfinder.cs
    private SpeedSearch spd; //Reference to SpeedSearch.cs
    private Timer timer; //Reference to Timer.cs
    private MeanCalculator record; //Reference to MeanCalculator.cs

    //Awake function is called just before the beginning of a program run
    private void Awake()
    {
        //Initialization
        stopSpawning = false;
        spawnPos = spawnPoint.GetComponent<Transform>();
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
        spd = gameObject.GetComponent<SpeedSearch>();
        timer = GameObject.Find("Manager").GetComponent<Timer>();
    }

    //Start function is called at the beginning of a program run
    void Start()
    {
        //SpawnObject function called at a slightly randomized start time and rate
        InvokeRepeating("SpawnObject", (spawnStartTime + Random.Range(-1.0f, 1.0f)), (spawnRate + Random.Range(-1.0f, 1.0f)));
    }

    //Update function is called once before every frame of animation
    void Update()
    {
        if (timer.spawnTimer >= (spawnLimit + spawnStartTime)) stopSpawning = true; //stopSpawning set to true after a certain length of time
    }

    //Creates a clone of a car prefab at the position of spawnPoint
    public void SpawnObject()
    {
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject"); //SpawnObject function is no longer repeatedly called
            enabled = false; //Script is deactivated
        }
        if (!stopSpawning)
        {
            saeLevel = (int)Random.Range(topSAE, bottomSAE);
            spawnee = carArray.carArray[saeLevel];
            
            //Preliminary setup
            GameObject clone = Instantiate(spawnee, spawnPos.position, spawnPos.rotation);
            pathfinder = clone.GetComponent<Pathfinder>();
            spd = clone.GetComponent<SpeedSearch>();
            pathfinder.enabled = true;
            pathfinder.current = gameObject;
            pathfinder.pointNetwork = gameObject.GetComponent<PointNetwork>();
            spd.saeLevel = saeLevel;
        }
    }
}
//Script spawns cars at a customizable position and rate over a specified duration
//*ATTACH TO ALL SPAWN POINT OBJECTS IN SIMULATION*
