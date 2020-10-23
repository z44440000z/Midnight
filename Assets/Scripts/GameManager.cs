//游戏管理
using UnityEngine;

public enum GameState
{
    Running,
    Pause
}
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public GameState gamestate = GameState.Running;//游戏状态，包括运行暂停

    public Transform SavePoint;
    [SerializeField] private int ringCount = 0;
    [SerializeField] private int clearCount;
    private UI ui;

    [HideInInspector]
    public int nowRingCount
    {
        get { return ringCount; }
        set
        {
            ringCount = value;
            ui.GetClassCondition();
        }
    }
    [HideInInspector]
    public int maxRingCount
    {
        get { return clearCount; }
        set { maxRingCount = value; }
    }


    void Awake()
    {
        DontDestroyOnLoad(this);
        _instance = this;
        // CursorControl(false);
        ui = GetComponent<UI>();
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
        if(_instance.isActiveAndEnabled)
        {
            if (gamestate == GameState.Running)
            {
                Time.timeScale = 0;
                gamestate = GameState.Pause;
                CursorControl(true);
            }
            else if (gamestate == GameState.Pause)
            {
                Time.timeScale = 1;
                gamestate = GameState.Running;
                CursorControl(false);
            }
        }
    }
    public void CursorControl(bool isCursorVisible)
    {
        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void Save(Transform SP)
    {
        SavePoint = SP;
    }

    public void AddRing()
    {
        nowRingCount++;
    }
}