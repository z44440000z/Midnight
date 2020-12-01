using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Story : MonoBehaviour
{
    [Header("對話")]
    public Flowchart flowchart;
    public string blockName;
    private GameObject sayDialog;
    public GameObject StartPoint;
    public GameObject Story_Vcam;
    private SimpleCharacterControl player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager._instance.player;
        sayDialog = GetComponentInChildren<SayDialog>().gameObject;
        GameManager._instance.onSwitchGameState += new GameManager.TransformGameStateHandler(ShowAndHideSayDialog);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Say();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Story_Vcam.SetActive(true);
            player.LockPlayerControl();
            flowchart.ExecuteBlock(blockName);
        }
    }
    public void Say()
    {
        Story_Vcam.SetActive(true);
        player.LockPlayerControl();
        flowchart.ExecuteBlock(blockName);//播放到一半突然停止???
    }

    public void ToNewGame()
    {
        MenuManager.instance.NewGameButton();
    }

    void ShowAndHideSayDialog()
    {
        if (this != null)
        {
            if (GameManager._instance.gamestate == GameState.Pause)
            { sayDialog.GetComponent<Canvas>().enabled = false; }
            if (GameManager._instance.gamestate == GameState.Running)
            { sayDialog.GetComponent<Canvas>().enabled = true; }
        }
    }
    private void OnDestroy()
    {
        if (player != null)
        { player.UnlockPlayerControl(); }
        else
        { }
        if (StartPoint != null)
        { StartPoint.SetActive(true); }
    }
}
