using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TimedSpawn : MonoBehaviour
{
    public GameObject spawnPoint;
    public bool stopSpawning = false;
    public float spawnStartTime;
    public float spawnRate;
    public float spawnLimit;   
    public float topSAE;
    public float bottomSAE;

    private int saeLevel;
    private GameObject spawnee;
    private Transform spawnPos;
    private CarArray carArray;
    private Pathfinder pathfinder;
    private SpeedSearch spd;
    private Timer timer;
    private MeanCalculator record;

    //Awake function is called just before the beginning of a program run
    private void Awake()
    {
        stopSpawning = false;
        spawnPos = spawnPoint.GetComponent<Transform>();
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
        spd = gameObject.GetComponent<SpeedSearch>();
        timer = GameObject.Find("Manager").GetComponent<Timer>();
    }

    //Start function is called at the beginning of a program run
    void Start()
    {
        InvokeRepeating("SpawnObject", (spawnStartTime + Random.Range(-1.0f, 1.0f)), (spawnRate + Random.Range(-1.0f, 1.0f)));
    }

    //Update function is called once before each frame of animation
    void Update()
    {
        if (timer.spawnTimer >= (spawnLimit + spawnStartTime)) stopSpawning = true;
    }

    public void SpawnObject()
    {
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
            enabled = false;
        }
        if (!stopSpawning)
        {
            saeLevel = (int)Random.Range(topSAE, bottomSAE);
            spawnee = carArray.carArray[saeLevel];
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
