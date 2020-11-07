using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 0.1f;
    public float IntervalTime = 2;

public GameObject car;
    public Vector3 originPosition;
    public Transform endPosition;
    private bool IsDisappear = false;
    private float alpha = 0;

    void Start()
    {
        originPosition = car.transform.position;
    }

    void Update()
    {
        if (!IsDisappear)
        {
            alpha += speed;
            if (Vector3.Distance(car.transform.position, endPosition.position) < 0.1f)
            { StartCoroutine("TimerAndReset"); }
            else
            { OnMove(endPosition.position); }

        }
    }
    
    #region private function
    private void OnMove(Vector3 newPos)
    {
        car.transform.position = Vector3.Lerp(car.transform.position, newPos, alpha * Time.deltaTime);
    }
    IEnumerator TimerAndReset()
    {
        IsDisappear = true;
        car.transform.position = originPosition;
        alpha = 0;
        yield return new WaitForSeconds(IntervalTime);
        IsDisappear = false;
    }
    #endregion
}
