using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HazzardWarning : MonoBehaviour
{
    public List<Image> images;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float pulseSpeed = 1.0f;

    private bool[] isImageDisplayed = new bool[5];

    void Start()
    {
        foreach (Image image in images)
        {
            image.enabled = false;
        }
    }

    void Update()
    {
        float scale = minScale + Mathf.PingPong(Time.time * pulseSpeed, maxScale - minScale);
        foreach (Image image in images)
        {
            image.transform.localScale = new Vector3(scale, scale, 1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && images.Count > 0)
        {
            EnableImageDisplay(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && images.Count > 1)
        {
            EnableImageDisplay(1);
        }
        //else if (Input.GetKeyDown(KeyCode.Alpha3) && images.Count > 2)
        //{
        //    EnableImageDisplay(2);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4) && images.Count > 3)
        //{
        //    EnableImageDisplay(3);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5) && images.Count > 4)
        //{
        //    ToggleImageDisplay(4);
        //}
    }

    public void EnableImageDisplay(int index)
    {
        images[index].enabled = true;

        int displayedImagesCount = 0;
        int displayedImageIndex = -1;

        for (int i = 0; i < isImageDisplayed.Length; i++)
        {
            if (isImageDisplayed[i])
            {
                displayedImagesCount++;
                displayedImageIndex = i;
            }
        }

        if (displayedImagesCount == 1)
        {
            images[displayedImageIndex].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else if (displayedImagesCount > 1)
        {
            float totalWidth = 0f;
            for (int i = 0; i < images.Count; i++)
            {
                if (isImageDisplayed[i])
                {
                    totalWidth += images[i].GetComponent<RectTransform>().rect.width;
                }
            }

            float xOffset = -(totalWidth / 2f) + (images[displayedImageIndex].GetComponent<RectTransform>().rect.width / 2f);
            for (int i = 0; i < images.Count; i++)
            {
                if (isImageDisplayed[i])
                {
                    images[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, 0f);
                    xOffset += images[i].GetComponent<RectTransform>().rect.width;
                }
            }
        }
    }
    
    public void DisableImageDisplay(int index)
    {
        images[index].enabled = false;
    }
}

