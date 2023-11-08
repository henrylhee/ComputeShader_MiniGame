using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionSettings
{
    [SerializeField]
    private string _name;
    public string _Name { get => _name; }
    [SerializeField]
    private Color color;
    public Color Color { get => color; }
    [SerializeField]
    private Vector2Int startPosition;
    public Vector2Int StartPosition { get => startPosition;}
    [SerializeField]
    private float conquerRate;
    public float ConquerRate { get => conquerRate; }
    [SerializeField]
    private float conquerStrength;
    public float ConquerStrength { get => conquerStrength; }
    [SerializeField]
    private float expansionRate;
    public float ExpansionRate { get => expansionRate; }
    [SerializeField]
    private float expansionStrength;
    public float ExpansionStrength { get => expansionStrength; }
    [SerializeField]
    private float expansionRange;
    public float ExpansionRange { get => expansionRange; }
    [SerializeField]
    private float sharpness;
    public float Sharpness { get => sharpness; }
    [SerializeField]
    private float stability;
    public float Stability { get => stability; }
    [SerializeField]
    private float randomness;
    public float Randomness { get => randomness; }

    public short id { get; private set; }


    public void Initialize(FactionObject faction, short id)
    {
        _name = faction._Name;
        color = faction.Color;
        startPosition = faction.StartPosition;
        conquerRate = faction.ConquerRate;
        conquerStrength = faction.ConquerStrength;
        expansionRate = faction.ExpansionRate;
        expansionStrength = faction.ExpansionStrength;
        expansionRange = faction.ExpansionRange;
        sharpness = faction.Sharpness;
        stability = faction.Stability;
        randomness = faction.Randomness;

        this.id = id;
    }
}
