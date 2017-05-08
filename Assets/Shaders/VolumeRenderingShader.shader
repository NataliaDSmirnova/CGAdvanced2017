Shader "CGA/VolumeRendering"
{
	Properties
	{
    _Volume("VolumeTex", 3D) = "" {}
    _BackTex("BackFaceTex", 2D) = "white" {}
    _FronTex("FrontFaceTex", 2D) = "white" {}
    _Step("Step size", Float) = 0.05
    _StepFactor("Step factor", Range(0.5, 2.0)) = 1.0    
    _Opacity("Opacity border", Float) = 0.0
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
      float _Opacity;
      float _ClipX;
      float _ClipY;

      // Used for 
      float3 transferFunctionColor(float density)
      {
        if (density >= 0 && density <= 0.08) // 0 - 0.08 - 'Skin'
          return float3(225.0 / 256.0, 223.0 / 256.0, 196.0 / 256.0);
        else if (density > 0.08 && density <= 0.45) // 0.08 - 0.45 - 'Brain'
          return float3(240.0 / 256.0, 200.0 / 256.0, 201.0 / 256.0);
        else if (density > 0.45 && density <= 0.9) // 0.45 - 0.9 - 'Bone'
          return float3(227.0 / 256.0, 218.0 / 256.0, 201.0 / 256.0);
        else //if (density > 0.9 && density <= 1.0) // 0.9 - 1.0 - 'Other'
          return float3(255.0 / 256.0, 255.0 / 256.0, 255.0 / 256.0);
      }

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
        float step = _Step * _StepFactor;
        float3 stepDir = step * dir;

        // walk along the ray sampling the volume
        float3 pos = front, 
          objectPos;
        float alpha = 0, density = 0, compositeTransparency = 1;
        float3 sampledColor = float3(0, 0, 0),
          color = 0,
          compositeColor = 0;          

        for (int i = 0; i < 30; i++)
        {
          if (distance(pos, back) < step * 0.5 || compositeTransparency < _Opacity) 
            break; // check when reach the back  

          objectPos = 2 * pos - 1;
          objectPos = mul(unity_WorldToObject, objectPos);

          if (objectPos.x < _ClipX || objectPos.y < _ClipY)
          {
            pos += stepDir;
            continue;
          }

          density = tex3D(_Volume, pos.xyz).r;
          sampledColor = transferFunctionColor(density);
          alpha = density;

          compositeColor += alpha * sampledColor * compositeTransparency;
          compositeTransparency *= (1 - alpha);
          
          pos += stepDir;
        }
        color = compositeColor;
        // temp color
        return float4(color, 1);
      }
      ENDCG
    }
  }
}
