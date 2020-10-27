using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCameraBase main_vcam;
    public CinemachineVirtualCameraBase aim_vcam;
    public CinemachineVirtualCameraBase menu_vcam;

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
    public float speed = 3f;

    private CinemachineDollyCart dCartComp;

    void Awake()
    {
        if (MenuManager.instance != null)
        { menu_vcam.MoveToTopOfPrioritySubqueue(); }
    }
    // Use this for initialization
    void Start()
    {
        dCartComp = GetComponent<CinemachineDollyCart>();
        dCartComp.m_Speed = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // if (doAim)
        // { aimCam.MoveToTopOfPrioritySubqueue(); }
        // else
        // {
        //     if (vCam)
        //     { vCam.MoveToTopOfPrioritySubqueue(); }
        // }
    }

    public void ChangeMenuCamera()
    {
        menu_vcam.MoveToTopOfPrioritySubqueue();
    }

    public void ChangeMainCamera()
    {
        main_vcam.MoveToTopOfPrioritySubqueue();
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
