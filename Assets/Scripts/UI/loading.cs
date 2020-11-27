using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEngine.Playables;

//Loading場景的UI動畫
public class Loading : MonoBehaviour
{
    public Animator animator;
    public TimelineAsset timeline;
    public PlayableDirector playableDirector;
    public float transitionTime = 1f;

    // Use this for initialization
    void Awake()
    {
        transitionTime = (float)timeline.duration + 0.5f;
    }

    // Update is called once per frame
    void Update()
    { }

    public bool IfPlayFinish()
    {
        if ((  playableDirector.time) < timeline.duration)
        { return true; }
        else
        { return false; }
    }
}
