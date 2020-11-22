using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SimpleCharacterControl : MonoBehaviour
{
    [SerializeField] private bool isControling = true;
    #region Move variable
    [SerializeField] private float m_moveSpeed = 5;
    [SerializeField] private float m_jumpForce = 10;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;
    private float m_currentV = 0;
    private float m_currentH = 0;
    private readonly float m_interpolation = 10;
    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;
    #endregion
    #region Jump variable
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();
    public GameObject flyParticle;
    #endregion
    #region Climb variable
    [Header("Climb Variable")]
    public Transform rightHand;  //右手著力點
    public Transform rightFoot;  //右腳著力點 
    [SerializeField] private bool isClimbPoint;
    private float climbUpMatchStart = 0.24f;
    private float climbUpMatchEnd = 0.61f;
    private float climbDownMatchStart = 0.01f;
    private float climbDownMatchEnd = 0.34f;
    [HideInInspector] public AnimatorStateInfo m_State;
    #endregion
    #region Shoot variable
    [Header("Shoot Variable")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float throwerPower;
    private float intervalTime = 0;
    private float t = 0.2f;
    [SerializeField] public LayerMask layermask;
    public GameObject Shoot_Vcam;
    private int maxDistatnce = 500;
    [Space(10)]
    public GameObject groundChecker;
    #endregion

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody>();
        GameManager._instance.onReset += new GameManager.ManipulationHandler(Dead);
        flyParticle.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
                m_animator.SetInteger("JumpCount", 0);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            GameManager._instance.Dead();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;

            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0)
            { m_isGrounded = false; }
        }
        if (collision.collider.tag == "SavePoint")
        { transform.parent = collision.transform; }
        // if (collision.collider.tag == "Block")
        // { transform.parent = collision.transform; }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0)
        { m_isGrounded = false; }
        // if (collision.collider.tag == "Block")
        // { transform.parent = null; }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ClimbPoint")
        {
            rightHand = other.transform.Find("RightHand");
            rightFoot = other.transform.Find("RightFoot");
            isClimbPoint = true;
        }
        if (other.tag == "SavePoint")
        { GameManager._instance.Save(other.transform); }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "ClimbPoint")
        {
            rightHand = other.transform.Find("RightHand");
            rightFoot = other.transform.Find("RightFoot");
            isClimbPoint = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "ClimbPoint")
        {
            isClimbPoint = false;
        }
    }

    private void FixedUpdate()
    {
        if (isControling)
        {
            m_animator.SetBool("Grounded", m_isGrounded);
            if (!m_State.IsName("Base Layer.Climb.ClimbUp") && !m_State.IsName("Base Layer.Climb.ClimbDown") && !m_State.IsName("Base Layer.Climb.Climbing"))
            {
                float v = Input.GetAxis("Vertical");
                float h = Input.GetAxis("Horizontal");


                m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
                m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

                Vector3 direction = Camera.main.transform.forward * m_currentV + Camera.main.transform.right * m_currentH;

                float directionLength = direction.magnitude;
                direction.y = 0;
                direction = direction.normalized * directionLength;

                if (direction != Vector3.zero)
                {
                    m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);
                    transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
                    m_animator.SetFloat("MoveSpeed", direction.magnitude);
                }

                //上下左右鍵方向
                if (v > 0)
                { SmoothRotation(Camera.main.transform.eulerAngles.y); }
                else if (v < 0)
                { SmoothRotation(Camera.main.transform.eulerAngles.y - 180); }

                if (h > 0)
                { SmoothRotation(Camera.main.transform.eulerAngles.y + 90); }
                else if (h < 0)
                { SmoothRotation(Camera.main.transform.eulerAngles.y - 90); }

                if (Shoot_Vcam != null)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    { Shoot_Vcam.SetActive(true); }
                    else
                    { Shoot_Vcam.SetActive(false); }
                }

                //射擊
                Shoot();
                //飛行
                Flying();
                //跳躍
                JumpingAndLanding();
            }
            //攀爬
            Climbing();

            groundChecker.transform.position = GroundCheck();
            m_wasGrounded = m_isGrounded;
        }
    }
    #region private function
    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_animator.GetInteger("JumpCount") < 2 && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            // ResetVelocity();
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            m_animator.SetTrigger("Jump");

            int jc = m_animator.GetInteger("JumpCount") + 1;
            m_animator.SetInteger("JumpCount", jc);
        }
    }
    private void Flying()
    {
        if (!isClimbPoint && !m_isGrounded && Input.GetMouseButton(1))
        {
            m_animator.SetBool("Fly", true);
            flyParticle.SetActive(true);
        }
        else
        {
            m_animator.SetBool("Fly", false);
            flyParticle.SetActive(false);
        }

        if (m_State.IsName("Base Layer.Fly"))
        {
            if (m_rigidBody.velocity.y <= 0)
            { m_rigidBody.AddForce(-Physics.gravity * 0.9f); }
        }
    }
    private void Climbing()
    {
        if (m_animator)
        {
            //獲取動畫狀態
            m_State = m_animator.GetCurrentAnimatorStateInfo(0);

            if (Input.GetButtonDown("Fire2"))
            {
                if (m_animator.GetBool("Climb"))
                { m_animator.SetBool("Climb", false); }
                else if (isClimbPoint && !m_animator.GetBool("Climb"))
                { m_animator.SetBool("Climb", true); }
            }

            if (m_State.IsName("Base Layer.Climb.ClimbUp"))
            {
                m_animator.applyRootMotion = true;
                transform.rotation = rightHand.rotation;
                transform.position = rightHand.position;
                //調用MatchTarget方法				
                m_animator.MatchTarget(rightHand.position, rightHand.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0), climbUpMatchStart, climbUpMatchEnd);
            }
            if (m_State.IsName("Base Layer.Climb.ClimbDown"))
            {
                // m_animator.applyRootMotion = true;
                transform.rotation = rightFoot.rotation;
                m_animator.MatchTarget(rightFoot.position, rightFoot.rotation, AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), climbDownMatchStart, climbDownMatchEnd);
            }
        }
    }
    public void Dead()
    {
        if (this != null)
        {
            transform.position = GameManager._instance.SavePoint.position;
            GameManager._instance.DeadCount++;
            isControling = true;
        }
    }

    private void Shoot()
    {
        Vector3 look = RayAim();
        if (Input.GetButton("Fire1"))
        {
            SmoothRotation(Camera.main.transform.eulerAngles.y);
            intervalTime -= Time.deltaTime;
            if (intervalTime <= 0)
            {
                intervalTime = t;
                m_animator.SetTrigger("Shoot");
                GameObject bullet = Instantiate(projectile, muzzle.position, muzzle.rotation) as GameObject;
                bullet.transform.LookAt(look,Vector3.up);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                Physics.IgnoreCollision(rb.GetComponent<Collider>(), this.GetComponent<Collider>());
                rb.velocity = bullet.transform.forward * throwerPower;
                
            }
        }
        if (Input.GetButtonUp("Fire1"))
        { intervalTime = 0; }
    }

    Vector3 GroundCheck()
    {
        Vector3 targetPoint;
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        if (Physics.Raycast(ray, out hit, maxDistatnce, ~layermask))//如果射線碰撞到物體
        {
            targetPoint = hit.point;//記錄碰撞的目標點
            // Debug.Log(hit.collider.name);
        }
        else//射線沒有碰撞到目標點
        {
            //將目標點設置在攝像機自身前方1000米處
            targetPoint = -transform.up * maxDistatnce;
        }
        return targetPoint;
    }
    #endregion

    #region public function
    public void SmoothRotation(float a)
    {
        float y = 3.0f;
        float rotateSpeed = 0.05f;
        transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, a, ref y, rotateSpeed), 0);
    }
    public void ResetVelocity()//重設加速度
    { m_rigidBody.velocity = Vector3.zero; }
    public void UseGravity(bool isuseGravity)
    { m_rigidBody.useGravity = isuseGravity; }
    public void OutClimbState()
    {
        rightHand = null;
        rightFoot = null;
    }
    public void LockPlayerControl()
    {
        isControling = false;
    }

    public void UnlockPlayerControl()
    { isControling = true; }
    public Vector3 RayAim()
    {
        Vector3 targetPoint;
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), Camera.main.transform.TransformDirection(Vector3.forward));

        if (Physics.Raycast(ray, out hit, maxDistatnce, ~layermask))//如果射線碰撞到物體
        {
            targetPoint = hit.point;//記錄碰撞的目標點
            Debug.Log(hit.collider.name);
        }
        else//射線沒有碰撞到目標點
        {
            //將目標點設置在攝像機自身前方1000米處
            targetPoint = Camera.main.transform.forward * maxDistatnce;
        }
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), Camera.main.transform.TransformDirection(Vector3.forward) * maxDistatnce, Color.red);
        return targetPoint;
    }

    public void ChangePlayerPosion(Vector3 pos)
    {
        transform.position = pos;
    }
    #endregion
}
