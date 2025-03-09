using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIN : MonoBehaviour
{
    public static bool fadeIn = false;
    public static bool cammove = false;
    public AudioSource audio;
    public AudioClip clip;
    private bool playonce;
    
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
        

    }

    private void Update()
    {
        if (fadeIn)
        {
            if (!playonce)
            {
                audio.PlayOneShot(clip);
                playonce = true;
            }
            
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime;
                if (canvasGroup.alpha >= 1)
                {
                    canvasGroup.alpha = 1;
                    fadeIn = false;

                }
            }
        }
    }
}

