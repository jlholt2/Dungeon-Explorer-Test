using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMidpoint : MonoBehaviour
{
    // always move to the midpoint between actors
    // distances are measured against this object
    // if this object moves far enough in either direction, it will pull all actors and the tiling in the opposite direction to keep the seamless nature of battles

    public Vector3 pos { get { return transform.localPosition; } }

    /*[SerializeField] */
    //float midpointOffset = 0f;
    [SerializeField] float midpointClamp = 1.2f;

    [SerializeField] public float distanceBetweenFarthestActors;

    public BattleActor LeftmostActor { get { return leftmostActor; } }
    public BattleActor RightmostActor { get { return rightmostActor; } }

    [SerializeField] BattleActor leftmostActor;
    [SerializeField] BattleActor rightmostActor;

    private void Awake()
    {
        //GetFarthestActors(GameManager.instance.battleManager.trackingList());
        //ResetOrigin();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.battleManager.actorList() != null)
        {
            GetFarthestActors(GameManager.instance.battleManager.trackingList());
        }

        ResetOrigin();

        if (transform.position.x >= midpointClamp || transform.position.x <= -midpointClamp)
        {
            ResetToOrigin();
        }
    }

    private void ResetOrigin()
    {
        // Resets origin without moving actors

        float midpointOffset = 0f;

        if (GameManager.instance.battleManager.actorList() != null)
        {
            midpointOffset = GetMidpointBetweenFarthestActors();
        }
        transform.localPosition = new Vector3(midpointOffset, 0f, 0f);

        foreach (BattleActor actor in GameManager.instance.battleManager.actorList())
        {
            actor.dist = actor.pos.x-pos.x;
            //GameManager.instance.battleManager.materialOffset -= xDistance / 2 * Time.deltaTime * GameManager.instance.battleManager.materialOffsetTune;
        }
    }
    public void ResetToOrigin()
    {
        // Resets origin and moves all actors

        // get distance of this object from origin, and save it to a variable

        float xDistance = transform.localPosition.x;

        transform.position = Vector3.zero;

        foreach (BattleActor actor in GameManager.instance.battleManager.actorList())
        {
            float prevDist = actor.dist;
            actor.transform.localPosition = new Vector3(0+prevDist,actor.pos.y,actor.pos.z);
        }
        //GameManager.instance.battleManager.materialOffset -= xDistance / 2 * Time.deltaTime * GameManager.instance.battleManager.materialOffsetTune;
    }

    float GetMidpointBetweenFarthestActors()
    {
        return (leftmostActor.pos.x + rightmostActor.pos.x) / 2;
    }
    void GetFarthestActors(List<BattleActor> trackingActors)
    {
        float minimumX = 99;
        float maximumX = -99;
        for (int i = 0; i < trackingActors.Count; i++)
        {
            if (trackingActors[i].transform.localPosition.x > maximumX)
            {
                maximumX = trackingActors[i].transform.localPosition.x;
                leftmostActor = trackingActors[i];
            }
            if (trackingActors[i].transform.localPosition.x < minimumX)
            {
                minimumX = trackingActors[i].transform.localPosition.x;
                rightmostActor = trackingActors[i];
            }
        }
        distanceBetweenFarthestActors = leftmostActor.pos.x - rightmostActor.pos.x;
    }
}
