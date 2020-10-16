// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using ShaderOneSystem;

public class ShaderOneParse
{
    static string templateDir 		= null;
    static string shaderDir 		= null;
    static string shaderMainName	= "ShaderOneMain.cginc";
	static string shaderMetaName	= "ShaderOnePassMeta.cginc";
	static string shaderShadowCasterName	= "ShaderOnePassShadowCaster.cginc";
	static string strMain;
	static string strMeta;

	//-------------------------------------------------------------------------
	// 0 = normal, 1 = surface var 2 = invert surface var
	static private int IsSurfaceVar ( string i_name )
	{
		int rc = 0;

		switch ( i_name )
		{
			case "METALLIC":
				rc = 1;
				break;
			case "ROUGHNESS":
				rc = 1;
				break;
			case "SMOOTHNESS":
				rc = 2;
				break;
		}

		return ( rc );
	}

	//-------------------------------------------------------------------------
	static private string GetBit ( int i_num, int i_pos )
	{
		bool        bit;

		bit = ( i_num & ( 1 << i_pos ) ) != 0;

		if (!bit)
			return("!");

		return ( "" );
	}

	//-------------------------------------------------------------------------
	static private string MakeBitDefines ( String i_name, Type i_enum, int i_numBits )
	{
		string 		finalOutput 	= "";
		string 		prefix 		= "SO_SF_" + i_name + "_BIT";
		int 		loop, b;
		string 		line = "";
		string	[] 	names = Enum.GetNames ( i_enum );

		for ( loop = 0; loop < names.Length; loop++ )
		{
			line = "#if ";

			for ( b = i_numBits-1; b >= 0; b-- )
			{
				line += GetBit ( loop, b ) + "defined(" + prefix + ( b + 1 ) +")";

				if ( b > 0 )
					line+= " && ";
			}

			finalOutput += line;
			finalOutput += "\n\t#define SO_" + i_name + "_" + names[loop] + "\n";
			finalOutput += "#endif\n\n";
		}

		finalOutput += "\n\n";

		return ( finalOutput );
	}

	//-------------------------------------------------------------------------
	static private int GetBlendBitCount ( Type i_enum, ShaderOneSettings i_ss )
	{
		int 		loop;
		int 		count 	= 0;
		string	[]	names	= Enum.GetNames ( i_enum );

		for ( loop = 0; loop < names.Length; loop++ )
		{
			if ( ShaderOneIO.GetBit ( i_ss.blendModeMask, loop ) )
			{
				count++;
			}
		}

		if ( count < 2 )
			return ( 0 );

		return ( ShaderOneIO.CountBitsNeeded ( count ) );
	}

	//-------------------------------------------------------------------------
	static private string MakeBlendBitDefines ( String i_name, Type i_enum, ShaderOneSettings i_ss )
	{
		// make array of names
		// make array of ID's
		// figrue out how many bits to use

		string 		finalOutput = "";
		string 		prefix 		= "SO_SF_" + i_name + "_BIT";
		string 		line 		= "";
		string 	[] 	names;
		string 	[] 	namesFinal;
		int 		loop;
		int 	[] 	ids;
		int        	index;
		int         numBits;
		int         b;

		names 		= Enum.GetNames ( i_enum );
		namesFinal 	= new string [ names.Length ];
		ids         = new int [ names.Length ];
		index       = 0;

		// get blends modes in use
		for ( loop = 0; loop < names.Length; loop++ )
		{
			if ( ShaderOneIO.GetBit ( i_ss.blendModeMask, loop ) )
			{
				namesFinal [ index ] = names [ loop ];
				ids [ index ] = loop;
				index++;
			}
		}

		numBits = ShaderOneIO.CountBitsNeeded ( index );

		//now make the defines
		for ( loop = 0; loop < index; loop++ )
		{
			if ( index > 1 )
			{
				line = "#if ";

				for ( b = numBits-1; b >= 0; b-- )
				{
					line += GetBit ( loop, b ) + "defined(" + prefix + ( b + 1 ) +")";

					if ( b > 0 )
						line+= " && ";
				}

				finalOutput += line;
			}

			finalOutput += "\n\t#define SO_" + i_name + "_" + namesFinal[loop] + "\n";

			if ( index > 1 )
				finalOutput += "#endif\n\n";
		}

		finalOutput += "\n\n";

		return ( finalOutput );

	}

	//-------------------------------------------------------------------------
	static private string SurfaceMapInlineFuncs ( Type i_enum )
	{
		string 		finalOutput 	= "";
		string [] 	channelName 	= new string[4];
		int 		channelIndex;
		int 		loop;
		int         surfaceFlag = 0;

		channelName[0] = "R";
		channelName[1] = "G";
		channelName[2] = "B";
		channelName[3] = "A";

		String[] names = Enum.GetNames ( i_enum );

		for ( loop = 1; loop < names.Length; loop++ )
		{
			surfaceFlag = IsSurfaceVar ( names[loop] );

			finalOutput += "\ninline void GetSurfaceMap_" + names[loop] + "( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )\n";

			finalOutput +="{\n";

			finalOutput += "\n#ifdef SO_SURFACE_MAP_" + names[loop] + "\n\n";

			for ( channelIndex = 0; channelIndex < 4; channelIndex++ )
			{
				if ( channelIndex == 0 )
					finalOutput += "\t#if ";
				else
					finalOutput += "\t#elif ";

				finalOutput += "defined (SO_SURFACE_MAP" + channelName[channelIndex] + "_" + names[loop] +")\n";

				if ( surfaceFlag > 0 )
					finalOutput += "\t\ti_result *= ( ";
				else
					finalOutput += "\t\ti_result = ( ";

				finalOutput += " i_surfaceMap." + channelName[channelIndex].ToLower() + " );\n";
			}

			finalOutput += "\t#endif\n\n";

			if ( surfaceFlag > 0 )
			{
				finalOutput += "#else\n";
//				finalOutput += "\n\ti_result = i_input;\n\n";
			}

			finalOutput += "#endif\n\n";

			finalOutput +="}\n";
		}

		return ( finalOutput );
	}


	//-------------------------------------------------------------------------
	static private string AlphaMapInlineFuncs ( Type i_enum )
	{
		string 		finalOutput 	= "";
		int 		loop;
		int         surfaceFlag = 0;

		String[] names = Enum.GetNames ( i_enum );

		for ( loop = 1; loop < names.Length; loop++ )
		{
			surfaceFlag = IsSurfaceVar ( names[loop] );

			finalOutput += "\ninline void GetAlphaMap_" + names[loop] + "( in SOLightingData i_sold, inout SOFLOAT i_result )\n";

			finalOutput +="{\n";

			finalOutput += "#ifdef SO_ALPHA_MAP_" + names[loop] + "\n";

			finalOutput += "\t\ti_result = ( ";

			if ( surfaceFlag > 0 )
			{
				finalOutput += " i_sold.alphaMap ) * i_result;\n";
			}
			else
			{
				finalOutput += " i_sold.alphaMap );\n";
			}

			finalOutput += "#endif\n";

			finalOutput +="}\n";
		}

		return ( finalOutput );
	}


	//-------------------------------------------------------------------------
	static private string SpecularMapInlineFuncs ( Type i_enum )
	{
		string 		finalOutput 	= "";
		int 		loop;
		int         surfaceFlag = 0;

		String[] names = Enum.GetNames ( i_enum );

		for ( loop = 1; loop < names.Length; loop++ )
		{
			surfaceFlag = IsSurfaceVar ( names[loop] );

			finalOutput += "\ninline void GetSpecularMap_" + names[loop] + "( in SOFLOAT4 i_specularMap, inout SOFLOAT i_result )\n";

			finalOutput +="{\n";

			finalOutput += "#ifdef SO_SPECULAR_MAP_" + names[loop] + "\n";

			if ( surfaceFlag > 0 )
				finalOutput += "\t\ti_result *= ( ";
			else
				finalOutput += "\t\ti_result = ( ";

			finalOutput += " i_specularMap.a );\n";

			finalOutput += "#endif\n";

			finalOutput +="}\n";
		}

		return ( finalOutput );
	}

	//-------------------------------------------------------------------------
	static private string AlphaAndSurfaceMapDefines()
	{
		string 		finalOutput 	= "";
		string [] 	channelName 	= new string[4];
		string      surfaceDefine;
		int 		channelIndex;
		int 		loop;

		channelName[0] = "R";
		channelName[1] = "G";
		channelName[2] = "B";
		channelName[3] = "A";

		String[] names = Enum.GetNames ( typeof(MAP_CHANNEL) );

		for ( loop = 0; loop < names.Length; loop++ )
		{
			finalOutput += "\n#if";

			for ( channelIndex = 0; channelIndex < 4; channelIndex++ )
			{
				finalOutput += " defined (SO_SURFACE_MAP" + channelName[channelIndex] + "_" + names[loop] +") ";

				if ( channelIndex < 3)
					finalOutput += "||";
			}

			surfaceDefine = "SO_SURFACE_MAP_" + names[loop];

			finalOutput += "\n\t#define "+ surfaceDefine + "\n";
			finalOutput += "#endif\n\n";

			finalOutput += "#if defined("+surfaceDefine+") || defined(SO_ALPHA_MAP_" + names[loop] + ") || defined(SO_SPECULAR_MAP_" + names[loop] + ")\n";
			finalOutput += "\t#define SO_MAP_" + names[loop] + "\n";
			finalOutput += "#endif\n\n\n";
		}

		return ( finalOutput );
	}


	//-------------------------------------------------------------------------
	static private string MakeDefines ( ShaderOneSettings i_ss )
	{
		string finalOutput = "";

		if ( i_ss.renderPipeline != RENDER_PIPELINE.UNLIT )
			finalOutput += MakeBitDefines ("LIGHTING", typeof(LIGHTINGMODE), 2 );

		if ( i_ss.vertexOptions )
			finalOutput += MakeBitDefines ("VERTEX", typeof(VERTEXMODE), 2 );

		finalOutput += MakeBlendBitDefines ("BLEND", typeof(BLENDMODE), i_ss );

		if ( i_ss.layer0.animOptions )
			finalOutput += MakeBitDefines ("LAYER0_ANIMTYPE", typeof(ANIMTYPE), 3 );

		if ( i_ss.layer1.enabled )
		{
			finalOutput += MakeBitDefines ("LAYER1_BLEND", typeof(LAYER_BLEND_MODE), 2 );

			if ( i_ss.layer1.animOptions )
				finalOutput += MakeBitDefines ("LAYER1_ANIMTYPE", typeof(ANIMTYPE), 3 );
		}

		if ( i_ss.layer2.enabled )
		{
			finalOutput += MakeBitDefines ("LAYER2_BLEND", typeof(LAYER_BLEND_MODE), 2 );

			if ( i_ss.layer2.animOptions )
				finalOutput += MakeBitDefines ("LAYER2_ANIMTYPE", typeof(ANIMTYPE), 3 );
		}

		if ( i_ss.layer3.enabled )
		{
			finalOutput += MakeBitDefines ("LAYER3_BLEND", typeof(LAYER_BLEND_MODE), 2 );

			if ( i_ss.layer3.animOptions )
				finalOutput += MakeBitDefines ("LAYER4_ANIMTYPE", typeof(ANIMTYPE), 3 );
		}

		if ( i_ss.layer0.surfaceMap || i_ss.layer1.surfaceMap || i_ss.layer2.surfaceMap || i_ss.layer3.surfaceMap )
		{
			if ( i_ss.surfaceMapR == MAP_CHANNEL.EMPTY )
				finalOutput += MakeBitDefines ("SURFACE_MAPR", typeof(MAP_CHANNEL), 3 );
			if ( i_ss.surfaceMapG == MAP_CHANNEL.EMPTY )
				finalOutput += MakeBitDefines ("SURFACE_MAPG", typeof(MAP_CHANNEL), 3 );
			if ( i_ss.surfaceMapB == MAP_CHANNEL.EMPTY )
				finalOutput += MakeBitDefines ("SURFACE_MAPB", typeof(MAP_CHANNEL), 3 );
			if ( i_ss.surfaceMapA == MAP_CHANNEL.EMPTY )
				finalOutput += MakeBitDefines ("SURFACE_MAPA", typeof(MAP_CHANNEL), 3 );
		}

		if (i_ss.alphaMap)
			finalOutput += MakeBitDefines ("ALPHA_MAP", typeof(MAP_CHANNEL), 3 );

		if ( i_ss.specularMap == MAP_CHANNEL.EMPTY && i_ss.workflow == WORKFLOW_MODE.SPECULAR )
			finalOutput += MakeBitDefines ("SPECULAR_MAP", typeof(MAP_CHANNEL), 3 );

		if ( i_ss.uvWorldMap )
			finalOutput += MakeBitDefines ("UV1_WORLDMAP", typeof(UV_WORLD_MAPPING), 2 );

		return ( finalOutput );
	}

	//=========================================================================
	static string MakeInstancingFile(ShaderOneSettings i_ss,  string i_templateDir )
	{
		string code = "";

		if ( i_ss.instancing )
		{
		#if UNITY_5
			code 	= File.ReadAllText ( i_templateDir + "ShaderOneColorInstanced_Unity5.txt" );
		#else
			code 	= File.ReadAllText ( i_templateDir + "ShaderOneColorInstanced.txt" );
		#endif
		}
		else
		{
			code 	= File.ReadAllText ( i_templateDir + "ShaderOneColor.txt" );
		}

		return ( code );
	}

	//=========================================================================
	static string MakeLightMacrosFile( ShaderOneSettings i_ss,  string i_templateDir )
	{
		string code = "";

		#if UNITY_2018
			code 	= File.ReadAllText ( i_templateDir + "ShaderOne_UnityLightMacros2018.txt" );
		#else
			code 	= File.ReadAllText ( i_templateDir + "ShaderOne_UnityLightMacros2017.txt" );
		#endif

		return ( code );
	}

	//============================================================
	public static string FindCurvedWorldPath ()
	{
		string[] assetPaths = AssetDatabase.GetAllAssetPaths();
		string curvedWorldPath = "";
		int loop;

		for ( loop = 0; loop < assetPaths.Length; loop++ )
		{
			if ( assetPaths[loop].IndexOf("CurvedWorld_Base.cginc") > 0 )
			{
				curvedWorldPath = assetPaths[loop];
				break;
			}
		}

		return ( curvedWorldPath );
	}


	//=========================================================================
	static string MakePass ( bool i_basePass, ShaderOneSettings i_ss )
	{
		string finalOutput = "";

		finalOutput += "Pass\n";
		finalOutput += "{\n";

		if ( i_basePass )
			finalOutput += "Tags { \"LightMode\" = \"ForwardBase\" }\n";
		else
			finalOutput += "Tags { \"LightMode\" = \"ForwardAdd\" }\n";

		if ( i_basePass )
		{
			finalOutput += "Blend [_SrcBlend] [_DstBlend]\n";
		}
		else
		{
			finalOutput += "Blend [_SrcBlendAddPass] [_DstBlendAddPass]\n";
		}

		finalOutput += "Cull [_MyCullMode]\n";
		finalOutput += "CGPROGRAM\n";
		finalOutput += "#pragma target 3.0\n";
		finalOutput += "#pragma vertex vert\n";
		finalOutput += "#pragma fragment frag\n";
		finalOutput += "#pragma fragmentoption ARB_precision_hint_fastest\n";

		finalOutput += MakePassOptions ( i_ss, i_basePass );

		finalOutput += "#pragma multi_compile __ SO_MC_CONTROL_SCRIPT_ON\n";
		finalOutput += "#pragma shader_feature SO_SF_MANUAL_CONTROL\n";
		finalOutput += "#pragma shader_feature SO_SF_NORMAL_FIX\n";
		finalOutput += "#pragma shader_feature SO_SF_MANAGER_ON\n";

		int bitCount = GetBlendBitCount ( typeof(BLENDMODE), i_ss);

		for (int loop = 0; loop < bitCount; loop++ )
		{
			finalOutput += "#pragma shader_feature SO_SF_BLEND_BIT" +(loop+1) + "\n";
		}

		finalOutput += "#include \"UnityCG.cginc\"\n";
		finalOutput += "#include \"AutoLight.cginc\"\n";

		if ( i_ss.bendType == BEND_TYPE.CURVED_WORLD )
		{
			finalOutput += "#include \"" + FindCurvedWorldPath() + "\"\n";
		}

		finalOutput += "#include \"ShaderOneGen_BitDecode.cginc\"\n";
		finalOutput += "#include \"ShaderOneDefine.cginc\"\n";
		finalOutput += "#include \"ShaderOneVars.cginc\"\n";
		finalOutput += "#include \"ShaderOneGen_FuncMaps.cginc\"\n";
		finalOutput += "#include \"ShaderOneUnity.cginc\"\n";
		finalOutput += "#include \"ShaderOneGen_UnityLightMacros.cginc\"\n";
		finalOutput += "#include \"ShaderOneFunc.cginc\"\n";
		finalOutput += "#include \"ShaderOneFuncLayersBlend.cginc\"\n";
		finalOutput += "#include \"ShaderOneFuncLayer.cginc\"\n";
		finalOutput += "#include \"ShaderOneFuncLighting.cginc\"\n";
		finalOutput += "#include \"ShaderOneGen_Instancing.cginc\"\n";
		finalOutput += "#include \"ShaderOneVert.cginc\"\n";
		finalOutput += "#include \"ShaderOneFrag.cginc\"\n";

		finalOutput += "ENDCG\n";
		finalOutput += "}\n";


		finalOutput += "\n";

		return ( finalOutput );
	}

	//=========================================================================
	static string Bits2ShaderFeature ( string i_name, int i_numBits )
	{
		string finalOutput = "";
		int loop;

		finalOutput += "\n";

		for ( loop = 1; loop <= i_numBits; loop++ )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_" + i_name + "_BIT" + loop + "\n";
		}

		finalOutput += "\n";

		return(finalOutput);
	}

	//=========================================================================
	static string SurfaceChannelSet ( string i_chan, MAP_CHANNEL i_mapChan )
	{
		string 	finalOutput = "";

		String[] names = Enum.GetNames ( typeof(MAP_CHANNEL) );

		finalOutput += "\n\t#define SO_SURFACE_MAP" + i_chan + "_" + names[(int)i_mapChan] + "\n\n";

		return(finalOutput);
	}

	//=========================================================================
	static public string MakeLayerOptions ( ShaderOneSettings i_ss, ShaderOneLayerSettings i_layer, int i_layerNum )
	{
		string 		finalOutput = "";
		string      layerName;

		layerName ="SO_SF_LAYER"+i_layerNum;

		if ( i_layer.enabled )
		{
			if ( i_layerNum > 0 )
			{
				finalOutput += "\t#pragma shader_feature " + layerName + "_ON\n";

				switch ( i_layer.uv2 )
				{
				case UV2_OPTION.OFF:
					break;

				case UV2_OPTION.MATERIAL_SELECTABLE:
					finalOutput += "\t#pragma shader_feature " + layerName + "_UV2_ON\n";
					break;

				case UV2_OPTION.ALWAYS_UV2:
					finalOutput += "\t#define " + layerName + "_UV2_ON\n";
					break;
				}
			}

			if ( i_layer.scriptToggle && i_layerNum > 0 )
			{
				finalOutput += "\t#pragma multi_compile __ " + "SO_MC_LAYER" + i_layerNum + "_ON\n";
			}

			if ( i_layer.normalMap )
				finalOutput += "\t#pragma shader_feature " + layerName + "_BUMP_ON\n";

			if ( i_layer.surfaceMap )
			{
				finalOutput += "\t#pragma shader_feature " + layerName + "_SURFACE_MAP_ON\n";
			}

			if ( i_ss.workflow == WORKFLOW_MODE.SPECULAR )
			{
				finalOutput += "\t#pragma shader_feature " + layerName + "_SPECULAR_MAP_ON\n";
			}

			if ( i_layer.flowMap )
				finalOutput += "\t#pragma shader_feature " + layerName + "_FLOWMAP_ON\n";

			if ( i_layer.animOptions )
			{
				finalOutput += Bits2ShaderFeature ( "LAYER" + i_layerNum + "_ANIMTYPE", 3 );

//				finalOutput += "\t#pragma shader_feature _ " + layerName + "_CELLANIMBLEND_ON " + layerName + "_CELLANIM_ON " + layerName + "_PROGRESS_ON " + layerName + "_RANDOMUV_ON \n";
			}

			if ( i_layer.scrollUV )
				finalOutput += "\t#pragma shader_feature " + layerName + "_SCROLLUV_ON\n";

			if ( i_layer.rotateUV )
				finalOutput += "\t#pragma shader_feature " + layerName + "_ROTATEUV_ON\n";

			if ( i_layer.blendModes )
				finalOutput += Bits2ShaderFeature ( "LAYER" + i_layerNum + "_BLEND", 2 );

			finalOutput += "\n";
		}

		return ( finalOutput );
	}


	//=========================================================================
	static public string MakeUnityMultiCompile(  ShaderOneSettings i_ss, bool i_unityForward = false )
	{
		string 		finalOutput = "";

		///SCOTTFIND will eventually have to make a FORWARD BASE and ADD
		///thing for this
		if ( i_ss.bakedLightMapping )
		{
			finalOutput += "\t#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON\n";
		}

		if ( i_ss.dynamicLightMapping )
		{
			finalOutput += "\t#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON\n";
		}

		if ( i_ss.shadows )
		{
			finalOutput += "\t#pragma multi_compile __ SHADOWS_SCREEN SHADOWS_CUBE\n";
			finalOutput += "\t#pragma multi_compile __ SHADOWS_SHADOWMASK\n";
			finalOutput += "\t#pragma multi_compile __ LIGHTMAP_SHADOW_MIXING\n";
		}

		if ( i_ss.lightProbes )
		{
			finalOutput += "\t#pragma multi_compile __ LIGHTPROBE_SH\n";
			finalOutput += "\t#define SO_GD_LIGHTPROBES_ON\n";
		}

		if ( i_unityForward )
		{
			finalOutput += "\t#pragma multi_compile DIRECTIONAL DIRECTIONAL_COOKIE POINT POINT_COOKIE SPOT SPOT_COOKIE\n";
			finalOutput += "\t#pragma multi_compile __ VERTEXLIGHT_ON\n";
		}

		return (finalOutput);
	}

	//=========================================================================
	static public string MakePassOptions ( ShaderOneSettings i_ss, bool i_basePass )
	{
		string 		finalOutput = "";

		// WE write this out so the array size dont change when going back and forth to ShaderOne Lighting
		// DIR
		finalOutput += "\t#define SO_GD_DIR_COUNT " + Mathf.Max ( i_ss.dirArraySize, 1 ) + "\n";

		finalOutput += "\t#define SO_GD_V_DIR_START " + i_ss.dirPerPixelCount + "\n";
		finalOutput += "\t#define SO_GD_V_DIR_END " + ( i_ss.dirArraySize - 1 ) + "\n";

		finalOutput += "\t#define SO_GD_P_DIR_START 0 \n";
		finalOutput += "\t#define SO_GD_P_DIR_END " + ( i_ss.dirPerPixelCount - 1 ) + "\n";

		// POINT
		finalOutput += "\t#define SO_GD_POINT_COUNT " +  Mathf.Max ( i_ss.pointArraySize, 1 ) + "\n";

		finalOutput += "\t#define SO_GD_V_POINT_START " + i_ss.pointPerPixelCount + "\n";
		finalOutput += "\t#define SO_GD_V_POINT_END " + ( i_ss.pointArraySize - 1 ) + "\n";

		finalOutput += "\t#define SO_GD_P_POINT_START 0 \n";
		finalOutput += "\t#define SO_GD_P_POINT_END " + ( i_ss.pointPerPixelCount - 1 ) + "\n";

		// SPOT
		finalOutput += "\t#define SO_GD_SPOT_COUNT " + Mathf.Max ( i_ss.spotArraySize, 1 ) + "\n";

		finalOutput += "\t#define SO_GD_V_SPOT_START " + i_ss.spotPerPixelCount + "\n";
		finalOutput += "\t#define SO_GD_V_SPOT_END " + ( i_ss.spotArraySize - 1 ) + "\n";

		finalOutput += "\t#define SO_GD_P_SPOT_START 0 \n";
		finalOutput += "\t#define SO_GD_P_SPOT_END " + (i_ss.spotPerPixelCount - 1 ) + "\n";
		finalOutput += "\n";


		if ( i_ss.colorAmplify )
		{
			finalOutput += "\t#define SO_GD_AMPLIFY_COLORS\n";
		}

		if ( i_ss.directXNormal )
		{
			finalOutput += "\t#define SO_GD_NORMAL_DIRECTX\n";
		}

		switch ( i_ss.bumpScale )
		{
		case SCALE_TYPE.NONE:
			finalOutput += "\t#define SO_GD_BUMP_SCALE_NONE\n";
			break;
		case SCALE_TYPE.GLOBAL:
			finalOutput += "\t#define SO_GD_BUMP_SCALE_GLOBAL\n";
			break;
		case SCALE_TYPE.PER_LAYER:
			finalOutput += "\t#define SO_GD_BUMP_SCALE_PER_LAYER\n";
			break;
		}

		switch ( i_ss.aoScale )
		{
		case SCALE_TYPE.NONE:
			finalOutput += "\t#define SO_GD_AO_SCALE_NONE\n";
			break;
		case SCALE_TYPE.GLOBAL:
			finalOutput += "\t#define SO_GD_AO_SCALE_GLOBAL\n";
			break;
		case SCALE_TYPE.PER_LAYER:
			finalOutput += "\t#define SO_GD_AO_SCALE_PER_LAYER\n";
			break;
		}

		if ( i_ss.uvWorldMap )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_UV1_WORLDMAP_BIT1\n";
			finalOutput += "\t#pragma shader_feature SO_SF_UV1_WORLDMAP_BIT2\n";
		}
		else
		{
			finalOutput += "\t#define SO_UV1_WORLDMAP_MESH_UV\n";
		}


		if ( i_ss.renderPipeline == RENDER_PIPELINE.UNITY_FORWARD )
		{
			finalOutput += "\t#define SO_GD_PIPELINE_UNITY_FORWARD 1 \n";

			if ( i_basePass )
			{
				finalOutput += MakeUnityMultiCompile(i_ss, true);
				//finalOutput += "\t#pragma multi_compile_fwdbase\n";
				finalOutput += "\t#define SO_GD_BASE_PASS 1\n";
			}
			else
			{
				finalOutput += "\t#pragma multi_compile_lightpass\n"; /// SCOTTFIND try to make it so we dont need this
				finalOutput += "\t#define SO_GD_ADD_PASS\n";
			}

			finalOutput += "\t#define SO_GD_VERTEX_LIGHTING_ON \n";
			finalOutput += "\t#define SO_GD_FRAG_LIGHTING_ON \n";

			finalOutput += "\n";
		}
		else if ( i_ss.renderPipeline == RENDER_PIPELINE.SHADERONE_LIGHTING )
		{
			finalOutput += "\t#define SO_GD_PIPELINE_SHADER_ONE 1 \n";
			finalOutput += "\t#define SO_GD_BASE_PASS 1\n";

			finalOutput += MakeUnityMultiCompile ( i_ss );

			if ( i_ss.dirArraySize > 0 )
			{
				if ( i_ss.dirPerPixelCount < i_ss.dirArraySize )
					finalOutput += "\t#pragma multi_compile __ SO_MC_V_DIRECTIONAL_ON\n";

				if ( i_ss.dirPerPixelCount > 0 )
					finalOutput += "\t#pragma multi_compile __ SO_MC_P_DIRECTIONAL_ON\n";
			}

			if ( i_ss.pointArraySize > 0 )
			{
				if ( i_ss.pointPerPixelCount < i_ss.pointArraySize )
					finalOutput += "\t#pragma multi_compile __ SO_MC_V_POINT_ON\n";

				if ( i_ss.pointPerPixelCount > 0 )
					finalOutput += "\t#pragma multi_compile __ SO_MC_P_POINT_ON\n";
			}

			if ( i_ss.spotArraySize > 0 )
			{
				if ( i_ss.spotPerPixelCount < i_ss.spotArraySize )
					finalOutput += "\t#pragma multi_compile __ SO_MC_V_SPOT_ON\n";

				if ( i_ss.spotPerPixelCount > 0 )
					finalOutput += "\t#pragma multi_compile __ SO_MC_P_SPOT_ON\n";
			}

			finalOutput += "\n";

			bool vertexLighting = false;

			if ( i_ss.dirPerPixelCount < i_ss.dirArraySize )
				vertexLighting = true;

			if ( i_ss.pointPerPixelCount < i_ss.pointArraySize )
				vertexLighting = true;

			if ( i_ss.spotPerPixelCount < i_ss.spotArraySize )
				vertexLighting = true;

			if ( vertexLighting )
			{
				finalOutput += "\t#define SO_GD_VERTEX_LIGHTING_ON \n";
			}

			if ( i_ss.dirPerPixelCount > 0 || i_ss.pointPerPixelCount > 0 || i_ss.spotPerPixelCount > 0)
			{
				finalOutput += "\t#define SO_GD_FRAG_LIGHTING_ON \n";
			}
    	}
		else
		{
			finalOutput += "\t#define SO_GD_UNLIT\n";
			finalOutput += "\t#define SO_GD_BASE_PASS 1\n";
		}

		if ( i_ss.workflow == WORKFLOW_MODE.SMOOTHNESS )
			finalOutput += "\t#define SO_GD_WORKFLOW_SMOOTHNESS\n";
		else if ( i_ss.workflow == WORKFLOW_MODE.ROUGHNESS )
			  finalOutput += "\t#define SO_GD_WORKFLOW_ROUGHNESS\n";
		else
			  finalOutput += "\t#define SO_GD_WORKFLOW_SPECULAR\n";

		if ( i_ss.renderPipeline != RENDER_PIPELINE.UNLIT )
		{
			if (i_ss.pbr )
			{
				finalOutput += "\t#pragma shader_feature SO_SF_PBR_ON\n";
			}

			if ( i_ss.fresnel )
			{
				finalOutput += "\t#define SO_GD_FRESNEL_ON\n";

				switch ( i_ss.fresnelPow )
				{
				case FRESNEL_POW.FAST:
					finalOutput += "\t#define SO_GD_FRESNEL_POW3\n";
					break;
				case FRESNEL_POW.NORMAL:
					finalOutput += "\t#define SO_GD_FRESNEL_POW4\n";
					break;
				case FRESNEL_POW.HQ:
					finalOutput += "\t#define SO_GD_FRESNEL_POW5\n";
					break;
				}
			}

			switch (i_ss.specularMode)
			{
			case  SPECULAR_MODE.OFF:
				finalOutput += "\t#define SO_GD_SPECULAR_OFF";
				break;
			case  SPECULAR_MODE.FASTEST:
				finalOutput += "\t#define SO_GD_SPECULAR_FASTEST\n";
				finalOutput += "\t#pragma shader_feature SO_SF_SPECULAR_ON\n";
				break;
			case  SPECULAR_MODE.FAST:
				finalOutput += "\t#define SO_GD_SPECULAR_FAST\n";
				finalOutput += "\t#pragma shader_feature SO_SF_SPECULAR_ON\n";
				break;
			case  SPECULAR_MODE.NORMAL:
				finalOutput += "\t#define SO_GD_SPECULAR_NORMAL\n";
				finalOutput += "\t#pragma shader_feature SO_SF_SPECULAR_ON\n";
				break;
			case  SPECULAR_MODE.HQ:
				finalOutput += "\t#define SO_GD_SPECULAR_HQ\n";
				finalOutput += "\t#pragma shader_feature SO_SF_SPECULAR_ON\n";
				break;
			}

			switch (i_ss.specularBlend)
			{
			case  SPECULAR_BLEND.MONOCHROMATIC:
				finalOutput += "\t#define SO_GD_SPECULAR_BLEND_MONOCHROMATIC\n";
				break;

			default:
				finalOutput += "\t#define SO_GD_SPECULAR_BLEND_COLOR\n";
				break;
			}
    	}

		if ( i_ss.renderPipeline != RENDER_PIPELINE.UNLIT )
			finalOutput += Bits2ShaderFeature ("LIGHTING", 2 );

		if ( i_ss.layer0.surfaceMap || i_ss.layer1.surfaceMap || i_ss.layer2.surfaceMap || i_ss.layer3.surfaceMap )
		{
			if ( i_ss.surfaceMapR == MAP_CHANNEL.EMPTY )
				finalOutput += Bits2ShaderFeature ("SURFACE_MAPR", 3 );
			else
				finalOutput += SurfaceChannelSet( "R", i_ss.surfaceMapR );

			if ( i_ss.surfaceMapG == MAP_CHANNEL.EMPTY )
				finalOutput += Bits2ShaderFeature ("SURFACE_MAPG", 3 );
			else
				finalOutput += SurfaceChannelSet( "G", i_ss.surfaceMapG );

			if ( i_ss.surfaceMapB == MAP_CHANNEL.EMPTY )
				finalOutput += Bits2ShaderFeature ("SURFACE_MAPB", 3 );
			else
				finalOutput += SurfaceChannelSet( "B", i_ss.surfaceMapB );

			if ( i_ss.surfaceMapA == MAP_CHANNEL.EMPTY )
				finalOutput += Bits2ShaderFeature ("SURFACE_MAPA", 3 );
			else
				finalOutput += SurfaceChannelSet( "A", i_ss.surfaceMapA );
		}

		if ( i_ss.alphaMap )
			finalOutput += Bits2ShaderFeature ("ALPHA_MAP", 3 );

		if ( i_ss.workflow == WORKFLOW_MODE.SPECULAR )
		{
			if ( i_ss.specularMap == MAP_CHANNEL.EMPTY )
				finalOutput += Bits2ShaderFeature ("SPECULAR_MAP", 3 );
			else
				finalOutput += "\n\t#define SO_SPECULAR_MAP_SMOOTHNESS";
		}

		if ( i_ss.vertexOptions )
			finalOutput += Bits2ShaderFeature ("VERTEX", 2 );

		switch ( i_ss.reflectionType )
		{
		default:
			break;

		case REFLECTION_TYPE.REFLECTION_PROBE:
			finalOutput += "\t#pragma shader_feature SO_SF_REFLECT_PROBE_ON\n";
			break;

		case REFLECTION_TYPE.SPHERE_MAP:
			finalOutput += "\t#pragma shader_feature SO_SF_REFLECT_2D_ON\n";
			break;
		}

		switch ( i_ss.fogMode )
		{
		case FOG_MODE.SOLID:
			finalOutput += "\t#pragma shader_feature SO_SF_FOG_ON\n";
			finalOutput += "\t#define SO_GD_FOG_SOLID\n";
			break;

		case FOG_MODE.VOLUMETRIC:
			finalOutput += "\t#pragma shader_feature SO_SF_FOG_ON\n";
			finalOutput += "\t#define SO_GD_FOG_VOLUMETRIC\n";

			if ( i_ss.fogImageLight )
				finalOutput += "\t#define SO_GD_FOG_IMAGELIGHT_ON\n";
			break;

		case FOG_MODE.VOLUMETRIC_3D:
			finalOutput += "\t#pragma shader_feature SO_SF_FOG_ON\n";
			finalOutput += "\t#define SO_GD_FOG_VOLUMETRIC_3D\n";

			if ( i_ss.fogRoughness )
				finalOutput += "\t#define SO_GD_FOG_ROUGHNESS_ON\n";

			if ( i_ss.fogImageLight )
				finalOutput += "\t#define SO_GD_FOG_IMAGELIGHT_ON\n";

			finalOutput += "\t#define SO_GD_FOG_RAYCOUNT " + i_ss.fogRays + "\n";
			break;
		}


		if ( i_ss.bendType == BEND_TYPE.SIMPLE )
		{
			finalOutput += "\t#define SO_GD_BENDING_SHADERONE\n";
			finalOutput += "\t#pragma shader_feature SO_SF_BENDING_ON\n";
		}

		if ( i_ss.bendType == BEND_TYPE.CURVED_WORLD )
		{
			finalOutput += "\t#define SO_GD_BENDING_CURVEDWORLD\n";
			finalOutput += "\t#pragma shader_feature SO_SF_BENDING_ON\n";
		}

  		switch ( i_ss.colorPrecision )
		{
		case COLOR_PRECISION.FIXED:
			finalOutput += "\t#define SOFLOAT fixed\n";
			finalOutput += "\t#define SOFLOAT2 fixed2\n";
			finalOutput += "\t#define SOFLOAT3 fixed3\n";
			finalOutput += "\t#define SOFLOAT4 fixed4\n";
			break;

		case COLOR_PRECISION.HALF:
			finalOutput += "\t#define SOFLOAT half\n";
			finalOutput += "\t#define SOFLOAT2 half2\n";
			finalOutput += "\t#define SOFLOAT3 half3\n";
			finalOutput += "\t#define SOFLOAT4 half4\n";
			break;

		case COLOR_PRECISION.FLOAT:
			finalOutput += "\t#define SOFLOAT float\n";
			finalOutput += "\t#define SOFLOAT2 float2\n";
			finalOutput += "\t#define SOFLOAT3 float3\n";
			finalOutput += "\t#define SOFLOAT4 float4\n";
			break;
		}

		finalOutput += "\n";

		// TurboKid
		finalOutput += "\t//THIS IS THE FUTURE\n";
		finalOutput += "\t//THIS IS THE YEAR 1997\n";

		finalOutput += "\n";

		#if UNITY_5
			finalOutput += "\t#define SO_UNITY_5\n";
		#endif

		#if UNITY_2017
			finalOutput += "\t#define SO_UNITY_2017\n";
		#endif

		finalOutput += MakeLayerOptions ( i_ss, i_ss.layer0, 0 );
		finalOutput += MakeLayerOptions ( i_ss, i_ss.layer1, 1 );
		finalOutput += MakeLayerOptions ( i_ss, i_ss.layer2, 2 );
		finalOutput += MakeLayerOptions ( i_ss, i_ss.layer3, 3 );

		finalOutput += "\n";

		if ( i_ss.terrainType == TERRAIN_TYPE.MESH_TERRAIN )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_MESH_TERRAIN_ON\n";
		}

		if ( i_ss.terrainType == TERRAIN_TYPE.UNITY_TERRAIN )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_UNITY_TERRAIN_ON\n";
			finalOutput += "\t#pragma multi_compile __ _TERRAIN_NORMAL_MAP\n";
		}

		if (i_ss.instancing)
		{
			finalOutput += "\t#pragma multi_compile __ _INSTANCING_ON\n";
			finalOutput += "\t#pragma multi_compile_instancing\n";
		}

		if ( i_ss.distortion )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_DISTORT_HORZ_ON\n";
			finalOutput += "\t#pragma shader_feature SO_SF_DISTORT_VERT_ON\n";
			finalOutput += "\t#pragma shader_feature SO_SF_DISTORT_CIRCULAR_ON\n";
		}

		if ( i_ss.chromaticAbberation )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_RGBOFFSET_ON\n";
		}

		if ( i_ss.saturation )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_SATURATION_ON\n";
		}

		if ( i_ss.scanlines )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_SCANLINE_ON\n";
		}

		if ( i_ss.rimLighting )
		{
			finalOutput += "\t#pragma shader_feature _ SO_SF_RIMLIT_ADD SO_SF_RIMLIT_SUBTRACT\n";
		}

		if ( i_ss.intersect )
		{
			finalOutput += "\t#pragma shader_feature SO_SF_INTERSECT_ON\n";
		}

		if ( i_ss.emission )
		{
			finalOutput += "\t#pragma multi_compile __ _EMISSION\n";
		}

		finalOutput += "\n";

		return ( finalOutput );
    }

	//=========================================================================
	static public string ParseShaderPass ( ShaderOneSettings i_ss, bool i_basePass, string i_inputStr )
	{
		string[]	lines 	= i_inputStr.Split('\n');
		string 		finalOutput = "";
		int    		lineCur;

		for ( lineCur = 0; lineCur < lines.Length; lineCur++ )
		{
			if( lines[lineCur].IndexOf("//SO_OPTIONS_INSERT:") >= 0)
			{
				finalOutput += MakePassOptions ( i_ss, i_basePass );
				finalOutput += "\n";
			}
			else
			{
				finalOutput += lines[lineCur] + "\n";
			}
		}

		return ( finalOutput );
	}

	//=========================================================================
	public static void SaveTextFile ( string ifname, string idata )
	{
 		StreamWriter sw 	= new StreamWriter ( ifname );

		sw.Write ( idata );

    	sw.Close();

    	return;
    }

	//=========================================================================
    static public void ShaderOneMake ( ShaderOneSettings i_ss )
    {
		int    lineCur;
		string finalOutput = "";
		string shaderOneRoot = ShaderOneIO.FindRootFolder();

		templateDir = shaderOneRoot +"/Templates/";
		shaderDir   = shaderOneRoot + "/Shaders/";

        strMain 	= File.ReadAllText ( templateDir + shaderMainName );

        string[] lines = strMain.Split('\n');

        for ( lineCur = 0; lineCur < lines.Length; lineCur++ )
        {
            if(lines[lineCur].IndexOf("//SO_PASS_BASE_INSERT:") >= 0)
            {
				finalOutput += MakePass ( true, i_ss );
            }
			else if(lines[lineCur].IndexOf("//SO_PASS_ADD_INSERT:") >= 0)
			{
				if ( i_ss.renderPipeline == RENDER_PIPELINE.UNITY_FORWARD )
				{
					finalOutput += MakePass ( false, i_ss );
				}
			}
			else if(lines[lineCur].IndexOf("//SO_PASS_META_INSERT:") >= 0)
			{
				if ( i_ss.emission )
				{
					finalOutput += File.ReadAllText ( templateDir + shaderMetaName );
				}
			}
			else if(lines[lineCur].IndexOf("//SO_SHADOW_CASTER_INSERT:") >= 0)
			{
				if ( i_ss.shadows || i_ss.intersect )
				{
					finalOutput += File.ReadAllText ( templateDir + shaderShadowCasterName );
				}
			}
			else
				finalOutput += lines[lineCur] + "\n";
        }

        SaveTextFile ( shaderDir + "ShaderOneGen.shader", finalOutput );

		finalOutput = MakeDefines(i_ss);
		finalOutput += AlphaAndSurfaceMapDefines();
		SaveTextFile ( shaderDir + "ShaderOneGen_BitDecode.cginc", finalOutput );

		finalOutput = SurfaceMapInlineFuncs ( typeof (MAP_CHANNEL) );
		finalOutput += AlphaMapInlineFuncs ( typeof (MAP_CHANNEL) );

		if ( i_ss.workflow == WORKFLOW_MODE.SPECULAR )
			finalOutput += SpecularMapInlineFuncs ( typeof (MAP_CHANNEL) );

		SaveTextFile ( shaderDir + "ShaderOneGen_FuncMaps.cginc", finalOutput );

		finalOutput = MakeInstancingFile( i_ss, templateDir );
		SaveTextFile ( shaderDir + "ShaderOneGen_Instancing.cginc", finalOutput );

		finalOutput = MakeLightMacrosFile ( i_ss, templateDir );
		SaveTextFile ( shaderDir + "ShaderOneGen_UnityLightMacros.cginc", finalOutput );
	}
}

#endif
