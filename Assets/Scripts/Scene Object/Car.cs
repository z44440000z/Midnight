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

    void Start()
    {
        originPosition = car.transform.position;
    }

    void Update()
    {
        if (!IsDisappear)
        {
            if (Vector3.Distance(car.transform.position, endPosition.position) < 0.1f)
            { StartCoroutine("TimerAndReset"); }
            else
            { OnMove(endPosition.position); }

        }
    }

    #region private function
    private void OnMove(Vector3 newPos)
    {
        car.transform.position = Vector3.MoveTowards(car.transform.position, newPos, speed * Time.deltaTime);
    }
    IEnumerator TimerAndReset()
    {
        IsDisappear = true;
        car.transform.position = originPosition;
        yield return new WaitForSeconds(IntervalTime);
        IsDisappear = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(endPosition.position, Vector3.one * 5);
    }
    #endregion
}
