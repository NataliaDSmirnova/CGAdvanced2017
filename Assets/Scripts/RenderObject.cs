using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderObject : MonoBehaviour {

    public GameObject renderObject;

    private RenderTexture renderTexture;
    private SpriteRenderer spriteRenderer;

    void Start () {
        spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
    }
	
	void Update () {
        // texture
        renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        Graphics.SetRenderTarget(renderTexture);

        spriteRenderer.material.SetPass(0);

        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        Graphics.DrawMeshNow(objectMesh, objectMesh.vertices[0], Quaternion.identity);

        spriteRenderer.material.mainTexture = renderTexture;

        Graphics.SetRenderTarget(null);
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
