// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

//=================================================================
		v2f vert (appdata v)
		{
			v2f o;
			UNITY_INITIALIZE_OUTPUT ( v2f, o );

			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);

			SOLightingData sold;
			UNITY_INITIALIZE_OUTPUT ( SOLightingData, sold );

			float4 wpos;

	#if defined (SO_SF_BENDING_ON) && defined(SO_GD_BENDING_CURVEDWORLD)
			V_CW_TransformPointAndNormal ( v.vertex, v.normal, v.tangent );
	#endif

//#ifdef SO_VERTEX_MOVEMENT
//			WindMovementHQ ( v.vertex, v.color, v.uv );
//#endif

			wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));

	#if defined (SO_SF_BENDING_ON) && defined(SO_GD_BENDING_SHADERONE)
			float3 curveOffset = wpos.xyz;

			curveOffset.xyz -= _SO_BendPivot.xyz;
			curveOffset.xyz = float3( 0.0f, ( curveOffset.x * curveOffset.x * -_SO_BendAmountX ) + ( curveOffset.z * curveOffset.z * -_SO_BendAmountZ ) , 0.0f );

			wpos.xyz	+=  curveOffset.xyz;
	#endif


	//=============================================================================
#ifdef SO_WORLDNORMAL_ON
			sold.worldNormal 	= UnityObjectToWorldNormal ( v.normal );
	#ifdef SO_WORLDNORMAL_VARY_ON
			o.normalDir 		= sold.worldNormal;
	#endif
#endif

#ifdef SO_VERTEX_MOVEMENT
		WindMovement ( wpos, v.color );
/*
			float4 _Wind = float4(1,1,1,1);
			float4    wind = _Wind;
			float     bendingFact    = v.color.a;
			float4    windParams    = float4(0,0.1,bendingFact.xx);
			float     windTime         = _Time.y * float2(0.5,1);


			wind.xyz    = mul((float3x3)unity_WorldToObject,_Wind);
			wind.w        = _Wind.w  * bendingFact;


			float4 tpos = AnimateVertex ( v.vertex, v.normal, windParams, wind, windTime  );
			o.pos = UnityObjectToClipPos( tpos );
*/
#else
			o.pos = mul( UNITY_MATRIX_VP, wpos );
#endif


#ifdef SO_POSWORLD_VARY_ON
			o.positionWorld 	= wpos.xyz;
			sold.positionWorld 	= o.positionWorld;    // if lighting on only i think
#endif



	#ifdef SO_TERRAIN_ON
			o.controlUV = TRANSFORM_TEX ( v.uv, _Control );
	#endif

#ifdef SO_VERTEX_SPLAT
			o.vertexSplatControl = v.color;
#endif




// only calculate this crapola once
//=============================================================================
#if defined (SO_BUMP_ON) || defined (SO_PARALLAX_ON)
	float4 tangent;
	float4 worldTangent;

	#ifdef CALC_TANGENT_ON
		tangent.xyz			= cross ( v.normal, float3 ( 0, 0, 1 ) );
		tangent.w 			= -1;
		worldTangent.xyz 	= UnityObjectToWorldDir ( tangent );
		worldTangent.w 		= -1;
	#else
		tangent = v.tangent;
		worldTangent.xyz 	=  UnityObjectToWorldDir ( v.tangent.xyz );
		worldTangent.w 		= v.tangent.w;
	#endif

		float3 binormal 	= cross ( sold.worldNormal, worldTangent ) * worldTangent.w;

#endif

//=============================================================================
#ifdef SO_BUMP_ON
		CalcVertBumpMap ( sold.worldNormal, worldTangent, binormal, o.tspace0, o.tspace1, o.tspace2 );
#endif

//=============================================================================
#ifdef SO_PARALLAX_ON
		o.surfaceCoords = CalcVertParallax ( v.vertex, v.normal, tangent, binormal );
#endif

//=============================================================================
#ifdef SO_SF_INTERSECT_ON
	   o.screenPos 	= ComputeScreenPos(o.pos);
#endif

//=============================================================================
// LAYER 0 ( mainText )
		Layer0VertSetup ( sold, o, v );
		Layer1VertSetup ( sold, o, v );
		Layer2VertSetup ( sold, o, v );
		Layer3VertSetup ( sold, o, v );

// move all this shit to frag
#ifdef SO_FOG_VERTEX_UV
		o.fogUV = o.positionWorld * half3( _FogScale, _FogScale, _FogScale );  // scale it all
		o.fogUV.x += _Time.y * _FogMoveX;
		o.fogUV.y += _Time.y * _FogMoveY;
		o.fogUV.z += _Time.y * _FogMoveZ;
#endif

//=============================================================================
#ifdef LIGHTMAP_ON
		o.lmUV 	 = ( unity_LightmapST.xy * v.texcoord1 ) + unity_LightmapST.zw;
#endif

//=============================================================================
#ifdef DYNAMICLIGHTMAP_ON
		o.dlmUV 	  	= ( unity_DynamicLightmapST.xy * v.texcoord2 ) + unity_DynamicLightmapST.zw;
#endif

//=============================================================================
#ifdef SO_VERTEX_COLOR
		//SCOTFIND * 2 only for particles
	#ifdef SO_PREMULTIPLY_ALPHA
		o.vertexColor = v.color * 2;
	#else
		o.vertexColor = v.color;
	#endif
#endif

#ifdef SO_SHADOWS
		TRANSFER_SHADOW(o)
#else
	#ifdef SO_LIGHTING_ON
 		TRANSFER_VERTEX_TO_FRAGMENT(o);
	#endif
#endif

#ifdef SO_LIGHT_PROBE_VERTEX
   	o.vertexLighting.rgb += ShadeSH9(float4(UnityObjectToWorldNormal(v.normal),1));
#endif

#ifdef SO_VERTEX_LIGHTING_ON

	#ifdef SO_GD_PIPELINE_SHADER_ONE
		int index;
		sold.metallic 				= LAYER0_METALLIC;
		sold.perceptualRoughness 	= LAYER0_GLOSSINESS;

		#ifdef SO_VIEWDIR_ON
		sold.viewDir = normalize( _WorldSpaceCameraPos.xyz - o.positionWorld );
		#endif

		ShaderOneDirectionalVert ( sold );
		ShaderOnePointVert ( sold );
   		ShaderOneSpotVert ( sold );

		#ifndef SO_SPECULAR_ONLY
		o.vertexLighting.rgb += sold.lightRGB;
		#endif

		#ifdef SO_SF_SPECULAR_ON
		o.vertexLighting.rgb += sold.specularLight;
		#endif

	#else
		#ifdef VERTEXLIGHT_ON
		o.vertexLighting.rgb += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
							unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
							unity_4LightAtten0, sold.positionWorld, sold.worldNormal );
		#endif
	#endif

#else


#endif

			return o;
		}


