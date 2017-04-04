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

		float _Opacity;

	struct VertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct VertexOutput
	{
		float4 vertex : SV_POSITION;
	};

	VertexOutput vert(VertexInput vertexInput)
	{
		VertexOutput vertexOutput;

		vertexOutput.vertex = mul(UNITY_MATRIX_MVP, vertexInput.vertex);
		float3 vertexNormal = abs(vertexInput.normal);
		return vertexOutput;
	}

	fixed4 frag(VertexOutput vertexOutput) : SV_Target
	{
		return vertexOutput.vertex;
	}

		ENDCG
	}
	}

		FallBack "VertexLit"

}