using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private float destoryTime = 1;
    [SerializeField] private float upForce = 1;
    [SerializeField] private bool isFloat = false;
    [SerializeField] private bool isDestory = false;

    private Rigidbody mrigidBody;
    private Vector3 originPos;
    private Vector3 startPos;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            t = 0;
            isFloat = false;
            // collision.transform.parent = this.transform;
            StartCoroutine("Countdown");
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Dead"))
        { Destroy(this.gameObject); }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            t = 0;
            isFloat = true;
            startPos = transform.position;
            collision.transform.parent = null;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            mrigidBody.velocity = Vector3.zero;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        mrigidBody = GetComponent<Rigidbody>();
        originPos = transform.position;
    }
    float t = 0;
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!isDestory)
        {
            if (isFloat)
            {

                if (Vector3.Distance(startPos, originPos) < 0.5f)
                { mrigidBody.velocity = Vector3.zero; }
                else
                {
                    if (t < 1)
                    {
                        t += Time.deltaTime / 5;
                        mrigidBody.MovePosition(Vector3.Lerp(startPos, originPos, t));
                    }
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, originPos) < 0.5f)
                { }
                else { UpForce(upForce); }
            }
        }
        else
        { transform.Translate(Vector3.down * upForce * Time.deltaTime, Space.World); }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(destoryTime);
        isDestory = true;
    }

    void UpForce(float forcePower)
    {
        mrigidBody.AddForce(Vector3.up * forcePower , ForceMode.Force);
    }
}
