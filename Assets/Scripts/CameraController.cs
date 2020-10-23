using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCameraBase vCam;
    public CinemachineVirtualCameraBase aimCam;
    public Camera menuCamera;
    public Camera mainCamera;

    public LayerMask layerMask;
    [Space(10)]
    [Header("===== Aim Settings =====")]
    public bool doAim;
    [SerializeField] private float offestY;
    private float offestYDampVelocity;
    [Space(10)]
    [Header("===== Latent Settings =====")]
    public float latentHalfAngle = 85f;
    public float latentMinSpecialAngle = 65f;
    public float latentMaxSpecialAngle = 90f;
    public float angle;
    public float offsetYLatentDis = 1.35f;
    // Use this for initialization
    void Start()
    {
        if (vCam)
        { vCam.MoveToTopOfPrioritySubqueue(); }
        // mainCamera.enabled = false;
        // menuCamera.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager._instance.gameObject.activeSelf)
        {
            // menuCamera.enabled = false; 
            // mainCamera.enabled = true;
        }
        // if (doAim)
        // { aimCam.MoveToTopOfPrioritySubqueue(); }
        // else
        // {
        //     if (vCam)
        //     { vCam.MoveToTopOfPrioritySubqueue(); }
        // }
    }
    public void DoAim()
    {
        doAim = true;

    }
    public void DoUnAim()
    {
        doAim = false;

    }

    public void ReLiveCam()
    {
        // offsetZDistance /= 2;
        // offestDampValue /= 5;
    }
}
