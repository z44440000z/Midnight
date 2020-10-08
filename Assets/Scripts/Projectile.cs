using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float timeToLive = 3f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void Awake()
    {
        Destroy(this.gameObject, timeToLive);
        if (transform.parent != null)
        { Destroy(this.transform.parent.gameObject, timeToLive); }
    }
    private void OnCollisionEnter(Collision other)
    {
        Destroy(this.gameObject);
        if (transform.parent != null)
        { Destroy(this.transform.parent.gameObject); }
    }
}
