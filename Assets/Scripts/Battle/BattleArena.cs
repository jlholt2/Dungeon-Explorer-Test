using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleArena : MonoBehaviour
{
    [SerializeField] GameObject[] setpieces;
    //[SerializeField] GameObject floorGO;
    //[SerializeField] GameObject ceilGO;

    public float offset = 0f;

    public void Update()
    {
        //wallGO.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0f);
        //floorGO.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0f);
        //ceilGO.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0f);
        foreach (GameObject setpiece in setpieces)
        {
            setpiece.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0f);
        }
    }

    //public void FixedUpdate()
    //{
    //    offset = 0f;
    //}
}
