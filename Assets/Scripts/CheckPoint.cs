using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Animator finalaniomator;
    public MeshRenderer drink_sign;
    public Material M_drink_picture_B;

    public AudioSource m_audio;
    public AudioClip clip_rise;
    public AudioClip clip_notEnough;
    // Start is called before the first frame update
    void Start()
    {
        m_audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Bullet")
        {
            if (GameManager._instance.CheckRing())
            {
                //升起電梯
                m_audio.PlayOneShot(clip_rise);
                finalaniomator.SetBool("Up", true);
                //換招牌
                drink_sign.material = M_drink_picture_B;
            }
            else
            { m_audio.PlayOneShot(clip_notEnough); }
        }

    }
}
