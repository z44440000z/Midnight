//-------------------------------------------------------------------------------------
inline half3 BoxProjectedCubemapDirectionFast ( half3 worldRefl, float3 worldPos, float4 cubemapCenter, float4 boxMin, float4 boxMax)
{
    UNITY_BRANCH
    if ( cubemapCenter.w > 0.0)
    {
        half3 nrdir = normalize(worldRefl);

        half3 rbmax = ( boxMax.xyz - worldPos );
        half3 rbmin = ( boxMin.xyz - worldPos );

        half3 select = step ( nrdir, half3(0,0,0) );
        half3 rbminmax = lerp (rbmax, rbmin, select);
        rbminmax /= nrdir;

        half fa = min ( min ( rbminmax.x, rbminmax.y ), rbminmax.z );

        worldPos -= cubemapCenter.xyz;
        worldRefl = worldPos + nrdir * fa;
    }

    return worldRefl;
}

#ifndef UNITY_SPECCUBE_LOD_STEPS
#define UNITY_SPECCUBE_LOD_STEPS (6)
#endif

#ifdef UNITY_COLORSPACE_GAMMA
	#define DielectricSpec SOFLOAT4(0.220916301, 0.220916301, 0.220916301, 1.0 - 0.220916301)
	#define ColorSpaceDouble fixed4(2.0, 2.0, 2.0, 2.0)
#else
	#define DielectricSpec SOFLOAT4(0.04, 0.04, 0.04, 1.0 - 0.04)
	#define ColorSpaceDouble fixed4(4.59479380, 4.59479380, 4.59479380, 2.0)
#endif

/*
#ifdef SO_GD_PIPELINE_UNITY_FORWARD

#ifndef SO_UNITY_5
	#ifdef POINT
		#define LIGHT_ATTENUATION_ONLY(a)    (tex2D(_LightTexture0, dot(a._LightCoord,a._LightCoord).rr).UNITY_ATTEN_CHANNEL )
	#endif

	#ifdef SPOT
		#define LIGHT_ATTENUATION_ONLY(a)    ( (a._LightCoord.z > 0) * UnitySpotCookie(a._LightCoord) * UnitySpotAttenuate(a._LightCoord.xyz) )
	#endif

	#ifdef DIRECTIONAL
		#define LIGHT_ATTENUATION_ONLY(a)     1
	#endif

	#ifdef POINT_COOKIE
		#define LIGHT_ATTENUATION_ONLY(a)    ( tex2D(_LightTextureB0, dot(a._LightCoord,a._LightCoord).rr).UNITY_ATTEN_CHANNEL * texCUBE(_LightTexture0, a._LightCoord).w )
	#endif

	#ifdef DIRECTIONAL_COOKIE
		#define LIGHT_ATTENUATION_ONLY(a)    ( tex2D(_LightTexture0, a._LightCoord ).w )
	#endif
#else

	#ifdef POINT
	#ifndef SO_SHADOWS_OFF
	#   define UNITY_LIGHT_ATTENUATION_SHADOW(destName, destShadow, input, worldPos) \
			unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1)).xyz; \
			destShadow = UNITY_SHADOW_ATTENUATION(input, worldPos); \
			destName = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).r;
	#else
	#   define UNITY_LIGHT_ATTENUATION_NOSHADOW(destName, input, worldPos) \
			unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1)).xyz; \
			destName = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).r;
	#endif
	#endif

	#ifdef SPOT
	inline fixed UnitySpotCookie2(unityShadowCoord4 LightCoord)
	{
		return tex2D(_LightTexture0, LightCoord.xy / LightCoord.w + 0.5).w;
	}
	inline fixed UnitySpotAttenuate2(unityShadowCoord3 LightCoord)
	{
		return tex2D(_LightTextureB0, dot(LightCoord, LightCoord).xx).r;
	}
	#if !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
	#define DECLARE_LIGHT_COORD(input, worldPos) unityShadowCoord4 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1))
	#else
	#define DECLARE_LIGHT_COORD(input, worldPos) unityShadowCoord4 lightCoord = input._LightCoord
	#endif

	#ifndef SO_SHADOWS_OFF

	#   define UNITY_LIGHT_ATTENUATION_SHADOW(destName, destShadow, input, worldPos) \
			DECLARE_LIGHT_COORD(input, worldPos); \
			destShadow = UNITY_SHADOW_ATTENUATION(input, worldPos); \
			destName = (lightCoord.z > 0) * UnitySpotCookie2(lightCoord) * UnitySpotAttenuate2(lightCoord.xyz);
	#else
	#   define UNITY_LIGHT_ATTENUATION_NOSHADOW(destName, input, worldPos) \
			DECLARE_LIGHT_COORD(input, worldPos); \
			destName = (lightCoord.z > 0) * UnitySpotCookie2(lightCoord) * UnitySpotAttenuate2(lightCoord.xyz);
	#endif
	#endif

	#ifdef DIRECTIONAL
	#ifndef SO_SHADOWS_OFF
	#   define UNITY_LIGHT_ATTENUATION_SHADOW(destName, destShadow, input, worldPos) destShadow = UNITY_SHADOW_ATTENUATION(input, worldPos); destName = 1;
	#else
	#   define UNITY_LIGHT_ATTENUATION_NOSHADOW(destName, input, worldPos);
	#endif
	#endif

	#ifdef POINT_COOKIE
	#   if !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
	#       define DECLARE_LIGHT_COORD(input, worldPos) unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1)).xyz
	#   else
	#       define DECLARE_LIGHT_COORD(input, worldPos) unityShadowCoord3 lightCoord = input._LightCoord
	#   endif

	#ifndef SO_SHADOWS_OFF

	#   define UNITY_LIGHT_ATTENUATION_SHADOW(destName, destShadow, input, worldPos) \
			DECLARE_LIGHT_COORD(input, worldPos); \
			destShadow = UNITY_SHADOW_ATTENUATION(input, worldPos); \
			destName = tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).r * texCUBE(_LightTexture0, lightCoord).w ;
	#else
	#   define UNITY_LIGHT_ATTENUATION_NOSHADOW(destName, input, worldPos) \
			DECLARE_LIGHT_COORD(input, worldPos); \
			destName = tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).r * texCUBE(_LightTexture0, lightCoord).w ;
	#endif
	#endif

	#ifdef DIRECTIONAL_COOKIE
	#   if !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
	#       define DECLARE_LIGHT_COORD(input, worldPos) unityShadowCoord2 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1)).xy
	#   else
	#       define DECLARE_LIGHT_COORD(input, worldPos) unityShadowCoord2 lightCoord = input._LightCoord
	#   endif
	#ifndef SO_SHADOWS_OFF
	#   define UNITY_LIGHT_ATTENUATION_SHADOW(destName, destShadow, input, worldPos) \
			DECLARE_LIGHT_COORD(input, worldPos); \
			destShadow = UNITY_SHADOW_ATTENUATION(input, worldPos); \
			destName = tex2D(_LightTexture0, lightCoord).w;
	#else
	#   define UNITY_LIGHT_ATTENUATION_NOSHADOW(destName, input, worldPos) \
			DECLARE_LIGHT_COORD(input, worldPos); \
			destName = tex2D(_LightTexture0, lightCoord).w;
	#endif
	#endif

	#endif
#endif
 */
