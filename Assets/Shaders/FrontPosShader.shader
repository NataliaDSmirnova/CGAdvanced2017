// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CGA/CullBackShader" {

	Properties{
		_Opacity("opacity", Range(0, 1)) = 0.6
	}

		SubShader{
		Tags
	{
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
	}
		Cull Back

		Pass
	{
		CGPROGRAM

#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag	

		struct VertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct VertexOutput
	{
		float4 vertex : SV_POSITION;
		float4 wpos : TEXCOORD0;
  };

	VertexOutput vert(VertexInput vertexInput)
	{
		VertexOutput vertexOutput;

		vertexOutput.vertex = mul(UNITY_MATRIX_MVP, vertexInput.vertex);
	    vertexOutput.wpos = mul(unity_ObjectToWorld, vertexInput.vertex);
        vertexOutput.wpos = (vertexOutput.wpos + 1.0) * 0.5;
    return vertexOutput;
	}

	float4 frag(VertexOutput vertexOutput) : SV_Target
	{
		return vertexOutput.wpos;
  }

		ENDCG
	}
	}

		FallBack "VertexLit"

}