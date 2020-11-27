using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Animator finalaniomator;
    public Animation drink_sign_animation;
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
        if (other.collider.tag == "Bullet")
        {
            if (GameManager._instance.CheckRing())
            {
                //升起電梯
                finalaniomator.SetBool("Up", true);
                //換招牌
                drink_sign_animation.Play("drink_sign_switch");
            }
            else
            { }
        }

    }
}
