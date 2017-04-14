using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Volume : MonoBehaviour
{
    public Texture3D texture;
    private Material textureMaterial;
    private string texFileName = "Default3DTexture";
    //private string texFileName = "Sphere3D2";

    private BackFrontTextureCreator texCreator;
  
    public string VolumeTextureData { get; set; }
                                                
    // Use this for initialization
    void Start()
    {

        var renderer = GetComponent<MeshRenderer>();
        textureMaterial = renderer != null ? renderer.sharedMaterial : null; 

        LoadCustomTextureData();

        // get texture creator from camera
        texCreator = Camera.main.GetComponent<BackFrontTextureCreator>();
    }

    // Update is called once per frame
    void OnWillRenderObject()
    {
        if (textureMaterial != null && texCreator != null)
        {
            textureMaterial.SetTexture("_BackTex", texCreator.BackTexture);
            textureMaterial.SetTexture("_FrontTex", texCreator.FrontTexture);
            textureMaterial.SetTexture("_Volume", texture);
        }
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

        colors = new Color32[dimX * dimY * dimZ];
        for (int lineNumber = 0; lineNumber < lines.Length - 1 && lineNumber < colors.Length; ++lineNumber)
        {
            colorData = lines[lineNumber + 1].Split(' ');
            //colors[(lineNumber % dimX) + ((lineNumber / dimX) % dimY) + (lineNumber / (dimX * dimY))] =
            colors[lineNumber] =
            new Color32(byte.Parse(colorData[0]), byte.Parse(colorData[1]), byte.Parse(colorData[2]), 255);
        }

        texture = new Texture3D(dimX, dimY, dimZ, TextureFormat.RGBA32, false);
        texture.SetPixels32(colors);
        texture.Apply();
    }
}
