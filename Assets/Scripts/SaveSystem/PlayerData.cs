using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class PlayerData
{
    public float x, y, z;
    public string sceneName;
    public int score = 0;
}
[System.Serializable]
public class RingData
{
    public bool isGet = false;
}