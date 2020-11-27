using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Loading場景的UI動畫
public class Loading : MonoBehaviour
{
    public Animator animator;
    public float transitionTime = 1f;
    public Text loadText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // mathf = Mathf.PingPong(Time.time, 180);
        loadText.color = new Color(1, 1, 1, Mathf.PingPong(Time.time, 1));
    }
}
