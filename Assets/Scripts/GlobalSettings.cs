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

    public void Initialize(List<FactionSettings> factionSettings)
    {
        this.factionSettings = factionSettings;
    }
}
