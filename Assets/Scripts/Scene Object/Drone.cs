using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private float destoryTime = 1;
    [SerializeField] private float upForce = 1;
    [SerializeField] private bool isOn = false;
    [SerializeField] private bool isDestory = false;

    private Rigidbody mrigidBody;
    private Animator mAnimator;
    private Vector3 originPos;
    private Vector3 shakePos;
    [SerializeField] private float shakeDistance = 1;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            isOn = false;
            StartCoroutine("Countdown");
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Dead"))
        { this.gameObject.SetActive(false); }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            t = 0;
            isOn = true;
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
        shakePos = originPos + new Vector3(0, -shakeDistance, 0);
        mAnimator = GetComponent<Animator>();
        GameManager._instance.onReset += new GameManager.ManipulationHandler(Reset);
    }
    float t = 0;
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!isDestory)
        {
            if (isOn)
            { }
            else
            {
                if (Vector3.Distance(transform.position, originPos) < 0.5f)
                { }
                else { }
            }
        }
        else
        { transform.Translate(Vector3.down * upForce * Time.deltaTime, Space.World); }
    }

    IEnumerator Countdown()
    {
        t = 0;
        yield return new WaitForSeconds(destoryTime);
        transform.position = Vector3.Lerp(transform.position, shakePos, t);
        mAnimator.SetBool("Down", true);

        //Get hash of animation
        int animHash = Animator.StringToHash("Base Layer.robot_drop");
        //Wait until we enter the current state
        while (mAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash)
        {
            yield return null;
        }

        float counter = 0;
        float waitTime = mAnimator.GetCurrentAnimatorStateInfo(0).length;

        //Now, Wait until the current state is done playing
        while (counter < (waitTime))
        {
            counter += Time.deltaTime;
            yield return null;
            isDestory = true;
        }
    }

    void UpForce(float forcePower)
    {
        mrigidBody.AddForce(Vector3.up * forcePower, ForceMode.Force);
    }

    private void Reset()
    {
        if (this != null)
        {
            StopCoroutine("Countdown");
            isOn = false;
            isDestory = false;
            mAnimator.SetBool("Down", false);
            transform.position = originPos;
            this.gameObject.SetActive(true);
        }
    }
}
