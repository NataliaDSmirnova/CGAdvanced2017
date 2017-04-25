Shader "CGA/Isosurface"
{
	Properties
	{
		_FronTex("FrontFaceTex", 2D) = "white" {}
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
	};

	struct v2f
	{
		float4 screenSpacePos : TEXCOORD0;
		float4 wpos : TEXTCOORD1;
		float4 vertex : SV_POSITION;
	};

	sampler2D _FrontTex;
	float _ClipX;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.screenSpacePos = o.vertex;
		o.wpos = mul(unity_ObjectToWorld, v.vertex);
		o.wpos = (o.wpos + 1.0) * 0.5;
		return o;
	}

	float4 frag(v2f pix) : SV_TARGET0
	{
		// screencoordinates
		float3 tc = pix.screenSpacePos.xyz / pix.screenSpacePos.w * 0.5 + 0.5;
		// get front pos for ray in [0, 1] cube
		float3 front = tex2D(_FrontTex, tc.xy).xyz;

		float4 color = float4(0, 0, 0, 0);
		if (distance(pix.wpos.xyz, front) < 0.0001)
			color = float4(0.5, 0, 0.5, 1);

		return color;
	}
		ENDCG
	}
	}
}

