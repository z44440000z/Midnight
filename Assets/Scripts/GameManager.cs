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
    public CameraController cc;
    public GameState gamestate = GameState.Pause;//游戏状态，包括运行暂停

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
        if (_instance != null)
        { Destroy(this.gameObject); }
        else
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
        CursorControl(true);
        ui = GetComponent<UI>();
        cc = FindObjectOfType<CameraController>();
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
            MenuManager.instance.isAct = true;
            Time.timeScale = 0;
            CursorControl(true);
            gamestate = GameState.Pause;

        }
        else if (gamestate == GameState.Pause)
        {
            MenuManager.instance.isAct = false;
            Time.timeScale = 1;
            CursorControl(false);
            gamestate = GameState.Running;

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
        MenuManager.instance.SetSaveScene();
    }

    public void AddRing()
    {
        nowRingCount++;
    }

    public bool CheckRing()
    {
        if (ringCount == clearCount)
        { return true; }
        else if (ringCount <= clearCount)
        { return false; }
        else
        {
            ringCount = clearCount;
            return true;
        }
    }

    public void ShowWin()
    {
        ui.win.gameObject.SetActive(true);
    }
}