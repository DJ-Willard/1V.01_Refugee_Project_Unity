using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnding : MonoBehaviour
{
    public GameObject player;
    public CanvasGroup mBICG;
    [Range(0, 1)] public float fadeInRate;
    [Range(0, 1)] public float fadeOutRate;

    bool first = true;
    bool visible = false;
    
    void OnTriggerEnter(Collider other)
    {
        if(first && other.gameObject == player)
        {
            first = false;
            visible = true;
        }
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            visible = false;
        }

        if(visible)
        {
            mBICG.alpha = Math.Min(mBICG.alpha + fadeInRate, 1f);
        }
        else
        {
            mBICG.alpha = Math.Max(mBICG.alpha - fadeOutRate, 0f);
        }
    }
}
