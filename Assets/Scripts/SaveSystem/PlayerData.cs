using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class PlayerData
{
    public float x, y, z;
    public string sceneName;
    public int score = 0;
    public int minute = 0;
    public int second = 0;
    public float time;
    public RingData[] ringDataArray;
}