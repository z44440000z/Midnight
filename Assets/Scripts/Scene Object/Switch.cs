using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Transform turnOnTunnels;
    [SerializeField] private Transform turnOffTunnels;

    [SerializeField] private float intervalTime = 0.1f;
    [SerializeField] private bool switchOn = false;
    [SerializeField] private Tunnel[] turnOnArray;
    [SerializeField] private Tunnel[] turnOffArray;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource m_audio;

    // Start is called before the first frame update
    void Start()
    {
        GameManager._instance.onReset += new GameManager.ManipulationHandler(Reset);
        //打開
        if (turnOnTunnels)
        {
            turnOnArray = new Tunnel[turnOnTunnels.childCount];
            for (int i = 0; i < turnOnTunnels.childCount; i++)
            {
                turnOnArray[i] = turnOnTunnels.GetChild(i).GetComponent<Tunnel>();
            }
        }
        //關閉
        if (turnOffTunnels)
        {
            turnOffArray = new Tunnel[turnOffTunnels.childCount];
            for (int i = 0; i < turnOffTunnels.childCount; i++)
            {
                turnOffArray[i] = turnOffTunnels.GetChild(i).GetComponent<Tunnel>();
            }
        }
        animator = GetComponentInChildren<Animator>();
        m_audio = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (switchOn == !switchOn)
        {
            StartCoroutine("TunnelSwicthOn");
            animator.SetBool("ON", true);
        }
        // else
        // { StartCoroutine("TunnelSwicthOff"); }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Bullet")
        {
            switchOn = !switchOn;
            if (switchOn)
            {
                StartCoroutine("TunnelSwicthOn");
                animator.SetBool("ON", true);
            }
            // else
            // { StartCoroutine("TunnelSwicthOff"); }
        }
    }

    IEnumerator TunnelSwicthOn()
    {
        m_audio.Play();
        if (turnOnTunnels)
        {
            foreach (Tunnel item in turnOnArray)
            {
                item.swicthOn(true);
                yield return new WaitForSeconds(intervalTime);
            }
        }
        if (turnOffTunnels)
        {
            foreach (Tunnel item in turnOffArray)
            {
                item.swicthOn(false);
                yield return new WaitForSeconds(intervalTime);
            }
        }
        yield return null;
    }
    IEnumerator TunnelSwicthOff()
    {
        if (turnOnTunnels)
        {
            foreach (Tunnel item in turnOnArray)
            {
                item.swicthOn(false);
                yield return new WaitForSeconds(intervalTime);
            }
        }
        if (turnOffTunnels)
        {
            foreach (Tunnel item in turnOffArray)
            {
                item.swicthOn(true);
                yield return new WaitForSeconds(intervalTime);
            }
        }
        yield return null;
    }

    private void Reset()
    {
        if (this != null)
        {
            switchOn = false;
            animator.SetBool("ON", false);
            this.gameObject.SetActive(true);
        }
    }
}