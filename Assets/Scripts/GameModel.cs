using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameModel
{
    private static GameModel instance;
    public static GameModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameModel();
            }
            return instance;
        }
    }

    [HideInInspector]
    public UnityEvent OnStatesUpdated;

    public Map map { get; private set; }
    public Fractions fractions { get; private set; }

    //Input
    private Vector2 mousePosition;
    private bool mousePressed;

    public Resolution resolution { get; private set; }


    public void Initialize(List<FractionSettings> fractionSettings)
    {
        resolution = Screen.currentResolution;
        fractions = new Fractions();
        map = new Map();
        OnStatesUpdated = new UnityEvent();

        fractions.Initialize(fractionSettings);
        map.Initialize(fractions.settings);

        Debug.Log("GameModel initialized.");
    }

    public void Update()
    {
        UpdateInput();
        map.Update();
        OnStatesUpdated.Invoke();
    }

    private void UpdateStates()
    {
        
    }

    #region Input

    private void UpdateInput()
    {
        mousePosition = Controller.Instance.GetMousePosition();
        if (Controller.Instance.IsMouseJustPressed())
        {
            mousePressed = true;
            MouseJustPressed();
        }
        else if (Controller.Instance.IsMouseJustReleased())
        {
            mousePressed = false;
            MouseJustReleased();
        }
    }

    private void MouseJustPressed()
    {
    }

    private void MouseJustReleased()
    {

    }
    #endregion
}
