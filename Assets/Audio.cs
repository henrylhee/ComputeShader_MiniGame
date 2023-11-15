using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    AudioSource injectPixelSound;

    void Start()
    {
        injectPixelSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayInjectPixelsSound()
    {
        injectPixelSound.Play();
    }
}
