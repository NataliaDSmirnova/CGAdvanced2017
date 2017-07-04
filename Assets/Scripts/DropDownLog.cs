using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownLog : MonoBehaviour {

    // public variables
    public Dropdown d;
    public string standartNamePVM;
    public GameObject logPanel;
    public Slider sliderIsoThreshold;
    public float recommendedThreshold;

    // private variables
    private Volume volumeClass;
    private new Renderer renderer;

    // Use this for initialization
    void Start()
    {
        if (d != null)
        {

            GameObject cubeObj = GameObject.Find("Cube");
            volumeClass = cubeObj.GetComponent<Volume>();
            renderer = cubeObj.GetComponent<Renderer>();

            d.onValueChanged.AddListener(OnValueChange);
            // List<string> PVMInResources = new List<string>();
            string pathResources = "Assets/Resources/";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(pathResources);
            System.IO.FileInfo[] files = dir.GetFiles("*.pvm");
            int dValue = 0;
            int idx = 0;
            foreach (System.IO.FileInfo f in files)
            {
                string tempName = f.Name;
                if (tempName == standartNamePVM)
                {
                    dValue = idx;
                }
                d.options.Add(new UnityEngine.UI.Dropdown.OptionData(tempName));
                idx = idx + 1;

            }
            d.value = dValue;
            d.Select();
            d.RefreshShownValue();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnValueChange(int arg)
    {        
        volumeClass.LoadTextureDataFromPVM(d.captionText.text);

        // set default options
        if (d.captionText.text == "Baby.pvm")
        {
          recommendedThreshold = 0.4f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.008f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "Base.pvm")
        {
          recommendedThreshold = 0.05f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.025f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "BluntFin.pvm")
        {
          recommendedThreshold = 0.05f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.025f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "DTI-B0.pvm")
        {
          recommendedThreshold = 0.37f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.01f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "DTI-MD.pvm")
        {
          recommendedThreshold = 0.17f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.03f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "Fuel.pvm")
        {
          recommendedThreshold = 0.05f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.05f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "Orange.pvm")
        {
          recommendedThreshold = 0.21f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.01f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "Standart.pvm")
        {
          recommendedThreshold = 0.05f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.025f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "Temp.pvm")
        {
          recommendedThreshold = 0.05f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.025f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }

        else if (d.captionText.text == "Test.pvm")
        {
          recommendedThreshold = 0.05f;
          renderer.sharedMaterial.SetFloat("_ColorFactor", 0.025f);
          renderer.sharedMaterial.SetFloat("_IsosurfaceThreshold", recommendedThreshold);
          sliderIsoThreshold.value = recommendedThreshold;
        }
    }
}

