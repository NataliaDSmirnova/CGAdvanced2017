﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderObject : MonoBehaviour {
    // input object
    public GameObject renderObject;
    public Camera mainCamera;

    // private variables
    private RenderTexture renderTexture;
    private RawImage image;
    
    void Start () {
        // get raw image from scene (see RTSprite)
        image = GetComponent<RawImage>();
    }
	
	void Update () {
        // create temprorary texture of screen size
        renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        // set our temprorary texture as target for rendering
        Graphics.SetRenderTarget(renderTexture);

        // get mesh and meshRenderer from input object
        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        var renderer = renderObject.GetComponent<MeshRenderer>();

        // activate first shader pass for our renderer
        renderer.material.SetPass(0);

        // render from camera to texture
        // mainCamera.targetTexture = renderTexture;
        // mainCamera.Render();

        // draw mesh of input object to render texture
        Graphics.DrawMeshNow(objectMesh, renderer.localToWorldMatrix * mainCamera.worldToCameraMatrix * mainCamera.projectionMatrix);

        // set texture of raw image equals to our render texture
        image.texture = renderTexture;

        //mainCamera.targetTexture = null;

        // render again to backbuffer
        Graphics.SetRenderTarget(null);
        // release texture
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}