using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int index;
    public bool isGet = false;
    AudioSource m_audio;
    // Start is called before the first frame update
     private void Start() 
    {
        m_audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsGet();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
        { this.gameObject.SetActive(false); }
        else
        { this.gameObject.SetActive(true); }
    }
}
