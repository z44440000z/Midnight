using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{


    public enum direction
    {
        x, y, z
    };
    public direction Axial;
    public float length = 3;
    public float speed = 0.1f;
    public float IntervalTime = 2;


    private Vector3 originPosition;
    private bool IsDisappear = false;
    private float alpha = 0;

    void Start()
    {
        originPosition = transform.position;
    }


    void Update()
    {
        if (!IsDisappear)
        {
            alpha += speed;
            switch (Axial)
            {
                case direction.x:
                    {
                        Vector3 newPosition = originPosition + new Vector3(length, 0, 0);
                        if (Vector3.Distance(transform.position, newPosition) < 0.1f)
                        { StartCoroutine("TimerAndReset"); }
                        else
                        { OnMove(newPosition); }
                    }
                    break;
                case direction.y:
                    {
                        Vector3 newPosition = originPosition + new Vector3(0, length, 0);
                        if (Vector3.Distance(transform.position, newPosition) < 0.1f)
                        { StartCoroutine("TimerAndReset"); }
                        else
                        { OnMove(newPosition); }
                    }
                    break;
                case direction.z:
                    {
                        Vector3 newPosition = originPosition + new Vector3(0, 0, length);
                        if (Vector3.Distance(transform.position, newPosition) < 0.1f)
                        { StartCoroutine("TimerAndReset"); }
                        else
                        { OnMove(newPosition); }
                    }
                    break;
            }
        }
    }
    #region private function
    private void OnMove(Vector3 newPos)
    {
        transform.position = Vector3.Lerp(transform.position, newPos, alpha * Time.deltaTime);
    }

    IEnumerator TimerAndReset()
    {
        IsDisappear = true;
        transform.position = originPosition;
        alpha = 0;
        yield return new WaitForSeconds(IntervalTime);
        IsDisappear = false;
    }

    #endregion
}
