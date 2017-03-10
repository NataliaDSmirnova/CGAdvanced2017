using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderObject : MonoBehaviour {

    public GameObject renderObject;
    private RawImage image;
    private RenderTexture rt;
    
    void Start () {
      image = GetComponent<RawImage>();
      rt = RenderTexture.GetTemporary(Screen.width, Screen.height);
    }
	
	  void Update () {
       
        Graphics.SetRenderTarget(rt);

        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        var renderer = renderObject.GetComponent<MeshRenderer>();
        renderer.material.SetPass(0);

        Graphics.DrawMeshNow(objectMesh, objectMesh.vertices[0], Quaternion.identity);

        image.texture = rt;

        Graphics.SetRenderTarget(null);
    }
}
