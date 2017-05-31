﻿Shader "CGA/X-Ray"
{
  Properties
  {
    _Volume("VolumeTex", 3D) = "" {}
    _BackTex("BackFaceTex", 2D) = "white" {}
    _FronTex("FrontFaceTex", 2D) = "white" {}
    _Step("Step size", Float) = 0.05
    _StepFactor("Step factor", Range(0.5, 2.0)) = 1.0
    _Opacity("Opacity border", Float) = 0.0  
    _ColorFactor("ColorFactor", Float) = 1.0
	  _ClipX("clipX", Float) = 0
	  _ClipY("clipY", Float) = 0
	  _XRayColorR("XRayColorR", Float) = 0
	  _XRayColorG("XRayColorG", Float) = 0
	  _XRayColorB("XRayColorB", Float) = 0
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
      float _IENB;
      float _Opacity;
      float _ColorFactor;
		  float _ClipX;
		  float _ClipY;

		  float _XRayColorR;
		  float _XRayColorG;
		  float _XRayColorB;
 
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
        float3 backObj = mul(unity_WorldToObject, back) * 2;
        float3 frontObj = mul(unity_WorldToObject, front) * 2;
           
        // ray throush the volume
        float3 dir = backObj.xyz - frontObj.xyz;
        float length = distance(frontObj, backObj);
        float step = _Step * _StepFactor;
        float3 stepDir = step * dir;

        // walk along the ray sampling the volume
        float3 pos = frontObj;
        float3 sampledColor = float3(0, 0, 0), 
               color = float3(_XRayColorR, _XRayColorG, _XRayColorB);
          
        for (int i = 0; i < 255; i++)
        {
          if (distance(pos, backObj) < step * 0.1) break; // check when reach the back  
			      
          if (pos.x < _ClipX || pos.y < _ClipY)
			    {
				    pos += stepDir;
				    continue;
			    }
          
    	    sampledColor = tex3Dlod(_Volume, float4(pos, 0)).rrr;
          if (sampledColor.r < _Opacity)
            sampledColor = 0;
          color += sampledColor * _ColorFactor;
          pos += stepDir;
        }
        // temp color
        return float4 (color, color.r);
      }
      ENDCG
    }
  }
}