using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*玩家控制鍵*/
public class CustomInput : MonoBehaviour
{
	[System.Serializable]
	public struct InputGroud
	{
		public string ButtonName;
		public KeyCode Key;
		public KeyCode GamePadKey;
	}
	public InputGroud[] Inputs;

	public static bool GamePadMode = false;
	private static  InputGroud[] s_Inputs;

	void Awake ()
	{
		s_Inputs = Inputs;
	}
	void Update()
	{
		if (Input.GetJoystickNames().Length != 0)
		{
			if (Input.GetJoystickNames()[0] != "")
			{
				Debug.Log("偵測到搖桿 : " + Input.GetJoystickNames()[0]);
				GamePadMode = true;
			}
			else
			{
				Debug.Log("Fuck!No GamePad");
				GamePadMode = false;
			}
		}
		else
		{
			Debug.Log("No GamePad");
			GamePadMode = false;
		}
	}
	//取得按鍵
	public static KeyCode Getkey(string buttonName)
	{
		if (!GamePadMode)
		{
			foreach(InputGroud CheckGroud in s_Inputs)
			{
				if (CheckGroud.ButtonName == buttonName)
				{
					return CheckGroud.Key;
				}
			}
			Debug.Log("找不到對應輸入鍵 : "+buttonName);
			return KeyCode.Numlock;
		}
		else
		{
			foreach(InputGroud CheckGroud in s_Inputs)
			{
				if (CheckGroud.ButtonName == buttonName)
				{
					return CheckGroud.GamePadKey;
				}
			}
			Debug.Log("找不到對應輸入鍵(搖桿) : "+buttonName);
			return KeyCode.Numlock;
		}
	}
	//設定按鍵
	public static void Setkey(string buttonName , KeyCode modifyKey)
	{
		for(int i = 0 ; i < s_Inputs.Length ; i++)
		{
			if (s_Inputs[i].ButtonName == buttonName)
			{
				s_Inputs[i].Key = modifyKey;
				Debug.Log("修改按鍵完成 : "+s_Inputs[i].ButtonName + " - " + s_Inputs[i].Key);
				break;
			}
		}
		Debug.Log("找不到對應輸入鍵 : "+buttonName);
	}
}



