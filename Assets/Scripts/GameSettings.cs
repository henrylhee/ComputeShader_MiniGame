using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameSettings : MonoBehaviour
{
    [SerializeField] 
    private float speed = 1;
    public float Speed { get { return speed; } }
    [SerializeField]
    private float brightnessMin = 0.5f;
    public float BrightnessMin { get { return brightnessMin; } }
    [SerializeField]
    private float brightnessInputIncrease = 0.001f;
    public float BrightnessInputIncrease { get { return brightnessInputIncrease; } }
}
