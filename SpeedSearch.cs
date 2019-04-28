using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSearch : MonoBehaviour
{
    public float mySpeed = 5.0f; //Speed car is currently traveling at
    [HideInInspector] public int saeLevel = 1; //SAE level
    [HideInInspector] public GameObject destination; //Destination car attempts to reach

    private Pathfinder path; //Reference to Pathfinder.cs
    private PointNetwork pointNetwork; //Reference to PointNetwork.cs
    private CarArray carArray; //Reference to CarArray.cs
    private float preferredSpeed; //Speed that the car wants to move at
    private bool isAutomated; //Determines whether or not a car is automated 
    private int aggro; //Determines behavior of non-automated cars
    [SerializeField] private float followDistance; //Space in front of car where other cars can trigger following state 
    private float followSpeed; //Speed of car in following state
    private bool isFollowing = false; //Determines whether or not car is in following state
    private float tempFD; //temporary following distance
    
    //Start function is called at the beginning of a program run
    void Start()
    {
        if (saeLevel > 2) isAutomated = true; //Cars at SAE levels 3-5 are automated
        else isAutomated = false; //Cars at SAE levels 0-2 are not automated
        aggro = (int)Random.Range(1.0f, 10.9f); //Gives car a random value from 1-10
        if (isAutomated) preferredSpeed = 5; //Automated cars given the same preferredSpeed value
        else preferredSpeed = 2 + (aggro / 2); //Non-automated cars given a random preferredSpeed value
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
        destination = carArray.destinationArray[(int)Random.Range(0, (carArray.destNum - 0.1f))]; //Assigns a random destination to car
        path = gameObject.GetComponent<Pathfinder>();
        mySpeed = preferredSpeed;
        followSpeed = mySpeed;
    }

    //Update function is called once before every frame of animation
    void Update()
    {
        if (mySpeed < 0) mySpeed = 0; //Speed values cannot be negative
        if (followDistance < 1) followDistance = 1; //followDistance cannot fall below 1

        if (isAutomated) preferredSpeed = 5; //Automated cars have the same preferredSpeed value
        else preferredSpeed = 2 + (aggro / 2); //Non-automated cars have a random preferredSpeed value

        //Sets following distance as a function of speed
        if (isAutomated) tempFD = mySpeed;
        else tempFD = mySpeed - (aggro * 0.2f);
        if (tempFD >= 1) followDistance = tempFD;
        else followDistance = 1;
        
        Search(); //Function call
        SetSpeed(); //Function call
    }

    void SetSpeed()
    {
        if (isFollowing) mySpeed = followSpeed; //Copies speed of car in front when isFollowing is true
        else if (mySpeed != preferredSpeed) //Car changes its speed to reach its assigned preferredSpeed value
        {
            if (mySpeed > preferredSpeed)
            {
                mySpeed -= 0.5f * aggro * Time.deltaTime;
            }
            else if (mySpeed < preferredSpeed)
            {
                mySpeed += 0.5f * aggro * Time.deltaTime;
            }
        }
        else mySpeed = preferredSpeed;
    }

    //Searches for cars ahead at a certain distance
    void Search()
    {
        Ray ray = new Ray(transform.position + transform.forward * 1.8f, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, followDistance))
        {
            GameObject carInFront = hit.transform.gameObject;
            Pathfinder follow = carInFront.GetComponent<Pathfinder>();
            //Debug.DrawRay(ray.origin, ray.direction * followDistance, Color.red);
            if (carInFront != null && hit.collider.isTrigger)
            {
                SpeedSearch follows = carInFront.GetComponent<SpeedSearch>();
                if (follows.preferredSpeed <= preferredSpeed)
                {
                    followSpeed = follow.mySpeed;
                    isFollowing = true;
                }
            }
        }
        else
        {
            isFollowing = false;
        }
    }

    //Checks if there are other cars in a given area to the left of the car
    public bool SafeToMergeL()
    {
        int x = 0;
        RaycastHit hit;
        for (int i = 0; i < 9; i++)
        {
            Ray ray = new Ray(transform.position + transform.forward * (4 - i), -transform.right * 3);

            if (Physics.Raycast(ray, out hit, 3.0f))
            {
                GameObject leftCar = hit.transform.gameObject;
                if (leftCar != null && hit.collider.isTrigger)
                {
                    x++;
                }
            }
        }
        if (x > 0) return false;
        return true;
    }

    //Checks if there are other cars in a given area to the right of the car
    public bool SafeToMergeR()
    {
        int x = 0;
        RaycastHit hit;
        for (int i = 0; i < 9; i++)
        {
            Ray ray = new Ray(transform.position + transform.forward * (4 - i), transform.right * 3);

            if (Physics.Raycast(ray, out hit, 3.0f))
            {
                GameObject rightCar = hit.transform.gameObject;
                if (rightCar != null && hit.collider.isTrigger)
                {
                    x++;
                }
            }
        }
        if (x > 0) return false;
        return true;
    }

    //Checks if the car needs to merge to the left to reach its destination
    public bool WantToMergeL()
    {
        GameObject searchObj;
        PointNetwork searchNet;
        if (path.pointNetwork.nextPos != null)
        {
            searchObj = path.pointNetwork.nextPos;
            searchNet = searchObj.GetComponent<PointNetwork>();
            for (int i = 0; i < searchNet.destSize; i++)
            {
                if (searchNet.possibleDestArray[i].name == destination.name)
                {
                    return false;
                }
            }
        }
        if (path.pointNetwork.leftMerge != null)
        {
            searchObj = path.pointNetwork.leftMerge;
            searchNet = searchObj.GetComponent<PointNetwork>();
            for (int i = 0; i < searchNet.destSize; i++)
            {
                if (searchNet.possibleDestArray[i].name == destination.name)
                {
                    return true;
                }
            }
        }
        if (isAutomated == false && isFollowing == true & aggro >= 5) return true;
        return false;
    }

    //Checks if the car needs to merge to the right to reach its destination
    public bool WantToMergeR()
    {
        GameObject searchObj;
        PointNetwork searchNet;
        if (path.pointNetwork.nextPos != null)
        {
            searchObj = path.pointNetwork.nextPos;
            searchNet = searchObj.GetComponent<PointNetwork>();
            for (int i = 0; i < searchNet.destSize; i++)
            {
                if (searchNet.possibleDestArray[i].name == destination.name)
                {
                    return false;
                }
            }
        }
        if (path.pointNetwork.rightMerge != null)
        {
            searchObj = path.pointNetwork.rightMerge;
            searchNet = searchObj.GetComponent<PointNetwork>();
            for (int i = 0; i < searchNet.destSize; i++)
            {
                if (searchNet.possibleDestArray[i].name == destination.name)
                {
                    return true;
                }
            }
        }
        if (isAutomated == false && isFollowing == true & aggro >= 7) return true;
        return false;
    }
}
//Script defines how cars interact with each other and controls their speeds
//*ATTACH TO EACH CAR OBJECT PREFAB*
