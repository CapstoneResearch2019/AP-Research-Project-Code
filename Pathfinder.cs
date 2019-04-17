using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [HideInInspector] public PointNetwork pointNetwork; //Reference to PointNetwork.cs
    [HideInInspector] public bool mergingLeft; //Determines merging state of car
    [HideInInspector] public bool mergingRight; //Determines merging state of car
    [HideInInspector] public bool merging; //Determines merging state of car
    [HideInInspector] public float mySpeed; //Determines speed of car
    public GameObject current; //Identifies the last PointNetwork object car has passed through

    [SerializeField] private GameObject nextPos; //Identifies the PointNetwork object car is moving towards
    private bool enterMerge; //Determines merging state of car
    private Timer globalTimer; //Records time from the beginning of the program
    private float privateTimer; //Records time from the initialization of car object running this script
    private float mergeTimer; //Records time from the start of a merge
    private SpeedSearch spd; //Reference to SpeedSearch.cs
    private CarArray carArray; //Reference to CarArray.cs
    private MeanCalculator record; //Reference to MeanCalculator.cs
    
    //Start function is called at the beginning of a program run
    void Start()
    {
        //Initialization
        gameObject.name = "Car";
        merging = false;
        spd = gameObject.GetComponent<SpeedSearch>();
        spd.enabled = true;
        nextPos = pointNetwork.nextPos;
        privateTimer = 0.0f;
        globalTimer = GameObject.Find("Manager").GetComponent<Timer>();
        record = GameObject.Find("Manager").GetComponent<MeanCalculator>();
        carArray = GameObject.Find("Manager").GetComponent<CarArray>();
    }
    
    //Update function is called once before every frame of animation
    void Update()
    {
        if (!pointNetwork.stopSpace) mySpeed = spd.mySpeed; //Speed set by SpeedSearch procedures
        privateTimer += Time.deltaTime; //Records time between car spawn and deletion
  
        if (pointNetwork.nextPos == null && pointNetwork.leftMerge == null
        && pointNetwork.rightMerge == null && pointNetwork.altNextPos == null) //Car deleted when it has no path to follow
        {
            record.SetMean(privateTimer, current); //Sends timer variable to MeanCalculator.cs
            Destroy(gameObject);
        }
        //Car always moves forward when in scene, only stops at stoplights
        if (!pointNetwork.stopSpace) transform.position += transform.forward * Time.deltaTime * mySpeed;
        else mySpeed = 0;

        //Merging state
        if (merging)
        {
            //Car is deleted if in merging state for too long
            mergeTimer += Time.deltaTime;
            if (mergeTimer > 2.0f) Destroy(gameObject);

            //Merge left
            if (mergingLeft)
            {
                //Only true at the beginning of a merge, used for preliminary setup
                if (enterMerge == true)
                {
                    if (pointNetwork.leftMerge == null) merging = false; //Deactivates merging state if there is no path to merge to
                    else nextPos = pointNetwork.leftMerge;
                    enterMerge = false;
                }
                else if (IsInLineWith(nextPos))
                {
                    merging = false; //Deactivates merging state
                }
                else if (transform.position != nextPos.transform.position)
                {
                    if (IsInFrontOf(nextPos))
                    {
                        pointNetwork = nextPos.GetComponent<PointNetwork>();
                        current = pointNetwork.current;
                        nextPos = pointNetwork.nextPos;
                    }
                    transform.position -= transform.right * Time.deltaTime * 3;
                }
                else merging = false;
            }

            //Merge right
            if (mergingRight)
            {
                if (enterMerge == true)
                {
                    if (pointNetwork.rightMerge == null) merging = false;
                    else nextPos = pointNetwork.rightMerge;
                    enterMerge = false;
                }
                else if (IsInLineWith(nextPos))
                {
                    merging = false;
                }
                else if (transform.position != nextPos.transform.position)
                {
                    if (IsInFrontOf(nextPos))
                    {
                        pointNetwork = nextPos.GetComponent<PointNetwork>();
                        current = pointNetwork.current;
                        nextPos = pointNetwork.nextPos;
                    }
                    transform.position += transform.right * Time.deltaTime * 3;
                }
                else merging = false;
            }
        }

        //Non-merging state
        else if (!merging)
        {
            //Prevents car from merging
            mergingLeft = false;
            mergingRight = false;
            
            if (pointNetwork.leftMerge != null && spd.SafeToMergeL() && spd.WantToMergeL()) //Merging conditions
            {
                MergeLeft(); //Function call, activates merging state
            }
            if (pointNetwork.rightMerge != null && spd.SafeToMergeR() && spd.WantToMergeR()) //Merging conditions
            {
                MergeRight(); //Function call, activates merging state
            }

            //Car faces towards nextPos object while moving forward
            if (transform.position != nextPos.transform.position)
            {
                if (IsInFrontOf(nextPos))
                {
                    pointNetwork = nextPos.GetComponent<PointNetwork>();
                    current = pointNetwork.current;
                    nextPos = pointNetwork.nextPos;
                    
                    //If an alternate path exists, car may move on the alternate path instead
                    if (pointNetwork.altNextPos != null)
                    {
                        int x = 0;
                        PointNetwork altNet = pointNetwork.altNextPos.GetComponent<PointNetwork>();
                        for (int i = 0; i < altNet.destSize; i++)
                        {
                            if (altNet.possibleDestArray[i].name == spd.destination.name)
                            {
                                x = 1;
                                break;
                            }
                            else x = (int)Random.Range(0.0f, 1.9f);
                        }
                        if (x == 1)
                        {
                            current = pointNetwork.current;
                            nextPos = pointNetwork.altNextPos;
                        }
                    }
                }
                transform.LookAt(nextPos.transform.position);
            }
            
            //After nextPos is reached, car looks for a new next position
            else
            {
                pointNetwork = nextPos.GetComponent<PointNetwork>();
                current = pointNetwork.current;
                nextPos = pointNetwork.nextPos;
                if (pointNetwork.altNextPos != null)
                {
                    int x = 0;
                    PointNetwork altNet = pointNetwork.altNextPos.GetComponent<PointNetwork>();
                    for (int i = 0; i < altNet.destSize; i++)
                    {
                        if (altNet.possibleDestArray[i].name == spd.destination.name)
                        {
                            x = 1;
                            break;
                        }
                        else x = (int)Random.Range(0.0f, 1.9f);
                    }
                    if (x == 1)
                    {
                        current = pointNetwork.current;
                        nextPos = pointNetwork.altNextPos;
                    }

                }
            }
        }
    }

    //Checks if car is facing towards a given object
    public bool IsInLineWith(GameObject target)
    {
        Vector3 between = (target.transform.position - transform.position).normalized;
        Vector3 inFront = transform.forward.normalized;
        float angle = Vector3.Angle(between, inFront);

        if (angle >= 177 || angle <= 3 || transform.position == target.transform.position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    //Checks if car has passed a given object
    public bool IsInFrontOf(GameObject target)
    {
        Vector3 between = (target.transform.position - transform.position).normalized;
        Vector3 inFront = transform.forward.normalized;
        float angle = Vector3.Angle(between, inFront);

        if (angle > 90)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Sets variables to allow the car to merge to the left
    public void MergeLeft()
    {
        if (!merging)
        {
            merging = true;
            mergingRight = false;
            enterMerge = true;
            mergingLeft = true;
            mergeTimer = 0;
        }
        else return;
    }

    //Sets variables to allow the car to merge to the right
    public void MergeRight()
    {
        if (!merging)
        {
            merging = true;
            mergingRight = true;
            enterMerge = true;
            mergingLeft = false;
            mergeTimer = 0;
        }
        else return;
    }
}
//Script determines the behavior of car movement
//*ATTACH TO EACH CAR OBJECT PREFAB*
