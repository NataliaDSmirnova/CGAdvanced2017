using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipPlane : MonoBehaviour
{

    // input objects
    public GameObject renderObject;
    public Slider slider;
    public Shader shader;

    private float clipX;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        clipX = slider.value - 0.5f;
    }

    void OnRenderObject()
    {
        Graphics.SetRenderTarget(null);

        // get mesh from input object
        Mesh objectMesh = renderObject.GetComponent<MeshFilter>().sharedMesh;
        if (objectMesh == null)
        {
            Debug.Log("Can't get mesh from input object");
            return;
        }

        // get mesh renderer from input object
        var renderer = renderObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.Log("Can't get mesh renderer from input object");
            return;
        }

        // activate first shader pass for our renderer
        renderer.material.shader = shader;
        renderer.material.SetFloat("_ClipX", clipX);
        renderer.material.SetPass(0);

        // draw mesh of input object
        Graphics.DrawMeshNow(objectMesh, renderObject.transform.localToWorldMatrix);
    }
}
