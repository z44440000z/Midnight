using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int index;
    public bool isGet = false;
    public GameObject powerObj;
    public AudioSource m_audio;
    // Start is called before the first frame update
    private void Start()
    {
        m_audio = GetComponent<AudioSource>();
        powerObj = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsGet();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isGet)
        {
            GameManager._instance.AddRing();
            isGet = true;
            m_audio.Play();
            CheckIsGet();
        }
    }

    void CheckIsGet()
    {
        if (isGet)
        { powerObj.SetActive(false); }
        else
        { powerObj.SetActive(true); }
    }
}
