using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [Header("Point")]
    public GameObject point;
    public Text now;
    public Text max;
    [Header("Timer")]
    public Text timerText;
    Timer timer;
    [Header("Win Panel")]
    public GameObject winPanel;
    public Text time;
    public Text count;

    // Start is called before the first frame update
    void Start()
    {
        GetClassCondition();
        SceneManager.activeSceneChanged += CloseWinPanel;
        timer = timerText.GetComponent<Timer>();
    }

    void CloseWinPanel(Scene current, Scene next)
    {
        winPanel.gameObject.SetActive(false);
        point.SetActive(true);
        timerText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //印出時間動作
        if (timer.minute < 10)
        {

            if (timer.second < 10)
            { timerText.text = "0" + timer.minute + ":0" + timer.second; }
            else
            { timerText.text = "0" + timer.minute + ":" + timer.second; }
        }
        else
        {
            if (timer.second < 10)
            { timerText.text = timer.minute + ":0" + timer.second; }
            else
            { timerText.text = timer.minute + ":" + timer.second; }
        }
    }

    public void UI_ShowWin()
    {
        winPanel.gameObject.SetActive(true);
        point.SetActive(false);
        timerText.gameObject.SetActive(false);
        //通關時間
        if (timer.minute < 10)
        {

            if (timer.second < 10)
            { time.text = "通關時間：0" + timer.minute + ":0" + timer.second; }
            else
            { time.text = "通關時間：0" + timer.minute + ":" + timer.second; }
        }
        else
        {
            if (timer.second < 10)
            { time.text = "通關時間：" + timer.minute + ":0" + timer.second; }
            else
            { time.text = "通關時間：" + timer.minute + ":" + timer.second; }
        }
        //死亡次數
        count.text = "死亡次數：" + GameManager._instance.DeadCount.ToString();
    }

    public void GetClassCondition()
    {
        now.text = GameManager._instance.nowRingCount.ToString();
        max.text = GameManager._instance.maxRingCount.ToString();
    }
}
