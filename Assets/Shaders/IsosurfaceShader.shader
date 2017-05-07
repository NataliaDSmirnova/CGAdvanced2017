Shader "CGA/Isosurface"
{
	Properties
	{
	    _Volume("VolumeTex", 3D) = "" {}
		_BackTex("BackFaceTex", 2D) = "white" {}
		_FronTex("FrontFaceTex", 2D) = "white" {}
		_Step("Step size", Float) = 0.05
		_StepFactor("Step factor", Range(0.5, 2.0)) = 1.0
		_IsosurfaceThreshold("Isosurface threshold", Float) = 0.05
		_ClipX("clipX", Float) = -0.5
		_ClipY("clipY", Float) = -0.5
	}

		SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		//		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		Cull Back
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 uvw : TEXCOORD0;
	};

	struct v2f
	{
		float4 screenSpacePos : TEXCOORD0;
		float3 uvw : TEXCOORD1;
		float4 vertex : SV_POSITION;
	};

	sampler2D _BackTex;
	sampler2D _FrontTex;

	sampler3D _Volume;
	float _Step;
	float _StepFactor;
	float _IsosurfaceThreshold;
	float _ClipX;
	float _ClipY;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uvw = v.uvw;
		o.screenSpacePos = o.vertex;
		return o;
	}

	float4 frag(v2f pix) : SV_TARGET0
	{
		// screencoordinates
		float3 tc = pix.screenSpacePos.xyz / pix.screenSpacePos.w * 0.5 + 0.5;
		// get front, back pos for ray in [0, 1] cube
		float3 back = tex2D(_BackTex, tc.xy).xyz;
		float3 front = tex2D(_FrontTex, tc.xy).xyz;

		// ray throush the volume
		float3 dir = back.xyz - front.xyz;
		float length = distance(front, back);
		float step = _Step * _StepFactor;
		float3 stepDir = step * dir;

		// walk along the ray sampling the volume
		float3 pos = front, objectPos;
		float4 color = float4(0, 0, 0, 0);
		float3 posColor = float3(0, 0, 0);
		for (int i = 0; i < 30; i++)
		{
			if (distance(pos, back) < step * 0.5) break; // check when reach the back
			objectPos = 2 * pos - 1;
			objectPos = mul(unity_WorldToObject, objectPos);
			if (objectPos.x < _ClipX || objectPos.y < _ClipY)
			{
				pos += stepDir; 
				continue;
			}
			posColor = tex3D(_Volume, pos.xyz).rrr;
			if (distance(posColor, float3(0, 0, 0)) > _IsosurfaceThreshold)
			{
				color = float4(0, 0.5, 0.5, 1);
				break;
			}
			pos += stepDir;
		}

		return color;
	}
		ENDCG
	}
	}
}

