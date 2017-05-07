using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public struct Ray
{
  public Vector3 origin;
  public Vector3 dir;
  public Vector3 invDir;
  public int[] sign;
};

public class Load3DTexture : MonoBehaviour
{

    public Texture3D texture;
    public Material textureMaterial;
    private string texFileName = "Default3DTexture";
    //private string volumeShaderName = "XRayEffectShader";

    public Vector4 _LowBound;
    public Vector4 _HighBound;
    public Vector4 _Params;
    public Vector4 _ScreenParams;
    public Camera cam;
    public Matrix4x4 P, V;

  public string VolumeTextureData { get; set; }
                                                
    // Use this for initialization
    void Start()
    {
      //    VolumeTextureData = "_MainTex";
      var renderer = GetComponent<MeshRenderer>();
      cam = Camera.main;
      textureMaterial = renderer != null ? renderer.sharedMaterial : null; 
      LoadCustomTextureData();
      LoadVolumeTexture();

        /*
        Matrix4x4 l2w1 = gameObject.transform.localToWorldMatrix,
          w2l = l2w1.inverse, l2w2;
        Vector4 loc1 = new Vector4(0.1f, 0.1f, 0.1f),
          loc2 = new Vector4(0.5f, 0.5f, 0.5f),
          loc3 = new Vector4(-0.2f, 0.2f, -0.5f);

        Vector4 r1 = l2w1.MultiplyPoint3x4(loc1),
          r2 = l2w1.MultiplyPoint3x4(loc2),
          r3 = l2w1.MultiplyPoint3x4(loc3);


        gameObject.transform.localScale = new Vector3(1.5f, 4, 7.5f);
        l2w2 = gameObject.transform.localToWorldMatrix;



        /*
        Vector4 p0 = cam.transform.localPosition;

        Vector4 a = new Vector4(0.5f, 0.5f, -0.5f, 1.0f);
        Vector4 p1 = gameObject.transform.TransformPoint(a);

        Vector4 c = cam.worldToCameraMatrix.MultiplyPoint(p1);
        Vector4 d = cam.projectionMatrix.MultiplyPoint(c);
        Vector4 dd = new Vector4((d.x + 1.0f) / 2.0f * cam.pixelWidth, (1.0f - d.y) / 2.0f * cam.pixelHeight);

        Vector4 cc = new Vector4((2.0f * dd.x) / cam.pixelWidth - 1.0f, (-2.0f * dd.y) / cam.pixelHeight + 1.0f, 0.0f);
        Vector4 p2 = (cam.projectionMatrix * cam.worldToCameraMatrix).inverse.MultiplyPoint(cc);

        Vector3 r = (p1 - p0).normalized,
          r2 = r * 0.3f;
        float t1 = r.magnitude, t2 = r2.magnitude, t3 = Vector3.Angle(r, r2);

        Vector3 input = new Vector3(800, 500, 0.8f);

        _ScreenParams = new Vector4(cam.pixelWidth, cam.pixelHeight, 1.0f / cam.pixelWidth + 1.0f, 1.0f / cam.pixelHeight + 1.0f);
        P = cam.projectionMatrix;
        V = cam.worldToCameraMatrix;
        Vector3 normDir = camToWorld(input);


        /*Ray ray = new Ray();
        Vector3[] bounds = new Vector3[2];
        Vector3 delta = Vector3.zero, currPos = Vector3.zero, normDir = Vector3.zero;
        float start = 0, step = _Params[1];

        Vector3 cubePos = gameObject.transform.localPosition,
                    cubeScale = gameObject.transform.localScale,
                    voxelSize = new Vector3(1.0f / texture.height, 1.0f / texture.width, 1.0f / texture.depth);
        _LowBound = new Vector4(cubePos.x - 0.5f * cubeScale.x, cubePos.y - 0.5f * cubeScale.y, cubePos.z - 0.5f * cubeScale.z, 1.0f / (float)texture.depth);
        _HighBound = new Vector4(cubePos.x + 0.5f * cubeScale.x, cubePos.y + 0.5f * cubeScale.y, cubePos.z + 0.5f * cubeScale.z, (float)texture.depth);
        bounds[0] = new Vector3(_LowBound.x, _LowBound.y, _LowBound.z);
        bounds[1] = new Vector3(_HighBound.x, _HighBound.y, _HighBound.z);

        ray.origin = new Vector3(0f, 0f, 0f);
        ray.dir = new Vector3(0.0f, 0.0f, 10f);
        ray.invDir = new Vector3(1.0f / ray.dir[0], 1.0f / ray.dir[1], 1.0f / ray.dir[2]);
        ray.sign = new int[3] { ray.invDir[0] < 0.0f ? 1 : 0, ray.invDir[1] < 0.0f ? 1 : 0, ray.invDir[2] < 0.0f ? 1 : 0 };


        bool res = isIntersected(ray, bounds, 0.6f, 0.8f);  */
    }

    // Update is called once per frame
    void Update()
    {
      if (textureMaterial != null)
      {
     //   textureMaterial.mainTexture = texture;

        Vector3 cubePos = gameObject.transform.localPosition,
                cubeScale = gameObject.transform.localScale,
                voxelSize = new Vector3(1.0f / texture.height, 1.0f / texture.width, 1.0f / texture.depth);
        _LowBound = new Vector4(cubePos.x - 0.5f * cubeScale.x, cubePos.y - 0.5f * cubeScale.y, cubePos.z - 0.5f * cubeScale.z, 1.0f / (float)texture.depth);
        _HighBound = new Vector4(cubePos.x + 0.5f * cubeScale.x, cubePos.y + 0.5f * cubeScale.y, cubePos.z + 0.5f * cubeScale.z, (float)texture.depth);
        _Params = new Vector4(cubeScale.magnitude, voxelSize.magnitude, 0.0f, 0.0f);
        _ScreenParams = new Vector4(cam.pixelWidth, cam.pixelHeight, 1.0f / cam.pixelWidth + 1.0f, 1.0f / cam.pixelHeight + 1.0f);
        P = cam.projectionMatrix;
        V = cam.worldToCameraMatrix;
        textureMaterial.SetTexture("_MainTex", texture);
        textureMaterial.SetVector("_LowBound", _LowBound);
        textureMaterial.SetVector("_HighBound", _HighBound);
        textureMaterial.SetVector("_Params", _Params);
        //tester();
      //textureMaterial.SetFloatArray("_LowBound", new float[4] { 1.0f, 1.0f, 0.0f, 1.0f });
      //textureMaterial.SetVector("_LowBound", new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
    }
    }


    // Assigning to material
    public void LoadVolumeTexture()
    {
       /* if (textureMaterial.GetTexture(VolumeTextureData) != null)
        {
            textureMaterial.mainTexture = texture;
        }
        else if (textureMaterial.shader.name.EndsWith(volumeShaderName))
        {
            textureMaterial.mainTexture = texture;

            Vector3 cubePos = gameObject.transform.localPosition,
                    cubeScale = gameObject.transform.localScale;

            textureMaterial.SetFloatArray("_LowBound", 
              new float[4] { cubePos.x - 0.5f * cubeScale.x, cubePos.y - 0.5f * cubeScale.y, cubePos.z - 0.5f * cubeScale.z, 1.0f / (float)texture.depth });
            textureMaterial.SetFloatArray("_HighBound",
              new float[4] { cubePos.x + 0.5f * cubeScale.x, cubePos.y + 0.5f * cubeScale.y, cubePos.z + 0.5f * cubeScale.z, (float)texture.depth });
        } 
        else 
        {
            Debug.Log("3D Texture isn't set (special shader needed)");
        }*/

  }


    /* Load custom texture. 
       Reminder: Custom texture format assumes:
                 * first line contains dimentions of texture separated with spaces (3 numbers)
                 * next lines represent color values of correspondent voxel in order: R G B (3 numbers)
                 * lines are stated in order: X row, XY plane (i.e (0, 0, 0) -> (Xdim, 0, 0),
                 *                                                 (0, 1, 0) -> (Xdim, 1, 0),
                 *                                                 ... 
                 *                                                 (Xdim, Ydim, 0) -> (0, 0, 1),
                 *                                                 ... )
     */
    private void LoadCustomTextureData()
    {
        TextAsset textAssetData = Resources.Load(texFileName) as TextAsset;
        string rawTextData = textAssetData.text;
        Color32[] colors;
        int dimX = 1, dimY = 1, dimZ = 1;
        
        string[] lines = rawTextData.Replace("\r\n", "\n").Split('\n'),
                 dimInfo = lines[0].Split(' '),
                 colorData = null;
        dimX = int.Parse(dimInfo[0]);
        dimY = int.Parse(dimInfo[1]);
        dimZ = int.Parse(dimInfo[2]);

        texture = new Texture3D(dimX, dimY, dimZ, TextureFormat.RGBA32, false);
        colors = new Color32[dimX * dimY * dimZ];

        for (int lineNumber = 0; lineNumber < lines.Length - 1; ++lineNumber)
        {
            colorData = lines[lineNumber + 1].Split(' ');

            colors[(lineNumber % dimX) + ((lineNumber / dimX) % dimY) + (lineNumber / (dimX * dimY))] =
                new Color32(byte.Parse(colorData[0]), byte.Parse(colorData[1]), byte.Parse(colorData[2]), 255);
        }
        texture.SetPixels32(colors);
        texture.Apply();
    }

  /*******************************************************************************************/

  public bool isIntersected(Ray ray, Vector3[] bounds, float start, float end)
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

    public Vector3 getClosestPos(Vector3 pos, Vector3 normDir)
    {
    Vector3 newPos = new Vector3(_LowBound.x, _LowBound.y, _LowBound.z);

      if (Math.Abs(pos.x - _LowBound.x) > Math.Abs(pos.x - _HighBound.x))
        newPos.x = _HighBound.x;
      if (Math.Abs(pos.y - _LowBound.y) > Math.Abs(pos.y - _HighBound.y))
        newPos.y = _HighBound.y;
      if (Math.Abs(pos.z - _LowBound.z) > Math.Abs(pos.z - _HighBound.z))
        newPos.z = _HighBound.z;

      return pos + (newPos - pos).magnitude * normDir;
    }

  Vector3 camToWorld(Vector4 vec)
  {
    Vector4 dirVec = new Vector4(2.0f * vec[0] * (_ScreenParams[2] - 1.0f) - 1.0f,
                                -2.0f * vec[1] * (_ScreenParams[3] - 1.0f) + 1.0f,
                                 0.0f, 0.0f);
    
    return ((P * V).inverse).MultiplyPoint(dirVec);
  }

  public void tester()
  {
    /*bool isInCube = false;
    float x, y, z;
    int n = 0;*/
    Vector4 col = Vector4.zero;
    Ray ray = new Ray();
    Vector3[] bounds = new Vector3[2];
    Vector3 delta = Vector3.zero, currPos = Vector3.zero, normDir = Vector3.zero;
    //float start = 0, step = _Params[1];
    float step = _Params[1];


        Vector3 input = new Vector3(563.50f, 353.50f, 0.18393f);
    Vector3 camPos = cam.transform.localPosition;

    bounds[0] = new Vector3(_LowBound.x, _LowBound.y, _LowBound.z);
    bounds[1] = new Vector3(_HighBound.x, _HighBound.y, _HighBound.z); 
    normDir = (camToWorld(input) - camPos).normalized;

    ray.origin = getClosestPos(camPos, normDir);
    ray.dir = ray.origin + normDir * _Params[0];

    Transform m = GameObject.Find("Sphere").GetComponent<MeshFilter>().transform;
    m.localPosition = ray.dir;


    /*ray.invDir = new Vector3(1.0f / ray.dir[0], 1.0f / ray.dir[1], 1.0f / ray.dir[2]);
    ray.sign = new int[3] { ray.invDir[0] < 0.0f ? 1 : 0, ray.invDir[1] < 0.0f ? 1 : 0, ray.invDir[2] < 0.0f ? 1 : 0 };


    while (n < 256)
    {
      n += 1;
      if (isIntersected(ray, bounds, start, start + step) == true)
      {
        isInCube = true;
        currPos = ray.origin + start * normDir;
        x = currPos.x - _LowBound.x;
        y = currPos.y - _LowBound.y;
        z = currPos.z - _LowBound.z;
        /*x = x < 0 && x > 1 ? 1 : x;
        y = y < 0 && y > 1 ? 1 : y;
        z = z < 0 && z > 1 ? 1 : z; */
        /*col += new Vector4(x, y, z, 1);
        //col += tex3D(_Tex3D, float3(x, y, z));
      }
      else if (isInCube || start > 1.0f) 
      {
        break;
      }
      else
      {
        ray.origin += normDir * step;
        ray.dir += normDir * step;
        ray.invDir = new Vector3(1.0f / ray.dir[0], 1.0f / ray.dir[1], 1.0f / ray.dir[2]);
        ray.sign = new int[3] { ray.invDir[0] < 0.0f ? 1 : 0, ray.invDir[1] < 0.0f ? 1 : 0, ray.invDir[2] < 0.0f ? 1 : 0 };
        continue;
      }
      start += step;    */    
  }
}
