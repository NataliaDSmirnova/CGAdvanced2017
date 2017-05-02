using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipPlane : MonoBehaviour
{
    // input objects
    public Slider slider;

    // private objects
    private new Renderer renderer;
    private float clipX;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void OnValueChanged(float value)
    {
        clipX = value;
        if (renderer != null)
        {
            renderer.sharedMaterial.SetFloat("_ClipX", clipX);
        }
    }
}
