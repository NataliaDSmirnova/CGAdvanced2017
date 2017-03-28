Shader "CGA/XRayEffectShader"
{
	  Properties
	  {
		    _Tex3D ("3D Texture", 3D) = "white" {}
        _LowBound ("Low Bound", Vector) = (0, 1, 0, 1)
        _HighBound("High Bound", Vector) = (1, 1, 1, 1)
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
			
          //  #pragma enable_d3d11_debug_symbols

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
			    //  float4 _MainTex_ST;            
            //float4 _MainTex_TexelSize;

            struct Ray
            {
              float3 origin;
              float3 dir;
              float3 invDir;
              int3 sign;
            };

            float4 _LowBound;
            float4 _HighBound;


            float4x4 inverse(float4x4 input)
            {
                #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
                //determinant(float3x3(input._22_23_23, input._32_33_34, input._42_43_44))

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


			      v2f vert (appdata v)
			      {
				        v2f res;
				        res.vertex = UnityObjectToClipPos(v.vertex);
				        res.uvw = v.uvw;				        
				        return res;
			      }
			

            float4 frag(v2f input) : SV_Target
            {
                /*int a = int(_MainTex_TexelSize[2]),
                    b = int(_MainTex_TexelSize[3]),
                    c = int(_HighBound[3]);*/
                int x, y, z, n = 0,
                  width = 2,//int(_MainTex_TexelSize[2]), 
                  height = 2,//int(_MainTex_TexelSize[3]),
                  depth = 1;//int(_LowBound[3]);
                fixed4 col = 0;
                Ray ray;
                float3 bounds[2],
               /*   delta = { (_HighBound[0] - _LowBound[0]) * _MainTex_TexelSize[0],
                            (_HighBound[1] - _LowBound[1]) * _MainTex_TexelSize[1],
                            (_HighBound[2] - _LowBound[2]) * _LowBound[3] };*/
                   delta = { (_HighBound[0] - _LowBound[0]) * _LowBound[3],
                  (_HighBound[1] - _LowBound[1]) * _LowBound[3],
                  (_HighBound[2] - _LowBound[2]) *  _LowBound[3] };



                ray.origin = _WorldSpaceCameraPos;                
                ray.dir = normalize((mul(inverse(UNITY_MATRIX_P * UNITY_MATRIX_V),
                                    float4(2.0f * input.vertex[0] * (_ScreenParams[2] - 1.0f) - 1.0f,
                                          -2.0f * input.vertex[1] * (_ScreenParams[3] - 1.0f) + 1.0f,
                                          0.0f, 1.0f))).xyz);
                ray.invDir = float3(1.0f / ray.dir[0], 1.0f / ray.dir[1], 1.0f / ray.dir[2]);
                ray.sign = int3(ray.invDir[0] < 0.0f, ray.invDir[1] < 0.0f, ray.invDir[2] < 0.0f);                

                for (x = 0; x < width; x++)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (z = 0; z < depth; z++)
                        {
                            bounds[0] = float3(_LowBound[0] + delta.x * x, _LowBound[1] + delta.y * y, _LowBound[2] + delta.z * z);
                            bounds[1] = float3(_LowBound[0] + delta.x * (x + 1), _LowBound[1] + delta.y * (y + 1), _LowBound[2] + delta.z * (z + 1));

                            if (isIntersected(ray, bounds, 0.0f, 1.0f) == true) 
                            {
                                col += tex3D(_Tex3D, float3(x, y, z));
                                n += 1;
                            }
                        }
                    }
                } 
                //return _LowBound;
             //   return float4(1, 0, 0, 1);
				        return col / (n + 1);
			      }
			      ENDCG
		    }
	  }
}
