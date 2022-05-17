using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActor : MonoBehaviour
{
    //public BattleManager battleManager;

    public bool trackThisActor = false;

    [HideInInspector] public Vector3 pos;
    [HideInInspector] public Vector3 lastPos;

    public float dist;

    private void Awake()
    {
        //battleManager = GameObject.Find("BattleBox").GetComponent<BattleManager>();
        pos = transform.localPosition;
        dist = pos.x;
    }

    private void OnEnable()
    {
        dist = pos.x;
    }

    void Update()
    {
        lastPos = pos;
        pos = transform.localPosition;

        //if (trackThisActor)
        //{
        //    if(trackThisActor && GameManager.instance.battleManager.trackingList().Count == 1)
        //    {
                //if(dist > 0)
                //{
                //    dist = Mathf.Clamp(dist -= 1, 0, dist);
                //}else if(dist < 0)
                //{
                //    dist = Mathf.Clamp(dist += 1, -dist, 0);
                //}
            //}
            //transform.localPosition = new Vector3(Mathf.Clamp(pos.x, -GameManager.instance.battleManager.posClamp, GameManager.instance.battleManager.posClamp), pos.y, pos.z);
        //}

        //dist = Vector3.Distance(pos, new Vector3(0, pos.y, pos.z));
        //dist = pos.x;
    }
}
