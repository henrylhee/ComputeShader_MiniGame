using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    UnityEvent GameEnds;

    [SerializeField]
    PlayerFactionObject playerFaction;
    [SerializeField]
    List<FactionObject> enemyFactions;
    [SerializeField]
    List<FactionSettings> factionSettingsList;

    Audio audio;

    float time;
    float updateTime;
    float fps;

    private void Awake()
    {
        Initialize();
        time = Time.time;
        updateTime = .5f;
        fps = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        audio = GetComponentInChildren<Audio>();
        GameModel.Instance.map.PixelsInjected.AddListener(audio.PlayInjectPixelsSound);
    }


    void Update()
    {
        GameModel.Instance.Update();

        //fps = 1f / Time.deltaTime;
        //if (fps < 20) { Application.Quit(); }

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
        GameEnds?.Invoke();
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

        GlobalSettings.Instance.Initialize(factionSettingsList, GetComponentInChildren<GameSettings>());
        
        GameModel.Instance.Initialize();
        GameEnds = new UnityEvent();
        GameEnds.AddListener(GameModel.Instance.map.ReleaseComputeBuffer);

        GetComponent<Controller>().Initialize();

        Debug.Log("Game initialized.");
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            if (GameModel.Instance.map != null)
            {
                GameModel.Instance.map.UpdateSettings();
            }
        }
    }
}
