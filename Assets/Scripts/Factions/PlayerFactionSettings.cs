using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


[System.Serializable]
public class PlayerFactionSettings : FactionSettings
{
    [SerializeField, Range(0f, 0.2f)]
    float injectionSize;
    public float InjectionSize { get => injectionSize; }
    [SerializeField, Range(0f, 0.5f)]
    float injectionRandomness;
    public float InjectionRandomness { get => injectionRandomness; }

    public void Initialize(PlayerFactionObject faction, short id)
    {
        base.Initialize(faction, id);
        this.injectionSize = faction.InjectionSize;
        this.injectionRandomness = faction.InjectionRandomness;
    }
}
