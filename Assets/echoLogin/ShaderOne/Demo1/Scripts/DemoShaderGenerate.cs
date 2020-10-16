#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using ShaderOneSystem;

//[ExecuteInEditMode]
public class DemoShaderGenerate : MonoBehaviour
{
	static ShaderOneSettings sceneSettings;
	static ShaderOneSettings currentSettings;
	public TextAsset demoSettings;

	void Awake()
    {
		sceneSettings = JsonUtility.FromJson<ShaderOneSettings>(demoSettings.text);

		ShaderOneIO.LoadSettings(ref currentSettings);

		if ( !Object.Equals ( currentSettings ,sceneSettings ) )
		{

			EditorUtility.DisplayDialog("ShaderOne", "Shader needs to be generated for this demo", "Generate" );
			{
				ShaderOneIO.SaveSettings( ref sceneSettings );
				ShaderOneParse.ShaderOneMake ( sceneSettings );
				AssetDatabase.Refresh();

				string path = ShaderOneIO.FindAsset ("ShaderOneGen.shader");
				if ( path != null )
					AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );

				ShaderOneTools.ReimportMaterials();

				UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

				ShaderOneManager.ResetManager();
			}
		}
    }
}
#endif
