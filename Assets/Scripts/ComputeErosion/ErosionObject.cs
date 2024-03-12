using UnityEngine;


[CreateAssetMenu(fileName = "ErosionSettingsObject", menuName = "Factions/Create Erosion settings")]
public class ErosionObject : ScriptableObject
{
    [SerializeField]
    Color color;
    public Color Color { get => color; }
    [SerializeField, Range(0, 1f)]
    float speed;
    public float Speed { get => speed; }
    [SerializeField, Range(0, 1f)]
    float slopeFactor;
    public float SlopeFactor { get => slopeFactor; }
    [SerializeField, Range(0, 0.01f)]
    float initValueDrop;
    public float InitValueDrop { get => initValueDrop; }
}
