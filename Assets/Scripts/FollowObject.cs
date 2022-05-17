using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;
    [SerializeField] bool followZ = true;

    [SerializeField] GameObject followObject;

    private void Update()
    {
        Track();
    }
    private void FixedUpdate()
    {
        Track();
    }

    private void Track()
    {
        if (followObject)
        {
            Vector3 pos = transform.localPosition;
            Vector3 followPos = followObject.transform.localPosition;
            float newX = pos.x;
            float newY = pos.y;
            float newZ = pos.z;
            if (followX)
            {
                newX = followPos.x;
            }
            if (followY)
            {
                newY = followPos.y;
            }
            if (followZ)
            {
                newZ = followPos.z;
            }

            transform.localPosition = new Vector3(newX, newY, newZ);
        }
    }
}
