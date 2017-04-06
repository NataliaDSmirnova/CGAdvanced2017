using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMaterial : MonoBehaviour
{
    public Material otherMaterial;

    private Renderer cube;
    private Material originalMaterial;

    void Start()
    {
        cube = GetComponent<Renderer>();
        originalMaterial = cube.material;
    }

    public void ChangeMaterialOnClick()
    {
        cube.material = cube.sharedMaterial != originalMaterial ? originalMaterial : otherMaterial;
    }

    public void OpenFileOnClick()
    {
        var filePath = UnityEditor.EditorUtility.OpenFilePanel("level", Application.streamingAssetsPath, "txt");
        if (filePath.Length != 0)
        {
            Debug.Log(filePath);
        }
    }
}
