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
		float4 ambient = float4(_AmbientR, _AmbientG, _AmbientB, 1),
			diffuse = float4(_DiffuseR, _DiffuseG, _DiffuseB, 1),
			specular = float4(_SpecularR, _SpecularG, _SpecularB, 1);
		float3 posColor = float3(0, 0, 0);
		float3 normal, reflectDir, viewDir, lightDir;
		float3 stepDirX, stepDirY, stepDirZ;
		float dx, dy, dz;
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
			objectPos = objectPos + 0.5;
			posColor = tex3D(_Volume, objectPos.xyz).rrr;
			if (distance(posColor, float3(0, 0, 0)) > _IsosurfaceThreshold)
			{
				// count normal
				stepDirX = float3(stepDir.x, -stepDir.z, stepDir.y); // rotate stepDir 90 degrees around X axis
				stepDirY = float3(-stepDir.z, stepDir.y, stepDir.x); // rotate stepDir 90 degrees around Y axis
				// stepDirX, stepDirY, stepDir to object coordinates
				stepDirX = 2 * stepDirX - 1; 
				stepDirX = mul(unity_WorldToObject, stepDirX);
				stepDirX = stepDirX + 0.5;
				stepDirY = 2 * stepDirY - 1;
				stepDirY = mul(unity_WorldToObject, stepDirY);
				stepDirY = stepDirY + 0.5;
				stepDirZ = 2 * stepDir - 1;
				stepDirZ = mul(unity_WorldToObject, stepDirZ);
				stepDirZ = stepDirZ + 0.5;
				// density difference between (pos - stepDir) and (pos + stepDir) in 3 directions
				dx = tex3D(_Volume, (objectPos - stepDirX).xyz).r - tex3D(_Volume, (objectPos + stepDirX).xyz).r;
				dy = tex3D(_Volume, (objectPos - stepDirY).xyz).r - tex3D(_Volume, (objectPos + stepDirY).xyz).r;
				dz = tex3D(_Volume, (objectPos - stepDirZ).xyz).r - tex3D(_Volume, (objectPos + stepDirZ).xyz).r;
				normal = dx * stepDirX + dy * stepDirY + dz * stepDirZ;
				normal = normal - 0.5; // normal to world coordinates
				normal = mul(unity_ObjectToWorld, normal);
				normal = normal + 1.0;
				normal = normalize(normal);
				// phong lighting model
				lightDir = normalize(_WorldSpaceLightPos0.xyz + 1.0);
				reflectDir = reflect(-lightDir, normal);
				reflectDir = normalize(reflectDir);
				viewDir = normalize((_WorldSpaceCameraPos + 1.0) * 0.5 - pos);
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