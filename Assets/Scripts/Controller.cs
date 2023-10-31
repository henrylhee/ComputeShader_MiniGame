using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private static Controller instance;
    public static Controller Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Controller>();
                if (instance == null)
                    instance = new GameObject(typeof(Controller).Name).AddComponent<Controller>();
                //DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    Resolution resolution;

    void Update()
    {
        
    }

    public void Initialize()
    {
        resolution = Screen.currentResolution;
        Debug.Log("Controller screen resolution:" + new Vector2(resolution.width,resolution.height));

        Debug.Log("Controller initialized.");
    }

    public Vector2 GetMousePosition()
    {
        return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    public bool IsMouseJustPressed()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool IsMouseJustReleased()
    {
        return Input.GetMouseButtonUp(0);
    }
}
