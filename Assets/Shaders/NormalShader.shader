Shader "CGA/NormalCullBackShader" {

	Properties {
		_Opacity("opacity", Range(0, 1)) = 0.6
	}

	SubShader {
			Tags
			{
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}

			Blend SrcAlpha OneMinusSrcAlpha
		    Cull Back

			Pass
			{
				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex ComputeVertex
				#pragma fragment ComputeFragment

				float _Opacity;

				struct VertexInput
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct VertexOutput
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
				};

				VertexOutput ComputeVertex(VertexInput vertexInput)
				{
					VertexOutput vertexOutput;

					vertexOutput.vertex = mul(UNITY_MATRIX_MVP, vertexInput.vertex);
					float3 vertexNormal = abs(vertexInput.normal);
					vertexOutput.color = float4(vertexNormal.x, vertexNormal.y, vertexNormal.z, _Opacity);
					return vertexOutput;
				}

				fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target
				{
					return vertexOutput.color;
				}

				ENDCG
			}
	}

	FallBack "VertexLit"

}