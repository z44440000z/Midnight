using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform FollowTarget;
    public float Radius = 3.0f;
    public float Smooth = 2.0f;

    private Transform myTransform;

    // Use this for initialization
    void Start()
    {
        if (!FollowTarget)
        { FollowTarget = GameObject.FindWithTag("MenuPos").transform; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!FollowTarget)
        { FollowTarget = GameObject.FindWithTag("MenuPos").transform; }
        transform.position = FollowTarget.position;
    }
}
