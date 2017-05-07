using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextureSwitcher : MonoBehaviour
{
    // private objects
    private RawImage image;
    private bool renderFront = true;
    private BackFrontTextureCreator textureCreator;

    void Start()
    {
        // get raw image from scene (see RTSprite)
        image = GetComponent<RawImage>();
        // get texture creator from camera
        textureCreator = Camera.main.GetComponent<BackFrontTextureCreator>();
    }

    void Update()
    {
        // rescale raw image size depending on screen size
        float imageHeight = image.rectTransform.sizeDelta.y;
        float imageWidth = imageHeight * Screen.width / (float)Screen.height;
        image.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);

        // renew texture 
        SetTexture();
    }

    public void Switch()
    {
        renderFront = !renderFront;
    }

    private void SetTexture()
    {
        if (!textureCreator)
        {
            Debug.Log("Texture creator is null");
            return;
        }
        image.texture = renderFront ? textureCreator.FrontTexture : textureCreator.BackTexture;
    }
}