using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWPlayerLook : MonoBehaviour
{
    Camera cam;

    public bool overwriteCamPos;

    public float transitionLookSpeed = 500f;

    public Vector3 TargetLocalRotation { get { return targetLocalRotation; } set { targetLocalRotation = value; } }
    Vector3 targetLocalRotation;

    public float neutralPos = -0.7f;
    public float zoomPos = -0.07f;

    [SerializeField] private float zoomSpeed = 500f;

    private float targetZPos;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        targetZPos = neutralPos;
    }
    private void FixedUpdate()
    {
        if (!overwriteCamPos)
        {
            Look();
        }
    }

    private void Look()
    {
        if (targetLocalRotation.y > 270 && targetLocalRotation.y < 361f) targetLocalRotation.y = 0f;
        if (targetLocalRotation.y < 0f) targetLocalRotation.y = 270f;

        cam.transform.localRotation = Quaternion.RotateTowards(cam.transform.localRotation, Quaternion.Euler(targetLocalRotation), Time.deltaTime * transitionLookSpeed);

        cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition, new Vector3(0, 0, targetZPos), zoomSpeed);
    }

    public void LookUp() { /*if (AtRest)*/ targetLocalRotation = new Vector3(-45,0,0); }
    public void LookDown() { /*if (AtRest)*/ targetLocalRotation = new Vector3(45, 0, 0); }
    public void LookNeutral() { /*if (AtRest)*/ targetLocalRotation = Vector3.zero; }
    public void LookZoom()
    {
        if (cam.transform.localEulerAngles == Vector3.zero)
        {
            targetZPos = zoomPos;
        }
        else
        {
            LookNoZoom();
        }
    }public void LookNoZoom()
    {
        targetZPos = neutralPos;
    }

    public bool CameraMatchesTarget()
    {
        return (cam.transform.localPosition.z == targetZPos);
    }

    //bool AtRest
    //{
    //    get
    //    {
    //        if ((Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
}
