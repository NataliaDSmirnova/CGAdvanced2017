using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderToTexture : MonoBehaviour {

    // input objects
    public GameObject renderObject;
    public Material CullBackMaterial;
    public Material CullFrontMaterial;

    // private objects
    private Material material;
    private RenderTexture renderTexture;

    void Start()
    {
        // create temprorary texture of screen size
        renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        renderTexture.format = RenderTextureFormat.ARGBFloat;

        material = CullBackMaterial;
    }

    void OnRenderObject()
    {
        // set our temprorary texture as target for rendering
        Graphics.SetRenderTarget(renderTexture);
        // clear render texture
        GL.Clear(false, true, new Color(0f, 0f, 0f));

        if (renderObject == null)
        {
            Debug.Log("Object for rendering is not set");
            return;
        }

        // get mesh from input object
        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        if (objectMesh == null)
        {
            Debug.Log("Can't get mesh from input object");
            return;
        }

        // activate first shader pass for our renderer
        material.SetPass(0);

        // draw mesh of input object to render texture
        Graphics.DrawMeshNow(objectMesh, renderObject.transform.localToWorldMatrix);

        // render again to backbuffer
        Graphics.SetRenderTarget(null);
    }

    public void RenderToTextureOnValueChanged()
    {
        if (material == CullBackMaterial)
        {
            material = CullFrontMaterial;
        }
        else
        {
            material = CullBackMaterial;
        }
    }
}
