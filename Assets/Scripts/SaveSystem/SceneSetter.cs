using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetter : MonoBehaviour
{
    public SceneData sceneData;
    public Ring[] ringObj;
    // Start is called before the first frame update
    private void Start()
    {
        // SceneManager.sceneLoaded += LoadNewScene;
        GameManager._instance.maxRingCount = sceneData.RingCount;
    
        ringObj = new Ring[sceneData.RingCount];
        Ring[] rA = GameObject.FindObjectsOfType<Ring>();
        //儲存場景、收集物資訊
        for (var i = 0; i < ringObj.Length; i++)
        {
            ringObj[i] = rA[i];
            ringObj[i].index = i;
        }
        Debug.Log("Load Scene Data");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
