using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class CharacterControls : MonoBehaviour
{

    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float VelocityLimit = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;
    Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = _rigidbody.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -VelocityLimit, VelocityLimit);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -VelocityLimit, VelocityLimit);
            velocityChange.y = 0;
            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (Input.GetButton("Jump"))
            {
                _rigidbody.linearVelocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }


            grounded = false;
        }

	}
        float CalculateJumpVerticalSpeed()
        {
            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }
    
}
