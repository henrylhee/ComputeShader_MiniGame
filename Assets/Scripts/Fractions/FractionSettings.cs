using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefineFraction", menuName = "Fractions")]
public class FractionSettings : ScriptableObject
{
    [SerializeField]
    string _name;
    public string _Name { get => _name; }
    [SerializeField]
    Color color;
    [SerializeField]
    Vector2Int startPosition;
    public Vector2Int StartPosition { get => startPosition; }
    public Color Color { get => color; }
    [SerializeField]
    float conquerRate;
    public float ConquerRate { get => conquerRate; }
    [SerializeField, Range(0f,1f)]
    float expansionRate;
    public float ExpansionRate { get => expansionRate;}
    [SerializeField, Range(0f, 1f)]
    float expansionRange;
    public float ExpansionRange { get => expansionRange; }
    [SerializeField]
    float sharpness;
    public float Sharpness { get => sharpness;}
    [SerializeField]
    float stability;
    public float Stability { get => stability;}
    //[SerializeField, Range(0f,1f)]
    //float splitChance;
    //public float SplitChance { get => splitChance;}
    [SerializeField]
    float randomness;
    public float Randomness { get => randomness;}
}
