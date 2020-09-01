using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//推進物(Boss用)
public class DestoryPS : MonoBehaviour
{

    private ParticleSystem ps;
    private BoxCollider _collision;
    public float speed = 30.0f;
    public float Damage = 0;

    public bool trigger_destory = false;

    [Header("Change Trigger Scale")]
    public bool If_change = true;
    public Vector3 maxscale = new Vector3(3, 0.3f, 3);
    void Start()
    {
        ps = this.GetComponentInChildren<ParticleSystem>();//取得粒子
        _collision = this.GetComponent<BoxCollider>();

    }

    void Update()
    {
        transform.Translate(0, 0, Time.deltaTime * speed);
        if (ps.IsAlive() == false)//判斷粒子是否存活
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (If_change)
            _collision.size = Vector3.Lerp(_collision.size, maxscale, Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (other.gameObject.tag == "Player")
        { other.gameObject.GetComponent<Player_Health>().NowHP -= Damage; }
        if (trigger_destory)
        { Destroy(this.gameObject); }
    }
}
