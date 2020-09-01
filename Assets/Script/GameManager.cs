//游戏管理
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public enum GameState
{
    Running,
    Pause
}
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public GameState gamestate = GameState.Running;//游戏状态，包括运行暂停


    void Awake()
    {
        _instance = this;
    }
    //测试添加删除物品
    private void Update()
    {
        //按ESC暂停游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _instance.TransformGameState();
        }
    }
    //改变游戏的运行状态，运行与暂停
    public void TransformGameState()
    {
        if (gamestate == GameState.Running)
        {
            Time.timeScale = 0;
            gamestate = GameState.Pause;
        }
        else if (gamestate == GameState.Pause)
        {
            Time.timeScale = 1;
            gamestate = GameState.Running;
        }
    }
}