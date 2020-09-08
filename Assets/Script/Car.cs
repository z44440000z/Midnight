using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    float alpha = 0;
    
    public enum direction
    {
        x, y, z
    };
    public direction Axial;
    public float length = 3;
    public float speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        alpha += speed;
        switch (Axial)
        {
            case direction.x:
                { transform.position = new Vector3(Mathf.PingPong(alpha, length), transform.position.y, transform.position.z); }
                break;
            case direction.y:
                { transform.position = new Vector3(transform.position.x, Mathf.PingPong(alpha, length), transform.position.z); }
                break;
            case direction.z:
                { transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.PingPong(alpha, length)); }
                break;
        }
    }
}
