Shader "CGA/ComboIsosurfaceVolumeRendering"
{
  Properties
  {
    _Volume("VolumeTex", 3D) = "" {}
    _BackTex("BackFaceTex", 2D) = "white" {}
    _FronTex("FrontFaceTex", 2D) = "white" {}
    _Step("Step size", Float) = 0.05
    _StepFactor("Step factor", Range(0.5, 2.0)) = 1.0
    _IsosurfaceThreshold("Isosurface threshold", Float) = 0.05
    _ClipX("clipX", Float) = 0
    _ClipY("clipY", Float) = 0
    _ClipZ("clipZ", Float) = 0
    _AmbientR("Ambient color red component", Float) = 0
    _AmbientG("Ambient color green component", Float) = 0
    _AmbientB("Ambient color blue component", Float) = 0
    _DiffuseR("Diffuse color red component", Float) = 0
    _DiffuseG("Diffuse color green component", Float) = 0.5
    _DiffuseB("Diffuse color blue component", Float) = 0.5
    _SpecularR("Specular color red component", Float) = 1
    _SpecularG("Specular color green component", Float) = 1
    _SpecularB("Specular color blue component", Float) = 1
    _Shininess("Shininess", Float) = 32
    
    _Opacity("Opacity border", Float) = 0.0
    _TransferFunctionId("Transfer function id", Int) = 0
    _GradientBorder("Gradient border", Range(0.0, 1.0)) = 0.05
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
      #include "UnityLightingCommon.cginc"

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
      float _IsosurfaceThreshold;
      float _ClipX;
      float _ClipY;
      float _ClipZ;
      float _AmbientR, _AmbientG, _AmbientB;
      float _DiffuseR, _DiffuseG, _DiffuseB;
      float _SpecularR, _SpecularG, _SpecularB;
      float _Shininess;

      float _Opacity;
      int _TransferFunctionId;
      float _GradientBorder;

      // Used for any model, based on gradient
      // densityC - current point
      // densityL - prev point
      // densityR - next point
      float3 transferFunctionColorCommon(float densityC, float densityL, float densityR)
      {
        float delta1 = densityC - densityL, delta2 = densityR - densityC,
          diff = 0;

        if (delta1 >= 0 && delta2 >= 0) // we are in the bottom
        {
          diff = abs(delta1 - delta2);
          return float3(127.0 / 256.0, 127.0 / 256.0, min(diff, 1.0) + _GradientBorder);
        }
        else if (delta1 >= 0 && delta2 < 0) // we are descending
        {
          diff = abs(delta1 + delta2);
          return float3(127.0 / 256.0, max(min(diff, 1.0) - _GradientBorder, 0.0), 127.0 / 256.0);
        }
        else if (delta1 < 0 && delta2 >= 0) // we are ascending
        {
          diff = abs(delta1 + delta2);
          return float3(127.0 / 256.0, min(diff, 1.0) + _GradientBorder / 256.0, 127.0 / 256.0);
        }
        else //if (delta1 < 0 && delta2 < 0) // we are atthe top
        {
          diff = abs(delta1 - delta2);
          return float3(min(diff, 1.0) + _GradientBorder, 127.0 / 256.0, 127.0 / 256.0);
        }
      }

      // Used for 'Orange' model
      float3 transferFunctionColorOrange(float density)
      {
        if (density > 0.7 && density <= 1.00) // 0 - 0.08 - 'Skin'
          return float3(255.0 / 256.0, 69.0 / 256.0, 0.0 / 256.0);
        else //if (density > 0.9 && density <= 1.0) // 0.9 - 1.0 - 'Innards'
          return float3(255.0 / 256.0, 140.0 / 256.0, 0.0 / 256.0);
      }

      // Used for 'Baby' model
      float3 transferFunctionColorBaby(float density)
      {
        if (density >= 0 && density <= 0.1) // 0 - 0.08 - 'Skin'
          return float3(225.0 / 256.0, 223.0 / 256.0, 196.0 / 256.0);
        else if (density > 0.1 && density <= 0.45) // 0.08 - 0.45 - 'Brain'
          return float3(240.0 / 256.0, 200.0 / 256.0, 201.0 / 256.0);
        else if (density > 0.45 && density <= 0.9) // 0.45 - 0.9 - 'Bone'
          return float3(227.0 / 256.0, 218.0 / 256.0, 201.0 / 256.0);
        else if (density > 0.9 && density <= 0.97) // 0.9 - 1.0 - 'Other'
          return float3(255.0 / 256.0, 255.0 / 256.0, 255.0 / 256.0);
        else //if (density > 0.9 && density <= 0.97) // 0.9 - 1.0 - 'Metal'
          return float3(176.0 / 256.0, 196.0 / 256.0, 222.0 / 256.0);
      }

      v2f vert(appdata v)
      {
        v2f o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uvw = v.uvw;
        o.screenSpacePos = o.vertex;
        return o;
      }

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
        float step = _Step * _StepFactor,
          stepDirLen = step * length;
        float3 stepDir = step * dir,
          stepDirNorm = normalize(stepDir);

        // walk along the ray sampling the volume
        float3 pos = frontObj, worldPos;
        float4 color = float4(0, 0, 0, 0);
        float4 ambient = float4(_AmbientR, _AmbientG, _AmbientB, 1),
          diffuse = float4(_DiffuseR, _DiffuseG, _DiffuseB, 1),
          specular = float4(_SpecularR, _SpecularG, _SpecularB, 1);
        float3 posColor = float3(0, 0, 0);
        float3 normal, reflectDir, viewDir, lightDir;
        float3 stepDirNormX, stepDirNormY, stepDirX, stepDirY;
        float dx, dy, dz;
        
        float alpha = 0,
          densityPrev = 0,
          density = tex3Dlod(_Volume, float4(pos, 0)).r,
          densityNext = density,
          compositeTransparency = 1;
        float3 sampledColor = float3(0, 0, 0),          
          compositeColor = 0;
        
        int isoFinished = 0;

        for (int i = 0; i < 350; i++)
        {
          if (distance(pos, backObj) < 0.5 * step || compositeTransparency < _Opacity) break; // check when reach the back
          if (pos.x < _ClipX || pos.y < _ClipY || pos.z < _ClipZ)
          {
            pos += stepDir;
            continue;
          }
          posColor = tex3Dlod(_Volume, float4(pos, 0)).rrr;
          if (distance(posColor, float3(0, 0, 0)) > _IsosurfaceThreshold)
          {
            // gray 2D texture on cliped slice
            if (_ClipX > 0 && abs(pos.x - _ClipX) < 0.01f)
            {
              color = float4(posColor, posColor.r);
              isoFinished = 1;
              break;
            }
            if (_ClipY > 0 && abs(pos.y - _ClipY) < 0.01f)
            {
              color = float4(posColor, posColor.r);
              isoFinished = 1;
              break;
            }
            if (_ClipZ > 0 && abs(pos.z - _ClipZ) < 0.01f)
            {
              color = float4(posColor, posColor.r);
              isoFinished = 1;
              break;
            }

            // count normal
            stepDirNormX = float3(stepDirNorm.x, -stepDirNorm.z, stepDirNorm.y); // rotate stepDirNorm 90 degrees around X axis        
            stepDirNormY = normalize(cross(stepDirNorm, stepDirNormX)); // rotate stepDir 90 degrees around Y axis
            stepDirX = stepDirNormX * stepDirLen;
            stepDirY = stepDirNormY * stepDirLen;
            // density difference between (pos - stepDir) and (pos + stepDir) in 3 directions
            dx = tex3Dlod(_Volume, float4(pos - stepDirX, 0)).r - tex3Dlod(_Volume, float4(pos + stepDirX, 0)).r;
            dy = tex3Dlod(_Volume, float4(pos - stepDirY, 0)).r - tex3Dlod(_Volume, float4(pos + stepDirY, 0)).r;
            dz = tex3Dlod(_Volume, float4(pos - stepDir, 0)).r - tex3Dlod(_Volume, float4(pos + stepDir, 0)).r;
            normal = dx * stepDirX + dy * stepDirY + dz * stepDir;
            // normal to world coordinates
            normal = mul(unity_ObjectToWorld, normal);
            normal = normalize(normal);
            // phong lighting model				
            worldPos = mul(unity_ObjectToWorld, pos);
            viewDir = normalize(_WorldSpaceCameraPos - worldPos);
            lightDir = normalize(_WorldSpaceLightPos0.xyz);
            reflectDir = reflect(-lightDir, normal);
            reflectDir = normalize(reflectDir);
            color = ambient + diffuse * max(0.0, dot(normal, lightDir)) + specular * pow(max(0.0, dot(reflectDir, viewDir)), _Shininess);

            isoFinished = 1;
            break;
          }
          else 
          {                                             
            densityPrev = density;
            density = densityNext;
            densityNext = tex3Dlod(_Volume, float4(pos + stepDir, 0)).r;
            if (_TransferFunctionId == 0)
            {
              sampledColor = transferFunctionColorBaby(density);
            }
            else if (_TransferFunctionId == 1)
            {
              sampledColor = transferFunctionColorOrange(density);
            }
            else
            {
              sampledColor = transferFunctionColorCommon(densityPrev, density, densityNext);
            }

            alpha = density;

            compositeColor += alpha * sampledColor * compositeTransparency;
            compositeTransparency *= (1 - alpha);                                                      
          }
          pos += stepDir;
        }

        if (isoFinished == 0)
          color = float4(compositeColor, 1 - compositeTransparency);

        return color;
      }
    
      ENDCG
    }
  }
}