Shader "CGA/ClipPlaneShader" {

	Properties{
		_Opacity("opacity", Range(0, 1)) = 0.6
		_ClipX("clipX", float) = 0.0
	}

		SubShader{
		Tags
	{
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
	}

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM

#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag

		float _Opacity;
	float _ClipX;

	struct VertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct VertexOutput
	{
		float4 vertex : SV_POSITION;
		float4 objectPos: TEXTCOORD0;
		fixed4 color : COLOR;
	};

	VertexOutput vert(VertexInput vertexInput)
	{
		VertexOutput vertexOutput;

		vertexOutput.vertex = mul(UNITY_MATRIX_MVP, vertexInput.vertex);
		vertexOutput.objectPos = vertexInput.vertex;
		float3 vertexNormal = abs(vertexInput.normal);
		vertexOutput.color = float4(vertexNormal.x, vertexNormal.y, vertexNormal.z, _Opacity);
		return vertexOutput;
	}

	fixed4 frag(VertexOutput vertexOutput) : SV_Target
	{
		if (vertexOutput.objectPos.x < _ClipX)
		discard;
	return vertexOutput.color;
	}

		ENDCG
	}
	}

		FallBack "VertexLit"

}