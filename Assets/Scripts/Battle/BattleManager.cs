using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct BattleData
{
    // This will contain the data, such as the backdrop, enemies to load, etc.
}
public class BattleManager : MonoBehaviour
{
    public bool battleOn = false;

    public BattleData currentBattle;

    BattleCamera battleCamera;

    public BattleMidpoint distanceObject;

    public float posClamp = 0.72f;
    public float groundVal = 0.02f;
    public float ceilVal = 0.38f;

    public bool useCeil = true;

    public float materialOffset = 0f;
    public float materialOffsetTune = 0.27f;

    public float maxKnockbackSpeed = 0.09f;

    private void Awake()
    {
        battleCamera = GameObject.Find("Battle Camera").GetComponent<BattleCamera>();
        distanceObject = GameObject.Find("DistanceObject").GetComponent<BattleMidpoint>();
    }

    public List<BattleActor> actorList()
    {
        BattleActor[] battleActors = transform.Find("SpriteLayer").GetComponentsInChildren<BattleActor>();

        List<BattleActor> returnList = new List<BattleActor>();

        for (int i = 0; i < battleActors.Length; i++)
        {
            returnList.Add(battleActors[i]);
        }

        return returnList;
    }
    public List<BattleActor> trackingList()
    {
        List<BattleActor> returnList = new List<BattleActor>();

        for (int i = 0; i < actorList().Count; i++)
        {
            if (actorList()[i].trackThisActor)
            {
                returnList.Add(actorList()[i]);
            }
        }

        return returnList;
    }

    private void Update()
    {
        if (battleOn)
        {
            var keyboard = Keyboard.current;

            if (keyboard.pKey.wasPressedThisFrame)
            {
                EndBattle();
            }
        }
        GetComponent<BattleArena>().offset = materialOffset;
    }

    //public void UpdateBattleActorDists(float distChange, BattleActor actingActor)
    //{
    //    foreach (BattleActor actor in actorList())
    //    {
    //        if(actingActor.dist > -posClamp && actingActor.dist < posClamp || !actingActor.trackThisActor)
    //        {
    //            if (actor != actingActor && actingActor.trackThisActor && actingActor.dist > -posClamp + 0.1f 
    //                && actingActor.dist < posClamp - 0.1f && (actingActor == battleCamera.LeftmostActor || actingActor == battleCamera.RightmostActor))
    //            {
    //                actor.dist -= distChange/2 * Time.deltaTime;
    //                materialOffset -= distChange/2 * Time.deltaTime * materialOffsetTune;
    //            }
    //            else if(actor == actingActor)
    //            {
    //                if ((actingActor == battleCamera.LeftmostActor || actingActor == battleCamera.RightmostActor) && actorList().Count > 1)
    //                {
    //                    actor.dist += distChange / 2 * Time.deltaTime;
    //                }
    //                else
    //                {
    //                    actor.dist += distChange * Time.deltaTime;
    //                }
    //            }
    //            if (actor.trackThisActor)
    //            {
    //                actor.dist = Mathf.Clamp(actor.dist, -posClamp + 0.05f, posClamp - 0.05f);
    //            }
    //        }
    //    }
    //}

    public void StartBattle()
    {
        // turn on battle camera
        // turn off dungeon camera

        // battleManager.battleOn = true;

        // turn on BattlePlayer's inputs

        // turn off dungeon UI

        GameManager.instance.battleCamera.gameObject.SetActive(true);
        GameManager.instance.dungeonCamera.gameObject.SetActive(false);

        GameManager.instance.battleManager.battleOn = true;

        GameManager.instance.battlePlayer.GetComponent<BattleInput>().controlLock = false;

        GameManager.instance.SetFrameRate("battle");
    }

    public void EndBattle()
    {
        // turn on dungeon camera
        // turn off battle camera

        // battleManager.battleOn = false;

        // advance event queue

        // turn on dungeon UI

        GameManager.instance.dungeonCamera.gameObject.SetActive(true);
        GameManager.instance.battleCamera.gameObject.SetActive(false);

        GameManager.instance.battleManager.battleOn = false;

        GameManager.instance.SetFrameRate("dungeon");

        GameManager.instance.advanceEvent = true;
    }
}
