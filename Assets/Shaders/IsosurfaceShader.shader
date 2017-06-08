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
		_ClipX("clipX", Float) = 0
		_ClipY("clipY", Float) = 0
		_ClipZ("clipZ", Float) = 0
		_AmbientR("Ambient color red component", Float) = 0
		_AmbientG("Ambient color green component", Float) = 0
		_AmbientB("Ambient color blue component", Float) = 0
		_DiffuseR("Diffuse color red component", Float) = 0
		_DiffuseG("Diffuse color green component", Float) = 0.5
		_DiffuseB("Diffuse color blue component", Float) = 0.5
		_SpecularR("Specular color red component", Float) = 1
		_SpecularG("Specular color green component", Float) = 1
		_SpecularB("Specular color blue component", Float) = 1
		_Shininess("Shininess", Float) = 32
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
	#include "UnityLightingCommon.cginc"

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
	float _ClipZ;
	float _AmbientR, _AmbientG, _AmbientB;
	float _DiffuseR, _DiffuseG, _DiffuseB;
	float _SpecularR, _SpecularG, _SpecularB;
	float _Shininess;

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
		float3 backObj = tex2D(_BackTex, tc.xy).xyz;
		float3 frontObj = tex2D(_FrontTex, tc.xy).xyz;

		// ray throush the volume
		float3 dir = backObj.xyz - frontObj.xyz;
		float length = distance(frontObj, backObj);
		float step = _Step * _StepFactor;
		float3 stepDir = step * dir;

		// walk along the ray sampling the volume
		float3 pos = frontObj, worldPos;
		float4 color = float4(0, 0, 0, 0);
		float4 ambient = float4(_AmbientR, _AmbientG, _AmbientB, 1),
			diffuse = float4(_DiffuseR, _DiffuseG, _DiffuseB, 1),
			specular = float4(_SpecularR, _SpecularG, _SpecularB, 1);
		float3 posColor = float3(0, 0, 0);
		float3 normal, reflectDir, viewDir, lightDir;
		float3 stepDirX, stepDirY;
		float dx, dy, dz;
		for (int i = 0; i < 350; i++)
		{
			if (distance(pos, backObj) < 0.5 * step) break; // check when reach the back
			if (pos.x < _ClipX || pos.y < _ClipY || pos.z < _ClipZ)
			{
				pos += stepDir;
				continue;
			}
			posColor = tex3Dlod(_Volume, float4(pos, 0)).rrr;
			if (distance(posColor, float3(0, 0, 0)) > _IsosurfaceThreshold)
			{
				// count normal
				stepDirX = float3(stepDir.x, -stepDir.z, stepDir.y); // rotate stepDir 90 degrees around X axis
				stepDirY = float3(-stepDir.z, stepDir.y, stepDir.x); // rotate stepDir 90 degrees around Y axis
				// density difference between (pos - stepDir) and (pos + stepDir) in 3 directions
				dx = tex3Dlod(_Volume, float4(pos - stepDirX, 0)).r - tex3Dlod(_Volume, float4(pos + stepDirX, 0)).r;
				dy = tex3Dlod(_Volume, float4(pos - stepDirY, 0)).r - tex3Dlod(_Volume, float4(pos + stepDirY, 0)).r;
				dz = tex3Dlod(_Volume, float4(pos - stepDir, 0)).r - tex3Dlod(_Volume, float4(pos + stepDir, 0)).r;
				normal = dx * stepDirX + dy * stepDirY + dz * stepDir;
				// normal to world coordinates
				normal = mul(unity_ObjectToWorld, normal);
				normal = normalize(normal);
				// phong lighting model
				lightDir = normalize(_WorldSpaceLightPos0.xyz);
				reflectDir = reflect(-lightDir, normal);
				reflectDir = normalize(reflectDir);
				worldPos = mul(unity_ObjectToWorld, pos);
				viewDir = normalize(_WorldSpaceCameraPos - worldPos);
				color = ambient + diffuse * max(0.0, dot(normal, lightDir)) + specular * pow(max(0.0, dot(reflectDir, viewDir)), _Shininess);
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