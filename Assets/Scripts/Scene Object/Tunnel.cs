using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
    [SerializeField] private float y = -30;
    [SerializeField] private bool isOn;
    private Vector3 finalPos;
    private Vector3 originPos;
    private float timers = 0;
    private float speed = 5;

    private void Awake()
    {
        finalPos = transform.position;
        originPos = transform.position = new Vector3(transform.position.x, y, transform.position.z);

    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager._instance.onReset += new GameManager.ManipulationHandler(Reset);
    }

    // Update is called once per frame
    void Update()
    {
        if (timers <= 1 && isOn)
        {

            transform.position = Vector3.Lerp(transform.position, finalPos, timers);
            timers += Time.deltaTime / speed;
        }
        else if (timers >= 0 && !isOn)
        {
            transform.position = Vector3.Lerp(originPos, transform.position, timers);
            timers -= Time.deltaTime / speed;
        }
        else
        {

        }
    }
    public void swicthOn(bool isOn)
    {
        this.isOn = isOn;
    }

    private void Reset()
    {
        transform.position = originPos;
        y = -30;
        timers = 0;
        isOn = false;
    }
}
