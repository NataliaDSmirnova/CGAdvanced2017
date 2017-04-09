using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownLog : MonoBehaviour {

    public UnityEngine.UI.Dropdown d = null;

    void Log(int arg)
    {
        Debug.Log(d.value.ToString());
        Debug.Log("Work");
    }

    // Use this for initialization
    void Start()
    {
        d.onValueChanged.AddListener(Log);
        Debug.Log("Hello");
    }

    // Update is called once per frame
    void Update()
    {

    }

}
