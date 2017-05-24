using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownLog : MonoBehaviour {

    // public variables
    public Dropdown d;
    public string standartNamePVM;
    public GameObject logPanel;

    // private variables
    private Volume volumeClass;

    // Use this for initialization
    void Start()
    {
        if (d != null)
        {

            GameObject cubeObj = GameObject.Find("Cube");
            volumeClass = cubeObj.GetComponent<Volume>();         

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
    }

}

