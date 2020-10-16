// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

Pass
{
    Name "META"
    Tags {"LightMode"="Meta"}
    Cull Off
    CGPROGRAM

    #include"UnityStandardMeta.cginc"

    //float4 _EmissionColor;
	sampler2D _EmissionTex;
	float  _EmissionScale;

    float4 frag_meta2 (v2f_meta i): SV_Target
    {
        FragmentCommonData data = UNITY_SETUP_BRDF_INPUT (i.uv);
        UnityMetaInput o;
        UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

        o.Albedo = tex2D ( _EmissionMap, i.uv ).rgb;

        o.Emission = _EmissionColor.rgb;
        return UnityMetaFragment(o);
    }

    #pragma vertex vert_meta
    #pragma fragment frag_meta2
    #pragma shader_feature _EMISSION
    #pragma shader_feature _METALLICGLOSSMAP
    #pragma shader_feature ___ _DETAIL_MULX2
    ENDCG

}




