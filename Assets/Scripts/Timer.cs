using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    bool isPause;
    float timer_f;
    int timer_i;
    public int minute;
    public int second;
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (!isPause)
        {
            timer_f += Time.deltaTime;
            timer_i = (int)timer_f;
            turn_time();
        }
        else
        { }
    }
    void turn_time()
    {
        minute = timer_i / 60;
        second = timer_i % 60;
    }
    public void TimeReset()
    {
        timer_f = 0f;
        timer_i = 0;
    }

    public void SetTime(float time_f)
    {
        timer_f = time_f;
        timer_i = (int)timer_f;
        turn_time();
    }

    public float GetTime()
    {
        return timer_f;
    }
}
