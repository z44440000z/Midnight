// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using ShaderOneSystem;

[CustomEditor(typeof(ShaderOneManager))]
public class ShaderOneManagerGUI : Editor
{
	private ShaderOneSettings 	_settings;

	SerializedProperty sp_fogColor;
	SerializedProperty sp_fogColorFade;
	SerializedProperty sp_fogAmbient;
	SerializedProperty sp_fogScale;
	SerializedProperty sp_fogVerticalRoughness;
	SerializedProperty sp_fogDensity;
	SerializedProperty sp_fogStartDistance;
	SerializedProperty sp_fogEndDistance;
	SerializedProperty sp_fogHeight;
	SerializedProperty sp_fogHeightSize;
	SerializedProperty sp_fogMoveX;
	SerializedProperty sp_fogMoveY;
	SerializedProperty sp_fogMoveZ;
	SerializedProperty sp_fogResourceName;
	SerializedProperty sp_disableUnityLights;
	SerializedProperty sp_bendPivot;
	SerializedProperty sp_bendAmountX;
	SerializedProperty sp_bendAmountZ;

	public bool fogFold 	= true;
	public bool terrainFold = true;
	public bool splat1Fold 	= true;
	public bool splat2Fold 	= true;
	public bool splat3Fold 	= true;
	public bool splat4Fold 	= true;

	//============================================================
	static void EditorUpdateLights()
	{
		ShaderOneManager.EditorUpdateLights();
	}

	//============================================================
	static void OnHierarchyChange()
	{
		ShaderOneManager.EditorUpdateLights();
	}

	//============================================================
	public void OnEnable()
	{
		sp_fogColor 				= serializedObject.FindProperty( "fogColor" );
		sp_fogColorFade 			= serializedObject.FindProperty( "fogColorFade" );
		sp_fogAmbient 				= serializedObject.FindProperty( "fogAmbient" );
		sp_fogScale 				= serializedObject.FindProperty( "fogScale" );
		sp_fogVerticalRoughness 	= serializedObject.FindProperty( "fogVerticalRoughness" );
		sp_fogDensity				= serializedObject.FindProperty( "fogDensity" );
		sp_fogStartDistance 		= serializedObject.FindProperty( "fogStartDistance" );
		sp_fogEndDistance 			= serializedObject.FindProperty( "fogEndDistance" );
		sp_fogHeight 				= serializedObject.FindProperty( "fogHeight" );
		sp_fogHeightSize 			= serializedObject.FindProperty( "fogHeightSize" );
		sp_fogMoveX 				= serializedObject.FindProperty( "fogMoveX" );
		sp_fogMoveY 				= serializedObject.FindProperty( "fogMoveY" );
		sp_fogMoveZ 				= serializedObject.FindProperty( "fogMoveZ");
		sp_fogResourceName 			= serializedObject.FindProperty( "fogTextureResourceName");
		sp_disableUnityLights 		= serializedObject.FindProperty( "disableUnityLights");
		sp_bendPivot 				= serializedObject.FindProperty( "bendPivot");
		sp_bendAmountX 				= serializedObject.FindProperty( "bendAmountX");
		sp_bendAmountZ 				= serializedObject.FindProperty( "bendAmountZ");
	}

	//============================================================
	public void OnDisable()
	{
		//EditorApplication.hierarchyWindowChanged -= EditorUpdateLights;
		//Debug.Log("ShaderManger disable");
	}

	//-------------------------------------------------------------
	private bool GetTexture2DReadWrite ( Texture2D i_tex )
	{
		string path = AssetDatabase.GetAssetPath( i_tex );
		TextureImporter A = (TextureImporter) AssetImporter.GetAtPath( path );
		return( A.isReadable );
	}

	//-------------------------------------------------------------
	private void SetTexture2DReadWrite ( Texture2D i_tex )
	{
		string path = AssetDatabase.GetAssetPath( i_tex );
		TextureImporter A = (TextureImporter) AssetImporter.GetAtPath( path );
		A.isReadable = true;
		AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
 	}

	//============================================================
	public void GuiSelectTexture ( SerializedProperty i_spTex, string i_msg, string i_toolTip )
	{
		EditorGUILayout.BeginHorizontal();
		i_spTex.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("", i_toolTip), i_spTex.objectReferenceValue, typeof(Texture2D), false, GUILayout.Width ( 48 ) );


		if ( i_spTex.objectReferenceValue != null )
		{
			if ( !GetTexture2DReadWrite((Texture2D)i_spTex.objectReferenceValue) )
			{
				if ( EditorUtility.DisplayDialog("**WARNING**\nThis Texture needs to have Read/Write enabled", "WOuld you like me to set this for You ?", "Yes", "No" ) )
				{
					SetTexture2DReadWrite((Texture2D)i_spTex.objectReferenceValue);
				}
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( i_msg );
		EditorGUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI()
	{
		string toolTip = "";

		if ( ShaderOneIO.Exists() )
		{
			ShaderOneIO.LoadSettings( ref _settings );
		}
		else
		{
			EditorGUILayout.LabelField ("**************************************");
			EditorGUILayout.LabelField ("Global Settings not found");
			EditorGUILayout.LabelField ("Rebuild ShaderOne");
			EditorGUILayout.LabelField ("Unity/Window/ShaderOne Generator");
			EditorGUILayout.LabelField ("**************************************");

			if ( GUILayout.Button( "ShaderOne Generator" ) )
			{
				ShaderOneGenerator.Init();
			}

			EditorGUILayout.LabelField (" ");

			return;
		}

		if ( _settings.renderPipeline == RENDER_PIPELINE.SHADERONE_LIGHTING )
		{
			EditorGUILayout.LabelField ("Settings");
			ShaderOneGUI.Line();
			sp_disableUnityLights.boolValue = EditorGUILayout.Toggle ("Disable Unity Lights", sp_disableUnityLights.boolValue );
			EditorGUILayout.LabelField (" ");
		}

		if ( _settings.bendType != BEND_TYPE.OFF)
		{
			EditorGUILayout.LabelField ("Warp Settings");
			ShaderOneGUI.Line();

			sp_bendAmountX.floatValue = EditorGUILayout.Slider(new GUIContent("Warp X Amount", toolTip),  sp_bendAmountX.floatValue, -0.1f, 0.1f );
			sp_bendAmountZ.floatValue = EditorGUILayout.Slider(new GUIContent("Warp Z Amount", toolTip),  sp_bendAmountZ.floatValue, -0.1f, 0.1f );

			sp_bendPivot.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Warp Pivot", toolTip), sp_bendPivot.objectReferenceValue, typeof(Transform), true);
		}

		EditorGUILayout.Separator();

		if ( _settings.fogMode != FOG_MODE.OFF )
		{
			EditorGUILayout.LabelField ("FOG Settings");
			ShaderOneGUI.Line();

			sp_fogColorFade.colorValue = EditorGUILayout.ColorField ( "Fade Color", sp_fogColorFade.colorValue );

			toolTip = "The distance from the camera, where the fog solid color starts";
			sp_fogStartDistance.floatValue = EditorGUILayout.FloatField(new GUIContent("Start Fade Distance", toolTip), sp_fogStartDistance.floatValue );

			toolTip = "The distance from the camera, where the fog solid color end";
			sp_fogEndDistance.floatValue = EditorGUILayout.FloatField(new GUIContent("End Fade Distance", toolTip), sp_fogEndDistance.floatValue );

			EditorGUILayout.LabelField (" ");

			if ( _settings.fogMode == FOG_MODE.VOLUMETRIC_3D || _settings.fogMode == FOG_MODE.VOLUMETRIC )
			{
				EditorGUILayout.LabelField ("FOG Volumetric Settings");
				ShaderOneGUI.Line();

				sp_fogColor.colorValue = EditorGUILayout.ColorField ( "Tint", sp_fogColor.colorValue );

				toolTip = "Ambient color value of fog";
				sp_fogAmbient.floatValue = EditorGUILayout.Slider(new GUIContent("Ambient", toolTip), sp_fogAmbient.floatValue, 0.0f, 1.0f);

				toolTip = "Fog Density";
				sp_fogDensity.floatValue = EditorGUILayout.Slider(new GUIContent("Density", toolTip), sp_fogDensity.floatValue, 0.0f, 1.0f );

				EditorGUILayout.Separator();

				toolTip = "Fog Y / Height position";
	   			sp_fogHeight.floatValue = EditorGUILayout.FloatField(new GUIContent("Vertical Position (Y)", toolTip), sp_fogHeight.floatValue );

	   			toolTip = "How far fog spreads out from the Fog Position up and down";
	   			sp_fogHeightSize.floatValue = EditorGUILayout.FloatField(new GUIContent("Vertical Size", toolTip), sp_fogHeightSize.floatValue );

				if ( _settings.fogMode == FOG_MODE.VOLUMETRIC_3D )
				{
					toolTip = "Scale of Fog";
					sp_fogScale.floatValue = EditorGUILayout.Slider(new GUIContent("Scale", toolTip), sp_fogScale.floatValue, 0.1f, 0.0025f);

					toolTip = "Exagerate the vertical edges of Fog";
					sp_fogVerticalRoughness.floatValue = EditorGUILayout.Slider(new GUIContent("Vertical Roughness", toolTip), sp_fogVerticalRoughness.floatValue, 0.0f, 16.0f);

					EditorGUILayout.Separator();
					toolTip = "How fast the dust moves on the X axis";
					sp_fogMoveX.floatValue = EditorGUILayout.Slider( new GUIContent("Movement X", toolTip), sp_fogMoveX.floatValue, -0.5f, 0.5f);

					toolTip = "How fast the dust moves on the Y axis";
					sp_fogMoveY.floatValue = EditorGUILayout.Slider( new GUIContent("Movement Y", toolTip), sp_fogMoveY.floatValue, -0.5f, 0.5f);

					toolTip = "How fast the dust moves on the Z axis";
					sp_fogMoveZ.floatValue = EditorGUILayout.Slider( new GUIContent("Movement Z", toolTip), sp_fogMoveZ.floatValue, -0.5f, 0.5f);
				}

				EditorGUILayout.Separator();

				ShaderOneGUI.Line();

				if ( GUILayout.Button( "Load Custom Fog", GUILayout.Width ( 128 ) ) )
				{
					string path = EditorUtility.OpenFilePanel("Load Custom Fog","/echoLogin/ShaderOne/Resources", "sofog" );

					path = Path.GetFileName ( path );
					path = path.Replace (".bytes", "");

					sp_fogResourceName.stringValue = path;

					ShaderOneManager.ResetManager();
				}

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField ("3D Fog file:", GUILayout.Width ( 64 ));

				if ( sp_fogResourceName.stringValue.Length < 1 )
					EditorGUILayout.LabelField ("ShaderOneDefaultFog" );
				else
					EditorGUILayout.LabelField ( sp_fogResourceName.stringValue, EditorStyles.boldLabel );

				EditorGUILayout.EndHorizontal();

			}
			else
			{
				Shader.DisableKeyword ("SO_SF_FOGMAP_ON");
			}

		}

		serializedObject.ApplyModifiedProperties();

//		EditorGUILayout.LabelField ("Terrain");
	}

}

