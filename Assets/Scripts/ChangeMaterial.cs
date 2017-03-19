using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMaterial : MonoBehaviour
{
    public Button button;
    public Renderer cube;
    public Material otherMaterial;
    public Shader otherShader;

    private Material originalMaterial;
    private Shader originalShader;

    void Start()
    {
        var btnChangeMaterial = button.GetComponent<Button>();
        btnChangeMaterial.onClick.AddListener(ChangeMaterialOnClick);
        originalMaterial = cube.material;
        originalShader = cube.material.shader;
    }

    void ChangeMaterialOnClick()
    {
        //cube.material = cube.material != originalMaterial ? originalMaterial : otherMaterial;
        cube.material.shader = cube.material.shader != originalShader ? originalShader : otherShader;
    }
}
