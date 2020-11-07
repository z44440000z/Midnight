using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmbark : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        { collision.transform.parent = this.transform; }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        { collision.transform.parent = null; }
    }
}
