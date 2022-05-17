using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    BattleMidpoint midpoint;

    ///*[SerializeField] */float midpointOffset = 0f;

    [SerializeField] bool overrideTracking = false;
    [SerializeField] bool overrideFOV = false;

    [SerializeField] int farClamp = 60;
    [SerializeField] int nearClamp = 45;

    [SerializeField] bool farCam = true;

    [SerializeField] float transitionDist = 0.8f;
    [SerializeField] float camZoomSpeed = 0.25f;
    [SerializeField] float camMoveSpeed = 0.25f;

    [SerializeField] Vector3 defaultCamAngle;

    Vector3 pos { get { return transform.localPosition; } }

    //[SerializeField] float distanceBetweenFarthestActors;

    //public BattleActor LeftmostActor { get { return leftmostActor; } }
    //public BattleActor RightmostActor { get { return rightmostActor; } }

    //[SerializeField] BattleActor leftmostActor;
    //[SerializeField] BattleActor rightmostActor;

    void Start()
    {
        midpoint = GameObject.Find("DistanceObject").GetComponent<BattleMidpoint>();
    }

    void Update()
    {
        if (!overrideTracking)
        {
            transform.localPosition = Vector3.MoveTowards(pos, new Vector3(-midpoint.pos.x, 0.12f, 0f), camMoveSpeed * Time.deltaTime);
            transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, defaultCamAngle, camMoveSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        //if (GameManager.instance.battleManager.actorList() != null)
        //{
        //    GetFarthestActors(GameManager.instance.battleManager.trackingList());
        //}
        if (!overrideTracking)
        {
            //if (GameManager.instance.battleManager.actorList() != null)
            //{
            //    midpointOffset = GetMidpointBetweenFarthestActors();
            //}
            transform.localPosition = Vector3.MoveTowards(pos,new Vector3(-midpoint.pos.x, 0.12f, 0f),camMoveSpeed * Time.deltaTime);
            transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, defaultCamAngle, camMoveSpeed * Time.deltaTime);
        }

        if (!overrideFOV)
        {
            if (GameManager.instance.battleManager.actorList() != null)
            {
                if (midpoint.distanceBetweenFarthestActors > transitionDist || midpoint.distanceBetweenFarthestActors < -transitionDist)
                {
                    farCam = true;
                }
                else
                {
                    farCam = false;
                }
            }
            if (farCam)
            {
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, farClamp, camZoomSpeed * Time.deltaTime);
            }
            else
            {
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, nearClamp, camZoomSpeed * Time.deltaTime);
            }
        }
    }

    //float GetMidpointBetweenFarthestActors()
    //{
    //    return (leftmostActor.pos.x + rightmostActor.pos.x) / 2;
    //}

    //void GetFarthestActors(List<BattleActor> trackingActors)
    //{
    //    float minimumX = 99;
    //    float maximumX = -99;
    //    for (int i = 0; i < trackingActors.Count; i++)
    //    {
    //        if (trackingActors[i].transform.localPosition.x > maximumX)
    //        {
    //            maximumX = trackingActors[i].transform.localPosition.x;
    //            leftmostActor = trackingActors[i];
    //        }
    //        if (trackingActors[i].transform.localPosition.x < minimumX)
    //        {
    //            minimumX = trackingActors[i].transform.localPosition.x;
    //            rightmostActor = trackingActors[i];
    //        }
    //    }
    //    distanceBetweenFarthestActors = leftmostActor.pos.x - rightmostActor.pos.x;
    //}
}
