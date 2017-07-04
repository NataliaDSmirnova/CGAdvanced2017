using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    // public variables
    public GameObject panelD;
    public bool isPanelDShow;
    public GameObject panelF;
    public bool isPanelFShow;
    public GameObject panelH;
    public bool isPanelHShow;
    public GameObject panelL;
    public bool isPanelLShow;
    
    public GameObject sliderX;
    public GameObject sliderY;    
    public GameObject sliderZ;

    public GameObject panelModeXRay;
    public GameObject panelModeVR;
    public GameObject panelModeSurface;
    public GameObject panelModeComboIsoVR;

    public GameObject panelRGBAmbient;
    public GameObject panelRGBDiffuse;
    public GameObject panelRGBSpecular;
    public GameObject panelShininess;

    public GameObject panelIsoVRRGBAmbient;
    public GameObject panelIsoVRRGBDiffuse;
    public GameObject panelIsoVRRGBSpecular;
    public GameObject panelIsoVRShininess;

    public float updateInterval = 0.5F;

    // private variables
    private GameObject cubeObject;
    private Renderer cubeObjectRenderer;    
    private ClipPlane clipPlane;
    private Text fpsText;

    private Shader xRayShader;
    private Shader volumeShader;
    private Shader isosurfaceShader;
    private Shader normalShader;
    private Shader comboIsoVRShader;    

    private float accum = 0; 
    private int frames = 0; 
    private float timeleft;

    // Use this for initialization
    void Start ()
    {
        cubeObject = GameObject.Find("Cube");
        cubeObjectRenderer = cubeObject.GetComponent<Renderer>();
        clipPlane = cubeObject.GetComponent<ClipPlane>();            

        clipPlane.OnValueXChanged(-0.5f);
        clipPlane.OnValueYChanged(-0.5f);

        fpsText = panelF.transform.FindChild("TextFPS").GetComponent<Text>();

        xRayShader = Shader.Find("CGA/X-Ray");
        volumeShader = Shader.Find("CGA/VolumeRendering");
        isosurfaceShader = Shader.Find("CGA/Isosurface");
        normalShader = Shader.Find("CGA/NormalShader");
        comboIsoVRShader = Shader.Find("CGA/ComboIsosurfaceVolumeRendering");

        cubeObjectRenderer.sharedMaterial.shader = xRayShader;

        panelD.SetActive(isPanelDShow);
        panelF.SetActive(isPanelFShow);
        panelH.SetActive(isPanelHShow);
        panelL.SetActive(isPanelLShow);

        panelModeXRay.SetActive(true);
        panelModeVR.SetActive(false);
        panelModeSurface.SetActive(false);
        panelModeComboIsoVR.SetActive(false);

        panelRGBAmbient.SetActive(false);
        panelRGBDiffuse.SetActive(false);
        panelRGBSpecular.SetActive(false);
        panelShininess.SetActive(false);

        panelIsoVRRGBAmbient.SetActive(false);
        panelIsoVRRGBDiffuse.SetActive(false);
        panelIsoVRRGBSpecular.SetActive(false);
        panelIsoVRShininess.SetActive(false);

        timeleft = updateInterval;
    }
	
    // Update is called once per frame
    void Update ()
    {
	if (Input.GetKeyDown(KeyCode.W))
        {
            isPanelDShow = !isPanelDShow;
            panelD.SetActive(isPanelDShow);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isPanelFShow = !isPanelFShow;
            panelF.SetActive(isPanelFShow);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isPanelLShow = !isPanelLShow;
            panelL.SetActive(isPanelLShow);
        }

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        
        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            fpsText.text = "FPS: " + fps.ToString("F1");

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    public void PanelHClose()
    {
        isPanelHShow = false;
        panelH.SetActive(isPanelHShow);
    }

    public void PanelHOpen()
    {
        isPanelHShow = true;
        panelH.SetActive(isPanelHShow);
    }
                

    public void ChangeMode(UnityEngine.UI.Dropdown d)
    {

        switch (d.value)
        {
            case 0:
                cubeObjectRenderer.sharedMaterial.shader = xRayShader;

                panelModeXRay.SetActive(true);
                panelModeVR.SetActive(false);
                panelModeSurface.SetActive(false);
                panelModeComboIsoVR.SetActive(false);

                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);

                panelIsoVRRGBAmbient.SetActive(false);
                panelIsoVRRGBDiffuse.SetActive(false);
                panelIsoVRRGBSpecular.SetActive(false);
                panelIsoVRShininess.SetActive(false);

                break;
            case 1:
                cubeObjectRenderer.sharedMaterial.shader = volumeShader;
                cubeObjectRenderer.sharedMaterial.SetInt("_TransferFunctionId", 0);

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(true);
                panelModeSurface.SetActive(false);
                panelModeComboIsoVR.SetActive(false);

                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);

                panelIsoVRRGBAmbient.SetActive(false);
                panelIsoVRRGBDiffuse.SetActive(false);
                panelIsoVRRGBSpecular.SetActive(false);
                panelIsoVRShininess.SetActive(false);

                break;
            case 2:
                cubeObjectRenderer.sharedMaterial.shader = isosurfaceShader;                                

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(false);
                panelModeSurface.SetActive(true);
                panelModeComboIsoVR.SetActive(false);

                panelRGBAmbient.SetActive(true);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);

                panelIsoVRRGBAmbient.SetActive(false);
                panelIsoVRRGBDiffuse.SetActive(false);
                panelIsoVRRGBSpecular.SetActive(false);
                panelIsoVRShininess.SetActive(false);
                break;
            case 3:
                cubeObjectRenderer.sharedMaterial.shader = comboIsoVRShader;
                cubeObjectRenderer.sharedMaterial.SetInt("_TransferFunctionId", 0);                                

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(false);
                panelModeSurface.SetActive(false);
                panelModeComboIsoVR.SetActive(true);

                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);

                panelIsoVRRGBAmbient.SetActive(true);
                panelIsoVRRGBDiffuse.SetActive(false);
                panelIsoVRRGBSpecular.SetActive(false);
                panelIsoVRShininess.SetActive(false);
                break;
            default:
                cubeObjectRenderer.sharedMaterial.shader = normalShader;

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(false);
                panelModeSurface.SetActive(false);

                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);

                panelIsoVRRGBAmbient.SetActive(false);
                panelIsoVRRGBDiffuse.SetActive(false);
                panelIsoVRRGBSpecular.SetActive(false);
                panelIsoVRShininess.SetActive(false);

                break;
        }
    }    

    public void ChangeReflParams(UnityEngine.UI.Dropdown param)
    {
        switch (param.value)
        {
            case 0:
                panelRGBAmbient.SetActive(true);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);
                break;
            case 1:
                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(true);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(false);
                break;
            case 2:
                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(true);
                panelShininess.SetActive(false);
                break;
            case 3:
                panelRGBAmbient.SetActive(false);
                panelRGBDiffuse.SetActive(false);
                panelRGBSpecular.SetActive(false);
                panelShininess.SetActive(true);
                break;
        }
    }

    public void ChangeIsoVRReflParams(UnityEngine.UI.Dropdown param)
    {
      switch (param.value)
      {
        case 0:
          panelIsoVRRGBAmbient.SetActive(true);
          panelIsoVRRGBDiffuse.SetActive(false);
          panelIsoVRRGBSpecular.SetActive(false);
          panelIsoVRShininess.SetActive(false);
          break;
        case 1:
          panelIsoVRRGBAmbient.SetActive(false);
          panelIsoVRRGBDiffuse.SetActive(true);
          panelIsoVRRGBSpecular.SetActive(false);
          panelIsoVRShininess.SetActive(false);
          break;
        case 2:
          panelIsoVRRGBAmbient.SetActive(false);
          panelIsoVRRGBDiffuse.SetActive(false);
          panelIsoVRRGBSpecular.SetActive(true);
          panelIsoVRShininess.SetActive(false);
          break;
        case 3:
          panelIsoVRRGBAmbient.SetActive(false);
          panelIsoVRRGBDiffuse.SetActive(false);
          panelIsoVRRGBSpecular.SetActive(false);
          panelIsoVRShininess.SetActive(true);
          break;
      }
    }


  public void ChangeTransferParams(UnityEngine.UI.Dropdown param)
    {
      if (param.value >= 0 && param.value <= 2)
      {
        cubeObjectRenderer.sharedMaterial.SetInt("_TransferFunctionId", param.value);
      }      
    }
}
