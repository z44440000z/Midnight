using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public Text now;
    public Text max;
    public Canvas win;

    // Start is called before the first frame update
    void Start()
    {
        GetClassCondition();
        SceneManager.activeSceneChanged += CloseWinCanvas;
        
    }

    void CloseWinCanvas(Scene current, Scene next)
    {
        win.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetClassCondition()
    {
        now.text = GameManager._instance.nowRingCount.ToString();
        max.text = GameManager._instance.maxRingCount.ToString();
    }
}
