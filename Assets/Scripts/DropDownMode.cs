using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownMode : MonoBehaviour
{
  public Dropdown dropdown;
  public Dropdown dropdownLog;
  public string currMode;

  // Sliders: X-ray
  public Slider sliderXrayR;
  public Slider sliderXrayG;
  public Slider sliderXrayB;

  // Sliders: Iso
  public Slider sliderIsoThreshold;
  public Slider sliderIsoAmbientR;
  public Slider sliderIsoAmbientG;
  public Slider sliderIsoAmbientB;
  public Slider sliderIsoDiffuseR;
  public Slider sliderIsoDiffuseG;
  public Slider sliderIsoDiffuseB;
  public Slider sliderIsoSpecularR;
  public Slider sliderIsoSpecularG;
  public Slider sliderIsoSpecularB;
  public Slider sliderIsoShininess;

  // Sliders Iso-VR
  public Slider sliderIsoVRThreshold;
  public Slider sliderIsoVRAmbientR;
  public Slider sliderIsoVRAmbientG;
  public Slider sliderIsoVRAmbientB;
  public Slider sliderIsoVRDiffuseR;
  public Slider sliderIsoVRDiffuseG;
  public Slider sliderIsoVRDiffuseB;
  public Slider sliderIsoVRSpecularR;
  public Slider sliderIsoVRSpecularG;
  public Slider sliderIsoVRSpecularB;
  public Slider sliderIsoVRShininess;

  private Renderer shaderRenderer;
  private DropDownLog dropdownLogScript;

  // Use this for initialization
  void Start()
  {
    if (dropdown != null)
    {
      GameObject cubeObj = GameObject.Find("Cube");
      shaderRenderer = cubeObj.GetComponent<Renderer>();
      dropdown.onValueChanged.AddListener(OnValueChange);
      currMode = "X-ray";
      dropdownLogScript = GameObject.FindObjectOfType(typeof(DropDownLog)) as DropDownLog;
    }    
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnValueChange(int arg)
  {
    string newMode = dropdown.captionText.text;
    
    if (newMode == "X-ray")
    {
      sliderXrayR.value = 0.0f;
      sliderXrayG.value = 0.0f;
      sliderXrayB.value = 0.0f;
      shaderRenderer.sharedMaterial.SetFloat("_XRayColorR", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_XRayColorG", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_XRayColorB", 0.0f);
    }
    else if (newMode == "Volume rendering")
    {
      ; // do nothing
    }
    else if (newMode == "Isosurface")
    {
      sliderIsoThreshold.value = dropdownLogScript.recommendedThreshold;
      sliderIsoAmbientR.value = 0.0f;
      sliderIsoAmbientG.value = 0.0f;
      sliderIsoAmbientB.value = 0.0f;
      sliderIsoDiffuseR.value = 0.0f;
      sliderIsoDiffuseG.value = 0.5f;
      sliderIsoDiffuseB.value = 0.5f;
      sliderIsoSpecularR.value = 1.0f;
      sliderIsoSpecularG.value = 1.0f;
      sliderIsoSpecularB.value = 1.0f;
      sliderIsoShininess.value = 32;

      shaderRenderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", dropdownLogScript.recommendedThreshold);
      shaderRenderer.sharedMaterial.SetFloat("_AmbientR", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_AmbientG", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_AmbientB", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_DiffuseR", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_DiffuseG", 0.5f);
      shaderRenderer.sharedMaterial.SetFloat("_DiffuseB", 0.5f);
      shaderRenderer.sharedMaterial.SetFloat("_SpecularR", 1.0f);
      shaderRenderer.sharedMaterial.SetFloat("_SpecularG", 1.0f);
      shaderRenderer.sharedMaterial.SetFloat("_SpecularB", 1.0f);
      shaderRenderer.sharedMaterial.SetFloat("_Shininess", 32);
    }
    else // if (newMode == "Combo Iso VR") 
    {
      sliderIsoVRThreshold.value = dropdownLogScript.recommendedThreshold;
      sliderIsoVRAmbientR.value = 0.0f;
      sliderIsoVRAmbientG.value = 0.0f;
      sliderIsoVRAmbientB.value = 0.0f;
      sliderIsoVRDiffuseR.value = 0.0f;
      sliderIsoVRDiffuseG.value = 0.5f;
      sliderIsoVRDiffuseB.value = 0.5f;
      sliderIsoVRSpecularR.value = 1.0f;
      sliderIsoVRSpecularG.value = 1.0f;
      sliderIsoVRSpecularB.value = 1.0f;
      sliderIsoVRShininess.value = 32;

      shaderRenderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", dropdownLogScript.recommendedThreshold);
      shaderRenderer.sharedMaterial.SetFloat("_AmbientR", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_AmbientG", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_AmbientB", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_DiffuseR", 0.0f);
      shaderRenderer.sharedMaterial.SetFloat("_DiffuseG", 0.5f);
      shaderRenderer.sharedMaterial.SetFloat("_DiffuseB", 0.5f);
      shaderRenderer.sharedMaterial.SetFloat("_SpecularR", 1.0f);
      shaderRenderer.sharedMaterial.SetFloat("_SpecularG", 1.0f);
      shaderRenderer.sharedMaterial.SetFloat("_SpecularB", 1.0f);
      shaderRenderer.sharedMaterial.SetFloat("_Shininess", 32);
    }

    currMode = newMode;
  }
}
