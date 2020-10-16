// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>
		Pass
		{
			Name "Caster"
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature SO_SF_BLEND_BIT1
			#pragma shader_feature SO_SF_BLEND_BIT2
			#pragma shader_feature SO_SF_BLEND_BIT3

			#include "UnityCG.cginc"
			#include "ShaderOneGen_BitDecode.cginc"
			#include "ShaderOneDefine.cginc"

			struct sv2f
			{
				V2F_SHADOW_CASTER;
#ifdef SO_BLEND_CUTOUT
				float2  uv : TEXCOORD1;
#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D   _MainTex;
			uniform float4 		_MainTex_ST;
			uniform half4 		_Color;

			sv2f vert( appdata_base v )
			{
				sv2f o;
				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

#ifdef SO_BLEND_CUTOUT
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
#endif

				return o;
			}

			float4 frag( sv2f i ) : SV_Target
			{

#ifdef SO_BLEND_CUTOUT
				fixed4 col = tex2D ( _MainTex, i.uv ) * _Color.rgba;
				clip ( col.a - 0.5 );
#endif

				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
	  }




