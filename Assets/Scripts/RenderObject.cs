using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderObject : MonoBehaviour {

    public GameObject renderObject;
<<<<<<< HEAD

    private RenderTexture renderTexture;
    private SpriteRenderer spriteRenderer;
    private int xSizeOfSprite;
    private int ySizeOfSprite;

    void Start () {
        spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
        xSizeOfSprite = (int) spriteRenderer.sprite.border.x;
        ySizeOfSprite = (int) spriteRenderer.sprite.border.y;
=======
    private RawImage image;
    private RenderTexture rt;
    
    void Start () {
      image = GetComponent<RawImage>();
      rt = RenderTexture.GetTemporary(Screen.width, Screen.height);
>>>>>>> origin/master
    }
	
	  void Update () {
       
        Graphics.SetRenderTarget(rt);

        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
<<<<<<< HEAD
        Graphics.DrawMeshNow(objectMesh, renderObject.transform.position, Quaternion.identity);
=======
        var renderer = renderObject.GetComponent<MeshRenderer>();
        renderer.material.SetPass(0);

        Graphics.DrawMeshNow(objectMesh, objectMesh.vertices[0], Quaternion.identity);
>>>>>>> origin/master

        image.texture = rt;

        Graphics.SetRenderTarget(null);
    }
}
