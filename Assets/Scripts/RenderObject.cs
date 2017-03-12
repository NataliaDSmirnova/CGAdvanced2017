using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderObject : MonoBehaviour {

    public GameObject renderObject;

    private RenderTexture renderTexture;
    private SpriteRenderer spriteRenderer;
    private int xSizeOfSprite;
    private int ySizeOfSprite;

    void Start () {
        spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
        xSizeOfSprite = (int) spriteRenderer.sprite.border.x;
        ySizeOfSprite = (int) spriteRenderer.sprite.border.y;
    }
	
	void Update () {
        // texture
        renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        Graphics.SetRenderTarget(renderTexture);

        spriteRenderer.material.SetPass(0);

        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        Graphics.DrawMeshNow(objectMesh, renderObject.transform.position, Quaternion.identity);

        spriteRenderer.material.mainTexture = renderTexture;

        Graphics.SetRenderTarget(null);
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
