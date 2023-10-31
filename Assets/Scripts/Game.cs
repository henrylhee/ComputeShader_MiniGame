using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    Controller controller;
    [SerializeField]
    List<FractionSettings> fractionSettings;
    float time;
    float updateTime;
    float fps;

    private void Awake()
    {
        Initialize();
        time = 0;
        updateTime = 0.1f;
        fps = 0;
    }


    void Start()
    {
    }

    void Update()
    {
        fps = 1f / Time.deltaTime;
        if (fps < 20) { Application.Quit(); }
        GameModel.Instance.Update();

        //time += Time.deltaTime;
        //if (time > updateTime)
        //{
        //    GameModel.Instance.Update();
        //    time = 0;
        //    updateTime = 0.1f;
        //}
    }

    private void Initialize()
    {
        GameModel.Instance.Initialize(fractionSettings);

        controller = this.AddComponent<Controller>();
        controller.Initialize();

        Debug.Log("Game initialized.");
    }

    
}
