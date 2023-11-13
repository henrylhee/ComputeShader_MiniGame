using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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
    public UnityEvent StatesUpdated;

    public Map map { get; private set; }

    public Resolution resolution { get; private set; }


    public void Initialize()
    {
        resolution = Screen.currentResolution;
        map = new Map();
        StatesUpdated = new UnityEvent();

        map.Initialize();

        Debug.Log("GameModel initialized.");
    }

    public void Update()
    {
        // input events executed before unity update!
        UpdateStates();
        StatesUpdated.Invoke();
    }

    private void UpdateStates()
    {
        map.Update();
    }
}
