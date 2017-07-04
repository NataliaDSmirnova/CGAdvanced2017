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
    _TransferFunctionId("Transfer function id", Int) = 0
    _GradientBorder("Gradient border", Range(0.0, 1.0)) = 0.05
    _ClipX("clipX", Float) = 0
    _ClipY("clipY", Float) = 0
	  _ClipZ("clipZ", Float) = 0
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
	    float _ClipZ;

      int _TransferFunctionId;
      float _GradientBorder;

      float3 fitColorInRange(float r, float g, float b)
      {
        return float3(r / 255.0, g / 255.0, b / 255.0);
      }

      // Used for any model, based on gradient
      // densityC - current point
      // densityL - prev point
      // densityR - next point
      float4 transferFunctionColorCommon(float densityC, float densityL, float densityR)
      {
        float delta1 = densityC - densityL, delta2 = densityR - densityC,
          diff = 0;

        if (delta1 >= 0 && delta2 >= 0) // we are in the bottom
        {
          diff = abs(delta1 - delta2);
          return float4(fitColorInRange(127, 127, min(diff, 1.0) + _GradientBorder), 1.0);
        }
        else if (delta1 >= 0 && delta2 < 0) // we are descending
        {
          diff = abs(delta1 + delta2);
          return float4(fitColorInRange(127, max(min(diff, 1.0) - _GradientBorder, 0.0), 127), 1.0);
        }
        else if (delta1 < 0 && delta2 >= 0) // we are ascending
        {
          diff = abs(delta1 + delta2);
          return float4(fitColorInRange(127, min(diff, 1.0) + _GradientBorder, 127), 1.0);
        }
        else //if (delta1 < 0 && delta2 < 0) // we are atthe top
        {
          diff = abs(delta1 - delta2);
          return float4(fitColorInRange(min(diff, 1.0) + _GradientBorder, 127, 127), 1.0);
        }
      }

      // Used for 'Orange' model
      float4 transferFunctionColorOrange(float density)
      {
        if (density > 0.7 && density <= 1.00) // 0 - 0.08 - 'Skin'
          return float4(fitColorInRange(255, 69, 0), 1.0);
        else //if (density > 0.9 && density <= 1.0) // 0.9 - 1.0 - 'Innards'
          return float4(fitColorInRange(255, 140, 0), 1.0);
      }

      // Used for 'Baby' model
      float4 transferFunctionColorBaby(float density)
      {
        if (density >= 0 && density <= 0.59) // 'Stuff'
          return float4(fitColorInRange(0, 0, 0), 0.0);
        else if (density > 0.59 && density <= 0.68) // 'Skin'
          return float4(fitColorInRange(225, 223, 196), 0.5);
        else if (density > 0.68 && density <= 0.76) // 'Muscle'/Brain
          return float4(fitColorInRange(240, 200, 201), 0.4);
        else if (density > 0.76 && density <= 0.91) // 'Metal'
          return float4(fitColorInRange(176, 196, 222), 0.7);
        else // 'Bone'
          return float4(fitColorInRange(250, 250, 250), 0.9);
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
		    float3 backObj = tex2D(_BackTex, tc.xy).xyz;
		    float3 frontObj = tex2D(_FrontTex, tc.xy).xyz;

        // ray throush the volume
		    float3 dir = backObj.xyz - frontObj.xyz;
		    float length = distance(frontObj, backObj);
		    float step = _Step * _StepFactor;
		    float3 stepDir = step * dir;

        // walk along the ray sampling the volume
        float3 pos = frontObj;
        float alpha = 0, 
          densityPrev = 0,
          density = tex3D(_Volume, pos.xyz).r,
          densityNext = density,
          compositeTransparency = 1;
        float3 color = 0,
          compositeColor = 0;          
        float4 sampledColor = float4(0, 0, 0, 1);

        for (int i = 0; i < 350; i++)
        {
          if (distance(pos, backObj) < step * 0.5 || compositeTransparency < _Opacity) 
            break; // check when reach the back  

          if (pos.x < _ClipX || pos.y < _ClipY || pos.z < _ClipZ)
          {
            pos += stepDir;
            continue;
          }

          densityPrev = density;
          density = densityNext;
          densityNext = tex3Dlod(_Volume, float4(pos + stepDir, 0)).r;
          if (_TransferFunctionId == 0)
          {
            sampledColor = transferFunctionColorBaby(density * sqrt(3));
          } 
          else if (_TransferFunctionId == 1)
          {
            sampledColor = transferFunctionColorOrange(density * sqrt(3));
          }
          else
          {
            sampledColor = transferFunctionColorCommon(densityPrev * sqrt(3), density * sqrt(3), densityNext * sqrt(3));
          }
          
          alpha = sampledColor.w * density;

          compositeColor += alpha * sampledColor.xyz * compositeTransparency;
          compositeTransparency *= (1 - alpha);
          
          pos += stepDir;
        }
        color = compositeColor;
        // temp color
        return float4(color, 1 - compositeTransparency);
      }
      ENDCG
    }
  }
}
