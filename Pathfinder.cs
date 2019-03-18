using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [HideInInspector] public PointNetwork pointNetwork;
    [HideInInspector] public bool mergingLeft;
    [HideInInspector] public bool mergingRight;
    [HideInInspector] public bool merging;
    [HideInInspector] public float mySpeed;
    public GameObject current;

    [SerializeField] private GameObject nextPos;
    private bool enterMerge;
    private Timer globalTimer;
    private float privateTimer;
    private float mergeTimer;
    private SpeedSearch spd;
    private CarArray carArray;
    private MeanCalculator record;

    void Start()
    {
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

    void Update()
    {
        if (!pointNetwork.stopSpace) mySpeed = spd.mySpeed;
        privateTimer += Time.deltaTime;
        if (pointNetwork.nextPos == null && pointNetwork.leftMerge == null && pointNetwork.rightMerge == null && pointNetwork.altNextPos == null)
        {
            //Debug.Log(privateTimer);
            record.SetMean(privateTimer, current);
            Destroy(gameObject);
        }
        if (!pointNetwork.stopSpace) transform.position += transform.forward * Time.deltaTime * mySpeed;
        else mySpeed = 0;

        if (merging)
        {
            mergeTimer += Time.deltaTime;
            if (mergeTimer > 2.0f) Destroy(gameObject);

            if (mergingLeft)
            {
                if (enterMerge == true)
                {
                    if (pointNetwork.leftMerge == null) merging = false;
                    else nextPos = pointNetwork.leftMerge;
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
                    transform.position -= transform.right * Time.deltaTime * 3;
                }
                else merging = false;
            }

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

        else if (!merging)
        {
            mergingLeft = false;
            mergingRight = false;
            if (pointNetwork.leftMerge != null && spd.SafeToMergeL() && spd.WantToMergeL())
            {
                MergeLeft();
            }
            if (pointNetwork.rightMerge != null && spd.SafeToMergeR() && spd.WantToMergeR())
            {
                MergeRight();
            }

            if (transform.position != nextPos.transform.position)
            {
                if (IsInFrontOf(nextPos))
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
                transform.LookAt(nextPos.transform.position);
            }
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
