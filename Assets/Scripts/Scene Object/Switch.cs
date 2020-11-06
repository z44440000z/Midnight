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
    private Tunnel[] turnOffArray;
    private int tunnelsCount;
    // Start is called before the first frame update
    void Start()
    {
        GameManager._instance.onReset += new GameManager.ManipulationHandler(Reset);
        tunnelsCount = turnOnTunnels.childCount;
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
                turnOnArray[i] = turnOffTunnels.GetChild(i).GetComponent<Tunnel>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        { switchOn = true; }
        if (switchOn)
        {
            if (turnOnTunnels)
            { StartCoroutine("TunnelSwicthOn"); }
            if (turnOffTunnels)
            { StartCoroutine("TunnelSwicthOff"); }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Bullet")
        {
            if (turnOnTunnels)
            { StartCoroutine("TunnelSwicthOn"); }
            if (turnOffTunnels)
            { StartCoroutine("TunnelSwicthOff"); }
        }
    }

    IEnumerator TunnelSwicthOn()
    {
        foreach (Tunnel item in turnOnArray)
        {
            item.swicthOn(true);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return null;
    }
    IEnumerator TunnelSwicthOff()
    {
        foreach (Tunnel item in turnOffArray)
        {
            item.swicthOn(false);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return null;
    }

    private void Reset()
    {
        switchOn = false;
        this.gameObject.SetActive(true);
    }
}