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
    public Vector3 StartPosition;
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

    public int DeadCount = 0;
    public Timer GameTimer;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = this;
        }
        else
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
        CursorControl(true);
        ui = GetComponent<UI>();
        cc = FindObjectOfType<CameraController>();
        GameTimer = transform.GetComponentInChildren<Timer>();
        StartPosition = transform.position;
    }
    private void Start()
    {
        MenuManager.instance.isAct = true;
        Time.timeScale = 0;
        CursorControl(true);
        gamestate = GameState.Pause;
    }
    //测试添加删除物品
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        { nowRingCount = maxRingCount; }
        //按ESC暂停游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _instance.TransformGameState();
        }
        if (gamestate == GameState.Running)
        { CursorControl(false); }
        else if (gamestate == GameState.Pause)
        { CursorControl(true); }
    }
    //改变游戏的运行状态，运行与暂停
    public void TransformGameState()
    {
        if (gamestate == GameState.Running)
        {
            MenuManager.instance.isAct = true;
            Time.timeScale = 0;
            gamestate = GameState.Pause;
        }
        else if (gamestate == GameState.Pause)
        {
            MenuManager.instance.isAct = false;
            Time.timeScale = 1;
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
        //記憶存檔點位置(暫時)
        SavePoint = SP;
        MenuManager.instance.SetSaveScene();
        //轉換當前參數為PlayerData
        PlayerData data = new PlayerData();
        data.x = SP.position.x;
        data.y = SP.position.y;
        data.z = SP.position.z;
        data.sceneName = MenuManager.instance.GetSaveScene();
        data.score = ringCount;
        data.minute = GameTimer.minute;
        data.second = GameTimer.second;
        //存檔
        SaveSystem.Save(data);
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
        Time.timeScale = 0;
        CursorControl(true);
        gamestate = GameState.Pause;
        ui.UI_ShowWin();
    }

    //發布&監聽
    public delegate void ManipulationHandler();
    public event ManipulationHandler onReset;
    protected virtual void OnPlayerDead()
    {
        if (onReset != null)
        {
            onReset(); /* 事件被触发 */
        }
        else
        {
            Debug.LogError("event not fire");
        }
    }

    public void Dead()
    {
        OnPlayerDead();
    }
}