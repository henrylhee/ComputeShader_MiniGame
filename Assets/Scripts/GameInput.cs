using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameInput : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 mousePositionLastPressed;

    public UnityEvent<Vector3> MouseJustPressed;
    public UnityEvent<Vector3> MouseJustReleased;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        MouseJustPressed.Invoke(Input.mousePosition);
        MouseJustReleased.Invoke(Input.mousePosition);
    }
}
