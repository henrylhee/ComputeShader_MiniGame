using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionSettings
{
    private static FactionSettings instance;
    public static FactionSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FactionSettings();
            }
            return instance;
        }
    }
}
