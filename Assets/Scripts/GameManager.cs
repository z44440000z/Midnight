//遊戲管理
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Running,
    Pause,
    Win
}
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public SimpleCharacterControl player;
    public GameState gamestate = GameState.Pause;//遊戲狀態，包括運行暫停
    public Vector3 StartPosition;
    public Transform SavePoint;
    [SerializeField] private int ringCount = 0;
    [SerializeField] private int clearCount;
    public UI ui;

    [HideInInspector]
    public int nowRingCount
    {
        get { return ringCount; }
        set
        { ringCount = value; ui.GetClassCondition(); }
    }
    [HideInInspector]
    public int maxRingCount
    {
        get { return clearCount; }
        set { clearCount = value; ui.GetClassCondition(); }
    }

    public int DeadCount = 0;
    public Timer GameTimer;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
        CursorControl(true);
        ui = GetComponent<UI>();
        GameManager._instance.player = FindObjectOfType<SimpleCharacterControl>();
        GameTimer = transform.GetComponentInChildren<Timer>();
        StartPosition = transform.position;
        ui.ShowTimeAndPoint();
    }
    private void Start()
    {
        //開啟遊戲設置
        MenuManager.instance.isAct = true;
        Time.timeScale = 0;
        CursorControl(true);
        gamestate = GameState.Pause;
    }
    private void Update()
    {
        //作弊鍵
        if (Input.GetKeyDown(KeyCode.X))
        { nowRingCount = maxRingCount; }
        //按ESC暫停遊戲
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _instance.TransformGameState();
            _instance.SwitchGameState();
        }
        if (gamestate == GameState.Running)
        { CursorControl(false); }
        else if (gamestate == GameState.Pause)
        { CursorControl(true); }
        else if (gamestate == GameState.Win) 
        { CursorControl(true); }
    }
    //改變運行狀態
    public void TransformGameState()
    {
        //運行
        if (gamestate == GameState.Running)
        {
            MenuManager.instance.isAct = true;
            Time.timeScale = 0;
            gamestate = GameState.Pause;
        }
        //暫停
        else if (gamestate == GameState.Pause)
        {
            MenuManager.instance.isAct = false;
            MenuManager.instance.ResetText();
            Time.timeScale = 1;
            gamestate = GameState.Running;
        }
    }
    //滑鼠控制開關
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
        // SceneManager.CreateScene("SaveScene");
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
        data.time = GameTimer.GetTime();

        //場景存檔
        Ring[] r = GameObject.FindObjectOfType<SceneSetter>().ringObj;
        data.ringDataArray = new RingData[r.Length];
        if (r.Length != 0)
        {
            for (var i = 0; i < r.Length; i++)
            {
                data.ringDataArray[i] = new RingData();
                data.ringDataArray[i].index = r[i].index;
                data.ringDataArray[i].isGet = r[i].isGet;
                data.ringDataArray[i].x = r[i].transform.position.x;
                data.ringDataArray[i].y = r[i].transform.position.y;
                data.ringDataArray[i].z = r[i].transform.position.z;
            }
        }
        //存檔
        SaveSystem.Save(data);
        // SaveSystem.SaveSceneChange(sdata);
    }
    //分數增加
    public void AddRing()
    {
        if (nowRingCount < maxRingCount)
        { nowRingCount++; }
    }
    //確認分數是否達標
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
        CursorControl(true);
        gamestate = GameState.Win;
        player.LockPlayerControl();
        ui.UI_ShowWin();
    }

    //發布&監聽死亡事件
    public delegate void ManipulationHandler();
    public event ManipulationHandler onReset;
    protected virtual void OnPlayerDead()
    {
        if (onReset != null)
        { onReset(); }
        else
        { Debug.LogError("event not fire"); }
    }
    public void Dead()
    { OnPlayerDead(); }

    //發布&監聽遊戲狀態切換
    public delegate void TransformGameStateHandler();
    public event TransformGameStateHandler onSwitchGameState;
    protected virtual void SwitchGameState()
    {
        if (onSwitchGameState != null)
        { onSwitchGameState(); } /* 事件觸發 */
        else
        { }
    }
}