using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int index;
    public bool isGet = false;
    // Start is called before the first frame update
    void Start()
    {
        CheckIsGet();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager._instance.AddRing();
            isGet = true;
            CheckIsGet();
        }
    }

    void CheckIsGet()
    {
        if (isGet)
        { this.gameObject.SetActive(false); }
        else
        { this.gameObject.SetActive(true); }
    }
}
