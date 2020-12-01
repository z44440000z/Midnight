using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetter : MonoBehaviour
{
    public SceneData sceneData;
    public Ring[] ringObj;

    private void Awake()
    {
        GameManager._instance.maxRingCount = sceneData.RingCount;
    }
    private void OnEnable()
    { }
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LoadSceneSetting());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        { ringObj = GameObject.FindObjectsOfType<Ring>(); }
    }

    IEnumerator LoadSceneSetting()
    {
        ringObj = GameObject.FindObjectsOfType<Ring>();
        if (ringObj.Length != 0)
        {
            //場景讀檔
            if (MenuManager.instance.CheckedIfContinueScene())
            {
                PlayerData data = new PlayerData();
                while (data.ringDataArray == null)
                {
                    data = SaveSystem.Load();
                    yield return null;
                }
                RingData[] r = data.ringDataArray;
                for (var i = 0; i < sceneData.RingCount; i++)
                {
                    ringObj[i].index = data.ringDataArray[i].index;
                    ringObj[i].isGet = data.ringDataArray[i].isGet;
                    ringObj[i].transform.position = new Vector3(data.ringDataArray[i].x, data.ringDataArray[i].y, data.ringDataArray[i].z);
                }
                SaveSystem.Save(data);
            }
            else
            {
                //儲存場景、收集物資訊
                for (var i = 0; i < ringObj.Length; i++)
                {
                    ringObj[i].index = i;
                }
                Debug.Log("加載新的場景");
            }
        }
        yield return null;
    }
}
