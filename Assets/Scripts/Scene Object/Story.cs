using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Story : MonoBehaviour
{
    [Header("對話")]
    public Flowchart flowchart;
    private GameObject sayDialog;
    // Start is called before the first frame update
    void Start()
    {
        sayDialog = GetComponentInChildren<SayDialog>().gameObject;
        GameManager._instance.onSwitchGameState += new GameManager.TransformGameStateHandler(ShowAndHideSayDialog);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        { flowchart.ExecuteBlock("New Game"); }
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
}
