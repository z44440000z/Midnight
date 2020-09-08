using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Loading場景的UI動畫
public class loading : MonoBehaviour
{
    [SerializeField] RectTransform _key;
    [SerializeField] Image _word;
    float mathf;
	float speed = 100;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mathf = Mathf.PingPong(Time.time, 180);
        _key.rotation = Quaternion.Euler(mathf * speed, 0, 0);
        _word.color = new Color(1, 1, 1, Mathf.PingPong(Time.time, 1));
    }
}
