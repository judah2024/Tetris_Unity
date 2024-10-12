using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBox : MonoBehaviour
{
    public Vector2Int kTargetResolution;

    float mTargetAspect;
    Camera mCamera;

    void Awake()
    {
        mCamera = Camera.main;
        mTargetAspect = kTargetResolution.x / (float)kTargetResolution.y;

    }

    void Update()
    {
        float currentAspect = Screen.width / (float)Screen.height;
        float scaleHeight = currentAspect / mTargetAspect;
        float scaleWidth = 1 / scaleHeight;

        Rect newRect = mCamera.rect;
        if (scaleHeight < 1.0f)
        {
            newRect = new Rect(0, (1 - scaleHeight) / 2.0f, 1, scaleHeight);
        }
        else
        {
            newRect = new Rect((1 - scaleWidth) / 2.0f, 0, scaleWidth, 1);
        }

        mCamera.rect = newRect;
    }

}
