using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    Controller controller;

    bool isPlaying = false;

#if !isPlaying
    [SerializeField]
    PlayerFactionObject playerFaction;
    [SerializeField]
    List<FactionObject> enemyFactions;
#endif
    [SerializeField]
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


        GameModel.Instance.Update();

        //time += Time.deltaTime;
        //if (time > updateTime)
        //{
        //    GameModel.Instance.Update();
        //    time = 0;
        //    updateTime = 0.2f;
        //}
    }

    private void OnDestroy()
    {
        GameModel.Instance.map.factionDataBuffer.Release();
        GameModel.Instance.map.pointsBuffer.Release();
        GameModel.Instance.map.impactValuesBuffer.Release();
        GameModel.Instance.map.test.Release();
    }

    private void Initialize()
    {
        factionSettingsList = new List<FactionSettings>();
        FactionSettings dummy = new FactionSettings();
        dummy.Initialize(ScriptableObject.CreateInstance<FactionObject>(),0);
        factionSettingsList.Add(dummy);

        PlayerFactionSettings playerSettings = new PlayerFactionSettings();
        playerSettings.Initialize(playerFaction, Convert.ToInt16(1));
        factionSettingsList.Add(playerSettings);

        short indexOffset = 2;
        for (short index = 0; index < enemyFactions.Count; index++)
        {
            FactionSettings enemySettings = new FactionSettings();
            enemySettings.Initialize(enemyFactions[index], (short)(index + indexOffset));
            factionSettingsList.Add(enemySettings);
        }

        GlobalSettings.Instance.Initialize(factionSettingsList);
        
        GameModel.Instance.Initialize();

        controller = this.AddComponent<Controller>();
        controller.Initialize();

        Debug.Log("Game initialized.");
        isPlaying = true;
    }

    private void OnValidate()
    {
        if(GameModel.Instance.map != null)
        {
            GameModel.Instance.map.UpdateSettings();
        }
    }
}
