using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SimpleCharacterControl : MonoBehaviour
{
    #region Move variable
    [SerializeField] private float m_moveSpeed = 5;
    [SerializeField] private float m_jumpForce = 10;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;
    private float m_currentV = 0;
    private float m_currentH = 0;
    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 1f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;
    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;
    #endregion
    #region Jump variable
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();
    #endregion
    #region Climb variable
    public Transform rightHand;  //右手著力點
    public Transform rightFoot;  //右腳著力點 
    [SerializeField] private bool isClimbPoint;
    private float climbUpMatchStart = 0.24f;
    private float climbUpMatchEnd = 0.61f;
    private float climbDownMatchStart = 0.01f;
    private float climbDownMatchEnd = 0.34f;
    private AnimatorStateInfo m_State;
    #endregion

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody>();
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
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Dead"))
        { Dead(); }
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

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "ClimbPoint")
        {
            rightHand = null;
            rightFoot = null;
            isClimbPoint = false;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);
        if (!m_State.IsName("Base Layer.Climb.ClimbUp") && !m_State.IsName("Base Layer.Climb.ClimbUp"))
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
        }

        //飛行
        Flying();
        //跳躍
        JumpingAndLanding();
        //攀爬
        Climbing();

        m_wasGrounded = m_isGrounded;
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_animator.GetInteger("JumpCount") < 2 && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            ResetVelocity();
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            m_animator.SetTrigger("Jump");

            int jc = m_animator.GetInteger("JumpCount") + 1;
            m_animator.SetInteger("JumpCount", jc);
        }
    }
    private void Flying()
    {
        if (!isClimbPoint && !m_isGrounded && Input.GetMouseButton(1))
        { m_animator.SetBool("Fly", true); }
        else
        { m_animator.SetBool("Fly", false); }

        if (m_State.IsName("Base Layer.Fly"))
        {
            SmoothRotation(Camera.main.transform.eulerAngles.y);
            Vector3 direct = Camera.main.transform.forward;
            float directionLength = direct.magnitude;
            direct.y = 0;
            direct = direct.normalized * directionLength;

            m_rigidBody.AddForce(-Physics.gravity * 0.2f + direct * m_moveSpeed);
        }
    }
    private void Climbing()
    {
        if (m_animator)
        {
            //獲取動畫狀態
            m_State = m_animator.GetCurrentAnimatorStateInfo(0);

            if (isClimbPoint && Input.GetMouseButtonDown(1))
            {
                if (m_animator.GetBool("Climb"))
                { m_animator.SetBool("Climb", false); }
                else
                {
                    m_animator.SetBool("Climb", true);
                    Vector3.Lerp(transform.position, rightFoot.position, Time.deltaTime * m_interpolation);
                }
            }

            if (m_State.IsName("Base Layer.Climb.ClimbUp"))
            {
                transform.rotation = rightHand.rotation;
                //調用MatchTarget方法				
                m_animator.MatchTarget(rightHand.position, rightHand.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0), climbUpMatchStart, climbUpMatchEnd);
            }
            if (m_State.IsName("Base Layer.Climb.ClimbDown"))
            {
                transform.rotation = rightHand.rotation;
                m_animator.MatchTarget(rightFoot.position, rightFoot.rotation, AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), climbDownMatchStart, climbDownMatchEnd);
            }
        }
    }
    private void Dead()
    {
        transform.position = GameManager._instance.SavePoint.position;
    }
    public void SmoothRotation(float a)
    {
        float y = 3.0f;
        float rotateSpeed = 0.05f;
        transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, a, ref y, rotateSpeed), 0);
    }
    public void ResetVelocity()
    { m_rigidBody.velocity = Vector3.zero; }
    public void UseGravity(bool isuseGravity)
    { m_rigidBody.useGravity = isuseGravity; }
}
