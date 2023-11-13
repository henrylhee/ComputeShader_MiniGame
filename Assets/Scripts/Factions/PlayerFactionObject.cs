using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerFaction", menuName = "Factions/Create player faction object")]
public class PlayerFactionObject : FactionObject
{
    [SerializeField, Range(0f, 0.2f)]
    float injectionSize;
    public float InjectionSize { get => injectionSize; }
    [SerializeField, Range(0f, 0.5f)]
    float injectionRandomness;
    public float InjectionRandomness { get => injectionRandomness; }
}
