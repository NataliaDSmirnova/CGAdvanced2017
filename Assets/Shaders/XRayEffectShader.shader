// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "CGA/XRayEffectShader"
{
	  Properties
	  {
		    _Tex3D ("3D Texture", 3D) = "white" {}
        _LowBound ("Low Bound", Vector) = (0, 1, 0, 1)
        _HighBound("High Bound", Vector) = (1, 1, 1, 1)
        _Params("Params", Vector) = (1, 0.1, 1, 1)
	  }

	  SubShader
	  {
		    Tags { "RenderType" = "Opaque" }
		    LOD 100

		    Pass
		    {
			      CGPROGRAM
			      #pragma vertex vert
			      #pragma fragment frag			      
			
            #pragma enable_d3d11_debug_symbols

			      #include "UnityCG.cginc"

			      struct appdata
			      {
				        float4 vertex : POSITION;
				        float3 uvw : TEXCOORD0;
			      };

			      struct v2f
			      {
				        float3 uvw : TEXCOORD0;				       
				        float4 vertex : SV_POSITION;
			      };

			      sampler3D _Tex3D;

            struct Ray
            {
              float3 origin;
              float3 dir;
              float3 invDir;
              int3 sign;
            };

            float4 _LowBound;
            float4 _HighBound;
            float4 _Params;   // (Diameter_of_cube, diameter_of_voxel, free, free)


            float4x4 inverse(float4x4 input)
            {
                #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))                

                float4x4 cofactors = float4x4(
                                      minor(_22_23_24, _32_33_34, _42_43_44),
                                      -minor(_21_23_24, _31_33_34, _41_43_44),
                                      minor(_21_22_24, _31_32_34, _41_42_44),
                                      -minor(_21_22_23, _31_32_33, _41_42_43),

                                      -minor(_12_13_14, _32_33_34, _42_43_44),
                                      minor(_11_13_14, _31_33_34, _41_43_44),
                                      -minor(_11_12_14, _31_32_34, _41_42_44),
                                      minor(_11_12_13, _31_32_33, _41_42_43),

                                      minor(_12_13_14, _22_23_24, _42_43_44),
                                      -minor(_11_13_14, _21_23_24, _41_43_44),
                                      minor(_11_12_14, _21_22_24, _41_42_44),
                                      -minor(_11_12_13, _21_22_23, _41_42_43),

                                      -minor(_12_13_14, _22_23_24, _32_33_34),
                                      minor(_11_13_14, _21_23_24, _31_33_34),
                                      -minor(_11_12_14, _21_22_24, _31_32_34),
                                      minor(_11_12_13, _21_22_23, _31_32_33));
                #undef minor
                return transpose(cofactors) / determinant(input);
            }


            bool isIntersected(Ray ray, float3 bounds[2], float start, float end)
            {
                float tmin, tmax, tymin, tymax, tzmin, tzmax;

                tmin = (bounds[ray.sign[0]][0] - ray.origin[0]) * ray.invDir[0];
                tmax = (bounds[1 - ray.sign[0]][0] - ray.origin[0]) * ray.invDir[0];
                tymin = (bounds[ray.sign[1]][1] - ray.origin[1]) * ray.invDir[1];
                tymax = (bounds[1 - ray.sign[1]][1] - ray.origin[1]) * ray.invDir[1];
              
                if (tmin > tymax || tymin > tmax)
                    return false;
                if (tymin > tmin) 
                    tmin = tymin;
                if (tymax < tmax)
                    tmax = tymax;
              
                tzmin = (bounds[ray.sign[2]][2] - ray.origin[2]) * ray.invDir[2];
                tzmax = (bounds[1 - ray.sign[2]][2] - ray.origin[2]) * ray.invDir[2];
              
                if (tmin > tzmax || tzmin > tmax)
                    return false;
                if (tzmin > tmin) 
                    tmin = tzmin;
                if (tzmax < tmax)
                    tmax = tzmax;
              
                return (tmin < end) && (tmax > start);
            }


            float4 camToWorld(float4 vec)
            {              
              float4 dirVec = float4(2.0f * vec[0] / _ScreenParams[0] - 1.0f,
                                     -2.0f * vec[1] / _ScreenParams[1] + 1.0f,
                                     0.0f, 0.0f);
              return mul(inverse(mul(UNITY_MATRIX_P, UNITY_MATRIX_V)), dirVec);
            }


            float3 getClosestPos(float3 pos, float3 normDir)
            {
              float3 center = (_HighBound.xyz + _LowBound) / 2.0f, newPos = 0;
              float t = 0;

              if (length(pos - center) < _Params[0])
              {
                  return pos;
              }
              else
              {
                  //newPos = center - _Params[0] * normDir;
                  //t = (dot(normDir, newPos) - dot(normDir, pos)) / (dot(normDir, normDir));
                  return pos;// +t * normDir;
              }

              /*if (abs(pos.x - _LowBound.x) < abs(pos.x - _HighBound.x))
                newPos.x = _LowBound.x;
              if (abs(pos.y - _LowBound.y) < abs(pos.y - _HighBound.y))
                newPos.y = _LowBound.y;
              if (abs(pos.z - _LowBound.z) < abs(pos.z - _HighBound.z))
                newPos.z = _LowBound.z;*/              
            }


			      v2f vert (appdata v)
			      {
				        v2f res;
				        res.vertex = UnityObjectToClipPos(v.vertex);
				        res.uvw = v.uvw;				        
				        return res;
			      }
           

            float4 frag(v2f input) : SV_Target
            {     
                bool isInCube = false;
                float x, y, z, newZ;
                int n = 0, m = (int)(_Params[0] / _Params[1]), e = 0;
                fixed4 col = 0;
                Ray ray;
                float3 bounds[2], currPos = 0, normDir = 0;
                float start = 0, step = _Params[1];                

                bounds[0] = _LowBound.xyz;
                bounds[1] = _HighBound.xyz;                                
                normDir = normalize(WorldSpaceViewDir(camToWorld(input.vertex)));
                
                ray.origin = getClosestPos(_WorldSpaceCameraPos, normDir);
                ray.dir = ray.origin + normDir * _Params[0];
                ray.invDir = float3(1.0f / ray.dir[0], 1.0f / ray.dir[1], 1.0f / ray.dir[2]);
                ray.sign = int3(ray.invDir[0] < 0.0f, ray.invDir[1] < 0.0f, ray.invDir[2] < 0.0f);                
                currPos = ray.origin;
                //col = float4(ray.dir, 1.0f);
                
                while (n < 245)
                {
                    n += 1;
                    if (isIntersected(ray, bounds, start, start + step / 2) == true)
                    {
                        isInCube = true;
                        currPos = ray.origin + (start - 0) * normDir;/// _Params[1];                        
                        /*x = x < 0 && x > 1 ? 1 : x;
                        y = y < 0 && y > 1 ? 1 : y;
                        z = z < 0 && z > 1 ? 1 : z;*/
                        //col += float4(mul(unity_WorldToObject, currPos).xyz, 0);
                        col += tex3D(_Tex3D, mul(unity_WorldToObject, currPos));
                        e += 1;                        
                    } 
                    else if (isInCube/* || start > 1.0f*/)
                    {
                        /*if (e > m)
                            col = float4(1, 0, 0, 0);
                        else if (e == m)
                            col = float4(0, 1, 0, 0);
                        else if (e >= 1)
                            col = float4(0, 0, 1, 0);
                        else
                            col = float4(0.5, 0.5, 0.5, 0);*/    
                        //col = col / e;
                        break;
                    }
                    //else 
                    //{
                     /*   ray.origin += normDir * step;
                        ray.dir += normDir * step;
                        ray.invDir = float3(1.0f / ray.dir[0], 1.0f / ray.dir[1], 1.0f / ray.dir[2]);
                        ray.sign = int3(ray.invDir[0] < 0.0f, ray.invDir[1] < 0.0f, ray.invDir[2] < 0.0f);                     
                        continue;*/
                    //}/**/

                    start += step / 2;                    
                }                      
				        return col;
			      }
			      ENDCG
		    }
	  }
}
