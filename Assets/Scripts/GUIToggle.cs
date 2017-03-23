using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIToggle : MonoBehaviour
{
	
	void Update()
    {
        var toggleRectTrasform = transform.GetComponent<RectTransform>();
        float toggleWidth = toggleRectTrasform.sizeDelta.x;
        float toggleHeight = toggleRectTrasform.sizeDelta.y;
        // set toggle position in upper left corner of the screen
        toggleRectTrasform.anchoredPosition = new Vector2((toggleWidth - Screen.width) / 2,
                                                          (-toggleHeight + Screen.height) / 2);
    }
}
