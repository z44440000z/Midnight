using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    bool isPause;
    float timer_f = 0f;
    int timer_i = 0;
    public int minute = 0;
    public int second = 0;
    private void Start()
    {
        SceneManager.activeSceneChanged += TimeReset;
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
    public void TimeReset(Scene current, Scene next)
    {
        timer_f = 0f;
        timer_i = 0;
        minute = 0;
        second = 0;
    }
}
