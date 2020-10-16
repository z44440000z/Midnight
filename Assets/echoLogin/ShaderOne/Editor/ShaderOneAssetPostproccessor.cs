using UnityEngine;
using UnityEditor;
using ShaderOneSystem;

public class ShaderOneAssetPostproccessor :  AssetPostprocessor
{
	public static bool dontProcess = false;

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		bool importedDone 	= false;
		bool genFlag 		= false;
		bool reimport 		= false;
		ShaderOneVersion so_version;
		ShaderOneVersion compiled_version;
		int loop;

		for ( loop = 0; loop < importedAssets.Length; loop++ )
		{
			if ( importedAssets[loop] != null )
			{
				if ( importedAssets[loop].IndexOf("ShaderOneVersion.json") > 0 )
					importedDone = true;
			}
		}

		if ( importedDone )
		{
			//string settings = ShaderOneIO.FindAsset ("ShaderOneSettings.json");
			string func 	= ShaderOneIO.FindAsset ("ShaderOneFunc.cginc");

			if ( func == null )
			{
				genFlag = true;
			}
			else
			{
				so_version 			= ShaderOneIO.LoadVersionSOV();
				compiled_version 	= ShaderOneIO.LoadVersionSOV(true);

				if ( compiled_version.version != so_version.version )
				{
					reimport 	= true;
					genFlag 	= true;
				}
			}

			if ( genFlag )
			{
				switch (EditorUtility.DisplayDialogComplex("ShaderOne", "Shader needs to be Generated", "Generate", "Cancel", "Open ShaderOne Generator" ) )
				{
				default:
					break;

				case 0:
					ShaderOneTools.GenerateShader ( reimport );
					Debug.Log("** ShaderOne Generated **" );
					break;

				case 2:
					ShaderOneGenerator.Init();
					break;
				}
			}
		}
	}
}


