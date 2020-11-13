using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetter : MonoBehaviour
{
    public SceneData sceneData;
    public Ring[] ringObj;
    // Start is called before the first frame update
    private void Start()
    {
        if (sceneData.ringDataArray.Length == 0)
        {
            ringObj = GameObject.FindObjectsOfType<Ring>();
            //儲存場景、收集物資訊
            sceneData.ringDataArray = new RingData[ringObj.Length];
            for (var i = 0; i < ringObj.Length; i++)
            {
                sceneData.ringDataArray[i].x = ringObj[i].transform.position.x;
                sceneData.ringDataArray[i].y = ringObj[i].transform.position.y;
                sceneData.ringDataArray[i].z = ringObj[i].transform.position.z;
            }
            Debug.Log("Saved Scene");
        }
        else
        {
            ringObj = GameObject.FindObjectsOfType<Ring>();
            //讀取資料
            for (var i = 0; i < ringObj.Length; i++)
            {
                if (sceneData.ringDataArray[i].isGet)
                { ringObj[i].gameObject.SetActive(false); }
                else
                { ringObj[i].transform.position = new Vector3(sceneData.ringDataArray[i].x, sceneData.ringDataArray[i].y, sceneData.ringDataArray[i].z); }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
