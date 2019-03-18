using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSearch : MonoBehaviour
{
    public float mySpeed = 5.0f;
    [HideInInspector] public int saeLevel = 1;
    [HideInInspector] public GameObject destination;

    private Pathfinder path; 
    private PointNetwork pointNetwork;
    private CarArray carArray;
    private float preferredSpeed;
    private bool isAutomated;
    private int aggro;
    [SerializeField] private float followDistance;
    private float followSpeed;
    private bool isFollowing = false;
    private float laneWidth;
    private float tempFD;
    
    void Start()
    {
        if (saeLevel > 2) isAutomated = true;
        else isAutomated = false;
        aggro = (int)Random.Range(1.0f, 10.9f);
        if (isAutomated) preferredSpeed = 5;
        else preferredSpeed = 2 + (aggro / 2);
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
        destination = carArray.destinationArray[(int)Random.Range(0, (carArray.destNum - 0.1f))];
        path = gameObject.GetComponent<Pathfinder>();
        mySpeed = preferredSpeed;
        followSpeed = mySpeed;
    }

    void Update()
    {
        if (mySpeed < 0) mySpeed = 0;
        if (followDistance < 1) followDistance = 1;

        if (isAutomated) preferredSpeed = 5;
        else preferredSpeed = 2 + (aggro / 2);

        if (isAutomated) tempFD = mySpeed;
        else tempFD = mySpeed - (aggro * 0.2f);
        if (tempFD >= 1) followDistance = tempFD;
        else followDistance = 1;
        Search();
        SetSpeed();        
    }

    void SetSpeed()
    {
        if (isFollowing) mySpeed = followSpeed;
        else if (mySpeed != preferredSpeed)
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
            //Debug.DrawRay(ray.origin, ray.direction * followDistance, Color.blue);
            isFollowing = false;
        }
    }

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
                //Debug.DrawRay(ray.origin, ray.direction * 3, Color.green);
                if (leftCar != null && hit.collider.isTrigger)
                {
                    x++;
                }
            }
        }
        if (x > 0) return false;
        return true;
    }

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
                //Debug.DrawRay(ray.origin, ray.direction * 3, Color.green);
                if (rightCar != null && hit.collider.isTrigger)
                {
                    x++;
                }
            }
        }
        if (x > 0) return false;
        return true;
    }

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
