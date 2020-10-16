using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text now;
    public Text max;

    // Start is called before the first frame update
    void Start()
    {
        GetClassCondition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetClassCondition()
    {
        now.text = GameManager._instance.nowRingCount.ToString();
        max.text = GameManager._instance.maxRingCount.ToString();
    }
}
