using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings
{
    private static GlobalSettings instance;

    public static GlobalSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GlobalSettings();
            }
            return instance;
        }
    }

    public List<FactionSettings> factionSettings;
    public GameSettings gameSettings;

    public void Initialize(List<FactionSettings> factionSettings, GameSettings gameSettings)
    {
        this.factionSettings = factionSettings;
        this.gameSettings = gameSettings;
    }
}
