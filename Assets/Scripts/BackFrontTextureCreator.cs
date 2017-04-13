using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BackFrontTextureCreator : MonoBehaviour
{
    // input objects
    public GameObject renderObject;
    public Material frontMaterial;
    public Material backMaterial;
    public RenderTexture FrontTexture { get { return frontTexture; } }
    public RenderTexture BackTexture { get { return backTexture; } }

    // private objects
    private RenderTexture frontTexture;
    private RenderTexture backTexture;
    private CommandBuffer buffer;
    
    void Start()
    {
        frontTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 1, RenderTextureFormat.ARGBFloat);
        backTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 1, RenderTextureFormat.ARGBFloat);

        // get mesh from input object
        buffer = new CommandBuffer();
        GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, buffer);
    }

    void OnPreRender()
    {
        // get mesh from input object
        var mesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            Debug.Log("Can't get mesh from input object");
            return;
        }

        // form command buffer 
        buffer.Clear();
        if (frontTexture  != null && frontMaterial)
        {
            buffer.SetRenderTarget(frontTexture);
            buffer.ClearRenderTarget(true, true, new Color(0f, 0f, 0f));
            buffer.DrawMesh(mesh, renderObject.transform.localToWorldMatrix, frontMaterial);
        }
        if (backTexture != null && backMaterial)
        {
            buffer.SetRenderTarget(backTexture);
            buffer.ClearRenderTarget(true, true, new Color(0f, 0f, 0f));
            buffer.DrawMesh(mesh, renderObject.transform.localToWorldMatrix, backMaterial);
        }
    }
}