using UnityEngine;
using System.Collections;


public class RotateMyself : MonoBehaviour
{
    public enum dynamic
    {
        Rotate,
        Drift
    }
    [SerializeField]
    private dynamic _dynamic;

    //X軸、Y軸、Z軸的旋轉速度 : 浮點數值
    public float X = 0;
    public float Y = 0;
    private float originY;
    //public float Z = 0;
    //宣告三軸的旋轉量是以每秒乘以各軸的速度 --> 以旋轉量持續旋轉各軸向。

    void Start()
    { originY = transform.position.y; }

    void Update()
    {
        switch (_dynamic)
        {
            case dynamic.Rotate:
                {
                    float XRotate = X * Time.deltaTime;
                    //float YRotate = Y *Time.deltaTime;
                    //float ZRotate = Z *Time.deltaTime;
                    transform.Rotate(XRotate, 0, 0);
                }
                break;
            case dynamic.Drift:
                {
                    transform.position = new Vector3(transform.position.x, originY + Mathf.PingPong(Time.time, Y), transform.position.z);
                }
                break;
        }
    }
}
