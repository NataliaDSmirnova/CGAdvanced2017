Shader "CGA/X-Ray"
{
  Properties
  {
    _Volume("VolumeTex", 3D) = "" {}
    _BackTex("BackFaceTex", 2D) = "white" {}
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
 
        v2f vert(appdata v)
        {
          v2f o;
          o.vertex = mul(UNITY_MATRIX_MVP, v.vertex); 
          o.uvw = v.uvw;
          o.screenSpacePos = o.vertex;
          return o;
        }

        struct PosColor
        {
          float4 pos;
          float4 color;
        };

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
          float step = 0.05;
          float3 stepDir = step * dir;

          // walk along the ray sampling the volume
          float3 pos = front;
          float3 color = float3(0, 0, 0);
          for (int i = 0; i < 30; i++)
          {
            if (distance(pos, back) < step * 0.5) break; // check when reach the back  
            color += tex3D(_Volume, pos.xyz).rgb * 0.05;
            pos += stepDir;
          }
          // temp color
          return float4 (color, max(color.b, max(color.r, color.g)));
        }
      ENDCG
    }
  }
}

