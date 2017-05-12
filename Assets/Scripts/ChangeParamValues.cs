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
    private float isosurfaceAmbientColorR;
    private float isosurfaceAmbientColorG;
    private float isosurfaceAmbientColorB;
    private float isosurfaceDiffuseColorR;
    private float isosurfaceDiffuseColorG;
    private float isosurfaceDiffuseColorB;
    private float isosurfaceSpecularColorR;
    private float isosurfaceSpecularColorG;
    private float isosurfaceSpecularColorB;
    private float isosurfaceShininess;

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

    public void OnValueIsosurfaceAmbientRColorChanged(float value)
    {
        isosurfaceAmbientColorR = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_AmbientR", isosurfaceAmbientColorR);
        }
    }

    public void OnValueIsosurfaceAmbientGColorChanged(float value)
    {
        isosurfaceAmbientColorG = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_AmbientG", isosurfaceAmbientColorG);
        }
    }

    public void OnValueIsosurfaceAmbientBColorChanged(float value)
    {
        isosurfaceAmbientColorB = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_AmbientB", isosurfaceAmbientColorB);
        }
    }

    public void OnValueIsosurfaceDiffuseRColorChanged(float value)
    {
        isosurfaceDiffuseColorR = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_DiffuseR", isosurfaceDiffuseColorR);
        }
    }

    public void OnValueIsosurfaceDiffuseGColorChanged(float value)
    {
        isosurfaceDiffuseColorG = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_DiffuseG", isosurfaceDiffuseColorG);
        }
    }

    public void OnValueIsosurfaceDiffuseBColorChanged(float value)
    {
        isosurfaceDiffuseColorB = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_DiffusetB", isosurfaceDiffuseColorB);
        }
    }

    public void OnValueIsosurfaceSpecularRColorChanged(float value)
    {
        isosurfaceSpecularColorR = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_SpecularR", isosurfaceSpecularColorR);
        }
    }

    public void OnValueIsosurfaceSpecularGColorChanged(float value)
    {
        isosurfaceSpecularColorG = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_SpecularG", isosurfaceSpecularColorG);
        }
    }

    public void OnValueIsosurfaceSpecularBColorChanged(float value)
    {
        isosurfaceSpecularColorB = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_SpecularB", isosurfaceSpecularColorB);
        }
    }

    public void OnValueShininessChanged(float value)
    {
        isosurfaceShininess = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_Shininess", isosurfaceShininess);
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
