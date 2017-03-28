// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CGA/DiffuseShader"
{
	Properties
	{
		_Light("Light color", Color) = (1,1,0,1)
		_Ambient("Ambient color", Color) = (0,0,0,1)
		_Diffuse("Object color", Color) = (1,0,1,1)
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque"
		"LightMode" = "ForwardBase" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"


		fixed4 _Light;
	fixed4 _Ambient;
	fixed4 _Diffuse;

	struct appdata
	{
		float4 vertex : POSITION;
		float3 norm : NORMAL;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 norm : NORMAL;
	};


	v2f vert(appdata input)
	{
		v2f output;
		output.vertex = UnityObjectToClipPos(input.vertex);
		output.norm = normalize(UnityObjectToWorldNormal(input.norm));
		return output;
	}

	fixed4 frag(v2f input) : SV_Target
	{
		fixed4 col = _Ambient + _Diffuse * _Light * max(0.0, dot(input.norm, _WorldSpaceLightPos0.xyz));
	return col;
	}
		ENDCG
	}
	}
}
