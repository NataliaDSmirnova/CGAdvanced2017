using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownLog : MonoBehaviour {

    public UnityEngine.UI.Dropdown d;
    public string standartNamePVM;

    void Log(int arg)
    {
        string optionselected = d.captionText.text;
        Debug.Log(optionselected);
    }

    // Use this for initialization
    void Start()
    {
        if (d != null)
        {
            d.onValueChanged.AddListener(Log);
            Debug.Log("Hello");


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

}

