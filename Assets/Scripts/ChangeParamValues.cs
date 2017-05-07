using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParamValues : MonoBehaviour {

    // private objects
    private new Renderer renderer;

    // xray params
    private float xRayColorR;
    private float xRayColorG;
    private float xRayColorB;

    // isosurface params
    private float isosurfaceThreshold;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void OnValueThresholdChanged(float value)
    {
        isosurfaceThreshold = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", isosurfaceThreshold);
        }
    }

    public void OnValueXRayRColorChanged(float value)
    {
        xRayColorR = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_XRayColorR", xRayColorR);
        }
    }

    public void OnValueXRayGColorChanged(float value)
    {
        xRayColorG = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_XRayColorG", xRayColorG);
        }
    }

    public void OnValueXRayBColorChanged(float value)
    {
        xRayColorB = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_XRayColorB", xRayColorB);
        }
    }
}
