using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToNext : MonoBehaviour
{
    public AudioSource m_audio;
    public AudioClip clip_pass;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(other.collider.tag =="Player")
        {
            GameManager._instance.ShowWin();
            other.gameObject.GetComponent<SimpleCharacterControl>().LockPlayerControl();
            m_audio.PlayOneShot(clip_pass);
        }
    }
}
