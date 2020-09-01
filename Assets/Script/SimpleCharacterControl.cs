using UnityEngine;
using System.Collections.Generic;

public class SimpleCharacterControl : MonoBehaviour
{

    private enum ControlMode
    { Direct }

    [SerializeField] private float m_moveSpeed = 5;
    [SerializeField] private float m_jumpForce = 10;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 1f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

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
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0)
        { m_isGrounded = false; }
    }

    void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
    }

    private void DirectUpdate()
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

        //飛行
        Flying();
      
        //跳躍
        JumpingAndLanding();
    }

    public void SmoothRotation(float a)
    {
        float y = 3.0f;
        float rotateSpeed = 0.05f;
        transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, a, ref y, rotateSpeed), 0);
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_animator.GetInteger("JumpCount") < 2 && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            m_animator.SetTrigger("Jump");

            int jc = m_animator.GetInteger("JumpCount") + 1;
            m_animator.SetInteger("JumpCount", jc);
        }
    }

    private void Flying()
    {
        if (!m_isGrounded && Input.GetMouseButton(1))
        { m_animator.SetBool("Fly", true); }
        else
        { m_animator.SetBool("Fly", false); }

        if (m_animator.GetBool("Fly"))
        { m_rigidBody.AddForce(transform.forward * m_moveSpeed, ForceMode.Impulse); }
    }
}
