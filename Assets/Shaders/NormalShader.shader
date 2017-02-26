Shader "Custom/NormalShader" {

	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTexture("MainTexture", 2D) = "white" {}
	}

	SubShader {
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

				#pragma vertex ComputeVertex
				#pragma fragment ComputeFragment

				sampler2D _MainTex;
				fixed4 _Color;

				struct VertexInput
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct VertexOutput
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				VertexOutput ComputeVertex(VertexInput vertexInput)
				{
					VertexOutput vertexOutput;

					vertexOutput.vertex = mul(UNITY_MATRIX_MVP, vertexInput.vertex);
					vertexOutput.texcoord = vertexInput.texcoord;
					float3 vertexNormal = abs(vertexInput.normal);
					vertexOutput.color = float4(vertexNormal.x, vertexNormal.y,
						vertexNormal.z, 1);
					return vertexOutput;
				}

				fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target
				{
					return tex2D(_MainTex, vertexOutput.texcoord) * vertexOutput.color;
				}

				ENDCG
			}
	}

	FallBack "VertexLit"

}