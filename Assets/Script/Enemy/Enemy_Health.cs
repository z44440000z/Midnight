using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_Health : MonoBehaviour
{

    // Use this for initialization
    public float _health = 1000;
    public int max_health = 1000;
    public float Die_time = 3;
    public ParticleSystem Die_particle;
    private AI _ai;

    // Use this for initialization
    void Start()
    {    }

    // Update is called once per frame
    void Update()
    {    }
}

