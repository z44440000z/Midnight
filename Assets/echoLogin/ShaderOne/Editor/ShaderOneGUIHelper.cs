// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShaderOneSystem
{
	public class ShaderOneGUI
	{
		//============================================================
		public static void Line( int i_height = 1 )
		{
			Rect rect = EditorGUILayout.GetControlRect(false, i_height + 1);
			rect.height = i_height;
			EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
		}
	}
}

