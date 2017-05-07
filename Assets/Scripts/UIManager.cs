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

    public Toggle toggleH;
    public GameObject sliderH;
    public Toggle toggleV;
    public GameObject sliderV;

    public GameObject panelModeXRay;
    public GameObject panelModeVR;
    public GameObject panelModeSurface;

    public GameObject panelRGB;
    public GameObject panelShininess;

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

    private float accum = 0; 
    private int frames = 0; 
    private float timeleft;

    // Use this for initialization
    void Start ()
    {
        cubeObject = GameObject.Find("Cube");
        cubeObjectRenderer = cubeObject.GetComponent<Renderer>();
        clipPlane = cubeObject.GetComponent<ClipPlane>();

        fpsText = panelF.transform.FindChild("TextFPS").GetComponent<Text>();

        xRayShader = Shader.Find("CGA/X-Ray");
        // TODO: fix to volume shader
        volumeShader = Shader.Find("CGA/X-Ray");
        isosurfaceShader = Shader.Find("CGA/Isosurface");
        normalShader = Shader.Find("CGA/NormalShader");

        cubeObjectRenderer.sharedMaterial.shader = xRayShader;

        panelD.SetActive(isPanelDShow);
        panelF.SetActive(isPanelFShow);
        panelH.SetActive(isPanelHShow);
        panelL.SetActive(isPanelLShow);

        panelModeXRay.SetActive(true);
        panelModeVR.SetActive(false);
        panelModeSurface.SetActive(false);

        panelRGB.SetActive(false);
        panelShininess.SetActive(false);

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
            fpsText.text = "FPS: " + fps;

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
    
    public void OnClickToggleH()
    {
        sliderH.SetActive(toggleH.isOn);
        if (!toggleH.isOn)
        {
            clipPlane.OnValueXChanged(-0.5f);
        }
        else
        {
            clipPlane.OnValueXChanged(sliderH.GetComponent<Slider>().value);
        }
    }

    public void OnClickToggleV()
    {
        sliderV.SetActive(toggleV.isOn);
        if (!toggleV.isOn)
        {
            clipPlane.OnValueYChanged(-0.5f);
        }
        else
        {
            clipPlane.OnValueYChanged(sliderV.GetComponent<Slider>().value);
        }
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

                panelRGB.SetActive(false);
                panelShininess.SetActive(false);
                break;
            case 1:
                cubeObjectRenderer.sharedMaterial.shader = volumeShader;

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(true);
                panelModeSurface.SetActive(false);

                panelRGB.SetActive(false);
                panelShininess.SetActive(false);
           
                break;
            case 2:
                cubeObjectRenderer.sharedMaterial.shader = isosurfaceShader;

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(false);
                panelModeSurface.SetActive(true);

                panelRGB.SetActive(true);
                panelShininess.SetActive(false);
                break;
            default:
                cubeObjectRenderer.sharedMaterial.shader = normalShader;

                panelModeXRay.SetActive(false);
                panelModeVR.SetActive(false);
                panelModeSurface.SetActive(false);

                panelRGB.SetActive(false);
                panelShininess.SetActive(false);
                break;
        }
    }

    public void ChangeReflParams(UnityEngine.UI.Dropdown param)
    {
        if (param.value == 3)
        {
            panelRGB.SetActive(false);
            panelShininess.SetActive(true);
        }
        else
        {
            panelRGB.SetActive(true);
            panelShininess.SetActive(false);
        }
    }

}
