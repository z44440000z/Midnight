using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventTest : MonoBehaviour
{
    private int value;

    public delegate void ManipulationHandler();
    public event ManipulationHandler ChangeNum;
    protected virtual void OnNumChanged()
    {
        if (ChangeNum != null)
        {
            ChangeNum(); /* 事件被触发 */
        }
        else
        {
            Console.WriteLine("event not fire");
        }
    }


    public EventTest()
    {
        string str = null;
        SetValue(str);
    }


    public void SetValue(string str)
    {
        if (str != null && str == "触发所有事件")
        {
            OnNumChanged();
        }
    }
}

