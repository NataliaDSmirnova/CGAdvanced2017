﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderToTexture : MonoBehaviour {

    // input objects
    public GameObject renderObject;
    public Material CullBackMaterial;
    public Material CullFrontMaterial;

    // private objects
    private RawImage image;
    private Material material;
    private bool cullBack = true;
    private RenderTexture renderTexture;

    void Start()
    {
        // get raw image from scene (see RTSprite)
        image = GetComponent<RawImage>();

        // create temprorary texture of screen size
        renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        renderTexture.format = RenderTextureFormat.ARGBFloat;
    }

    void Update()
    {
        // rescale raw image size depending on screen size
        float imageHeight = image.rectTransform.sizeDelta.y;
        float imageWidth = imageHeight * Screen.width / (float)Screen.height;
        image.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
        // fix position of image in left lower corner
        image.rectTransform.anchoredPosition = new Vector2((imageWidth - Screen.width) / 2,
                                                           (imageHeight - Screen.height) / 2);
    }

    void OnRenderObject()
    {
        // set our temprorary texture as target for rendering
        Graphics.SetRenderTarget(renderTexture);
        // clear render texture
        GL.Clear(false, true, new Color(0f, 0f, 0f));

        if (renderObject == null)
        {
            // Debug.Log("Object for rendering is not set");
            return;
        }

        // get mesh from input object
        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        if (objectMesh == null)
        {
            //Debug.Log("Can't get mesh from input object");
            return;
        }

        material = cullBack ? CullBackMaterial : CullFrontMaterial;
        // activate first shader pass for our renderer
        material.SetPass(0);

        // draw mesh of input object to render texture
        Graphics.DrawMeshNow(objectMesh, renderObject.transform.localToWorldMatrix);

        // set texture of raw image equals to our render texture
        image.texture = renderTexture;

        // render again to backbuffer
        Graphics.SetRenderTarget(null);
    }

    public void RenderToTextureOnValueChanged()
    {
        cullBack = !cullBack;
    }
}