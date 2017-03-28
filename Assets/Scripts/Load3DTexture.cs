using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load3DTexture : MonoBehaviour
{

    public Texture3D texture;
    public Material textureMaterial;
    private string texFileName = "Default3DTexture";
    private string volumeShaderName = "XRayEffectShader";

    public string VolumeTextureData { get; set; }
                                                
    // Use this for initialization
    void Start()
    {
      //    VolumeTextureData = "_MainTex";
      var renderer = GetComponent<MeshRenderer>();
      textureMaterial = renderer != null ? renderer.sharedMaterial : null; 
      LoadCustomTextureData();
      LoadVolumeTexture();        
    }

    // Update is called once per frame
    void Update()
    {
      if (textureMaterial != null)
      {
     //   textureMaterial.mainTexture = texture;

        Vector3 cubePos = gameObject.transform.localPosition,
                cubeScale = gameObject.transform.localScale;

        textureMaterial.SetTexture("_MainTex", texture);
        textureMaterial.SetVector("_LowBound",
          new Vector4(cubePos.x - 0.5f * cubeScale.x, cubePos.y - 0.5f * cubeScale.y, cubePos.z - 0.5f * cubeScale.z, 1.0f / (float)texture.depth ));
        textureMaterial.SetVector("_HighBound",
          new Vector4( cubePos.x + 0.5f * cubeScale.x, cubePos.y + 0.5f * cubeScale.y, cubePos.z + 0.5f * cubeScale.z, (float)texture.depth ));
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
}
