using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipPlane : MonoBehaviour
{
    // input objects
    public Slider slider;
    public Material clipPlaneMaterial;

    // private objects
    private Renderer cube;
    private float clipX;

    void Start()
    {
        cube = GetComponent<Renderer>();
    }

    public void ClipPlaneOnValueChanged(float value)
    {
        clipX = value - 0.5f;
        cube.sharedMaterial = clipPlaneMaterial;
        cube.sharedMaterial.SetFloat("_ClipX", clipX);
    }
}
