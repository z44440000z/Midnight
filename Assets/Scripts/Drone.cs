using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private float destoryTime = 1;
    [SerializeField] private float upForce = 1;
    [SerializeField] private bool isAbove = false;
    [SerializeField] private bool isDestory = false;


    private Rigidbody mrigidBody;
    private Vector3 originPos;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            isAbove = true;
            collision.transform.parent = this.transform;
            StartCoroutine("Countdown");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            isAbove = false;
            collision.transform.parent = null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        mrigidBody = GetComponent<Rigidbody>();
        originPos = transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!isDestory)
        {
            if (isAbove)
            {
                UpForce(upForce);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, originPos, 0.05f);
            }
        }
        else
        { transform.Translate(Vector3.back); }
    }

    IEnumerator Countdown()
    {
        //transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 0.1F), transform.position.z);
        yield return new WaitForSeconds(destoryTime);
        isDestory = true;
    }

    void UpForce(float forcePower)
    {
        mrigidBody.AddForce(Vector3.up * forcePower * mrigidBody.mass, ForceMode.Force);
    }
}
