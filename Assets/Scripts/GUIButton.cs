using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIButton : MonoBehaviour
{
    private const float eps = 0;

    void Update()
    {
        var button = transform.GetComponent<RectTransform>();
        button.localPosition = new Vector3(
            (Screen.width - button.sizeDelta[0]) / 2 - eps,
            -(Screen.height - button.sizeDelta[1]) / 2 - eps,
            0);
    }
}
