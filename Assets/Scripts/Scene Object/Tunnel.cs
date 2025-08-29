using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
    [SerializeField] private float hiddenY = -30f;  // 隱藏時的 Y 座標
    [SerializeField] private bool isOn;
    [SerializeField] private float speed = 5f;

    private Vector3 finalPos;   // 最後要到的位置
    private Vector3 originPos;  // 起始隱藏的位置
    private float timers = 0f;  // 0~1 的插值時間

    private void Awake()
    {
        finalPos = transform.position;
        originPos = new Vector3(transform.position.x, hiddenY, transform.position.z);

        // 一開始先隱藏
        transform.position = originPos;
    }

    private void Start()
    {
        GameManager._instance.onReset += Reset;
    }

    private void Update()
    {
        // 根據 isOn 來決定 timers 往前還是往後
        if (isOn)
        {
            timers = Mathf.Clamp01(timers + Time.deltaTime / speed);
        }
        else
        {
            timers = Mathf.Clamp01(timers - Time.deltaTime / speed);
        }

        // 在 origin 和 final 之間做插值
        transform.position = Vector3.Lerp(originPos, finalPos, timers);
    }

    public void SwitchOn(bool value)
    {
        isOn = value;
    }

    private void Reset()
    {
        if (this == null)
        {
            return;
        }
        transform.position = originPos;
        timers = 0f;
        isOn = false;
    }
}
