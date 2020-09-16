using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField]private Transform mainCameraT;
    private Transform CameraD;
    private CharacterController _player;
    public Transform _groundChecker;
    [SerializeField] private LayerMask Ground;
    [SerializeField] private float GroundDistance = 0.2f;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float JumpHeight = 2;
    public Vector3 _velocity;

    [SerializeField] private float Sprint_speed = 30;
    private AudioSource _audiosource;

    public enum MoveState
    {
        idle,
        isMove,
        isSprint
    }

    public MoveState MS;
    private int i;
    GameObject CameraD_object;

    Player_State _playerState;



    // Use this for initialization
    void Start()
    {

        mainCameraT = Camera.main.gameObject.transform;
        CameraD_object = new GameObject();
        CameraD_object.transform.parent = transform;
        CameraD_object.transform.localPosition = transform.position;
        CameraD_object.name = "Direction";
        CameraD = CameraD_object.transform;
        _playerState = transform.GetComponent<Player_State>();
        _player = GetComponent<CharacterController>();
        _audiosource = GetComponent<AudioSource>();
        // StartCoroutine(FootSound());
    }

    // Update is called once per frame
    void Update()
    {
        //人物跟隨攝影機方向
        if (mainCameraT)
        { CameraD.eulerAngles = new Vector3(0, mainCameraT.eulerAngles.y, 0); }

        //移動
        float Vertical = Input.GetAxis("Vertical");
        float v = Mathf.Clamp(Vertical, -1f, 1f);
        float Horizontal = Input.GetAxis("Horizontal");
        float h = Mathf.Clamp(Horizontal, -1f, 1f);

        //衝刺
        switch (MS)
        {
            case MoveState.idle:
                {
                    if (!Player_State.ismove)
                    { }
                    if (Sprint())
                    {
                        if (Input.GetKeyDown(KeyCode.W) && i == 1)
                        { MS = MoveState.isSprint; }
                        else if (Input.GetKeyDown(KeyCode.S) && i == 2)
                        { MS = MoveState.isSprint; }
                        else if (Input.GetKeyDown(KeyCode.A) && i == 3)
                        { MS = MoveState.isSprint; }
                        else if (Input.GetKeyDown(KeyCode.D) && i == 4)
                        { MS = MoveState.isSprint; }
                    }
                    else
                    {
                        if (Vertical != 0)
                        {
                            MS = MoveState.isMove;
                            Player_State.ismove = true;
                        }
                        else if (Horizontal != 0)
                        {
                            MS = MoveState.isMove;
                            Player_State.ismove = true;
                        }
                    }

                }
                break;

            case MoveState.isMove:
                {

                    if (Player_State.ismove)
                    {
                        _player.Move(CameraD.forward * v * Time.deltaTime * speed);
                        _player.Move(CameraD.right * h * Time.deltaTime * speed);
                    }

                    if (Sprint())
                    {
                        if (Input.GetKeyDown(KeyCode.W) && i == 1)
                        { MS = MoveState.isSprint; }
                        else if (Input.GetKeyDown(KeyCode.S) && i == 2)
                        { MS = MoveState.isSprint; }
                        else if (Input.GetKeyDown(KeyCode.A) && i == 3)
                        { MS = MoveState.isSprint; }
                        else if (Input.GetKeyDown(KeyCode.D) && i == 4)
                        { MS = MoveState.isSprint; }
                    }

                    if (v == 0 && h == 0)
                    {
                        ComboTime_Time = 0;
                        MS = MoveState.idle;
                    }

                }
                break;

            case MoveState.isSprint:
                {
                    _player.Move(CameraD.forward * v * Time.deltaTime * Sprint_speed);
                    _player.Move(CameraD.right * h * Time.deltaTime * Sprint_speed);
                    MS = MoveState.isMove;
                    if (v == 0 && h == 0)
                    {
                        ComboTime_Time = 0;
                        MS = MoveState.idle;
                    }
                }
                break;
        }

        if (ComboTime_Time > ComboTime)
        {
            isComboing = false;
            ComboTime_Time = 0;
        }

        if (Input.GetKeyDown(KeyCode.W))
        { isComboing = true; i = 1; }
        else if (Input.GetKeyDown(KeyCode.S))
        { isComboing = true; i = 2; }
        else if (Input.GetKeyDown(KeyCode.A))
        { isComboing = true; i = 3; }
        else if (Input.GetKeyDown(KeyCode.D))
        { isComboing = true; i = 4; }


        //跳躍
        _playerState.is_grounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (_playerState.is_grounded && _velocity.y < 0)
        { _velocity.y = 0f; }
        _velocity.y += Physics.gravity.y * Time.deltaTime;
        _player.Move(_velocity * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        { _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y); }


        //上下左右鍵方向
        if (v > 0)
        { SmoothRotation(CameraD.eulerAngles.y); }
        else if (v < 0)
        { SmoothRotation(CameraD.eulerAngles.y - 180); }

        if (h > 0)
        { SmoothRotation(CameraD.eulerAngles.y + 90); }
        else if (h < 0)
        { SmoothRotation(CameraD.eulerAngles.y - 90); }

        //鎖定時角度修正
        // if (Player_State.islock == true && Vertical == 0 && Horizontal == 0 && Player_target._target != null)
        // { Rotation_To(Player_target._target.position); }

    }

    // IEnumerator FootSound()
    // {
    //     //Debug.Log(_player.velocity.magnitude);
    //     while (true)
    //     {
    //         if (_playerState.is_grounded && MS == MoveState.isMove)
    //         {
    //             _audiosource.PlayOneShot(walksound);
    //             yield return new WaitForSeconds(walksound.length);
    //         }
    //         else
    //         { yield return null; }
    //     }
    // }

    public void SmoothRotation(float a)
    {
        float y = 3.0f;
        float rotateSpeed = 0.1f;
        transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, a, ref y, rotateSpeed), 0);
    }


    public void Rotation_To(Vector3 t)
    {
        Vector3 targetDir = t - transform.position;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.1f, 0.0F);

        transform.rotation = Quaternion.LookRotation(new Vector3(newDir.x, 0, newDir.z));
    }

    private bool isComboing = false;
    private float ComboTime_Time = 0;
    public float ComboTime = 1;

    //二下W:衝刺鍵
    bool Sprint()
    {
        if (isComboing)
        {
            ComboTime_Time += Time.deltaTime;
            return true;
        }
        else
            return false;
    }
}
