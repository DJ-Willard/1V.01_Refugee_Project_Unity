using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    public GameObject player;
    public CanvasGroup eBICG;
    public Image endingImage;
    [Range(0, 1)] public float fadeInRate;
    [Range(0, 1)] public float fadeOutRate;

    bool visible = false;
    bool reached = false;
    int count = 0;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            reached = true;
            visible = true;
        }
    }

    void Update()
    {
        if(visible)
        {
            eBICG.alpha = Math.Min(eBICG.alpha + fadeInRate, 1f);
            if(eBICG.alpha == 1f)
            {
                visible = false;
            }
        }
        else
        {
            if(reached)
            {
                Color newColor = endingImage.color;
                newColor.a = Math.Max(endingImage.color.a - fadeOutRate, 0f);
                endingImage.color = newColor;
                count++;
                if(newColor.a <= 0f || count >= 300)
                {
                    Application.Quit();
                }
            }
        }
    }
}
