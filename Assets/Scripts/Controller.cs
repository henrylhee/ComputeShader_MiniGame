using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    Resolution resolution;

    [HideInInspector]
    public UnityEvent<int,int> MouseJustPressed;
    [HideInInspector]
    public UnityEvent<int,int> MouseJustReleased;

    Vector2 mousePosition;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
            MouseJustPressed?.Invoke(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));
            Debug.Log("mouse just pressed");
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                mousePosition = Input.mousePosition;
                MouseJustReleased?.Invoke(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));
            }
        }
    }

    public void Initialize()
    {
        resolution = GameModel.Instance.resolution;

        MouseJustPressed = new UnityEvent<int,int>();
        MouseJustReleased = new UnityEvent<int, int>();
        MouseJustPressed.AddListener(GameModel.Instance.map.InjectPixels);

        Debug.Log("Controller initialized.");
    }
}
