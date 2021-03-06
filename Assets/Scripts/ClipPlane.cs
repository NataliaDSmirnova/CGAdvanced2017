﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipPlane : MonoBehaviour
{
    // private objects
    private new Renderer renderer;
    private float clipX;
    private float clipY;
    private float clipZ;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void OnValueXChanged(float value)
    {
        clipX = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_ClipX", clipX);
        }
    }

    public void OnValueYChanged(float value)
    {
        clipY = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_ClipY", clipY);
        }
    }

    public void OnValueZChanged(float value)
    {
        clipZ = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_ClipZ", clipZ);
        }
    }
}
