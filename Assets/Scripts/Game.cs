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
    List<FactionObject> factionObjects;
    List<FactionSettings> factionSettingsList;
    float time;
    float updateTime;
    float fps;

    private void Awake()
    {
        Initialize();
        time = Time.time;
        updateTime = .5f;
        fps = 0;
    }


    void Start()
    {
    }

    void Update()
    {
        fps = 1f / Time.deltaTime;
        if (fps < 20) { Application.Quit(); }

        if (time < Time.time - updateTime)
        {
            time = Time.time;
        }
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
        factionSettingsList = new List<FactionSettings>();
        FactionSettings dummy = new FactionSettings();
        dummy.Initialize(new FactionObject(),0);
        factionSettingsList.Add(dummy);

        for (short index = 1; index <= factionObjects.Count; index++)
        {
            FactionSettings factionSettings = new FactionSettings();
            factionSettings.Initialize(factionObjects[index-1], index);
            factionSettingsList.Add(factionSettings);
        }

        GlobalSettings.Instance.Initialize(factionSettingsList);
        
        GameModel.Instance.Initialize();

        controller = this.AddComponent<Controller>();
        controller.Initialize();

        Debug.Log("Game initialized.");
    }
}
