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
    private float brightnessInputIncrease = 0.0001f;
    public float BrightnessInputIncrease { get { return brightnessInputIncrease; } }
    [SerializeField]
    private float neutralTimeInterval = 3f;
    public float NeutralTimeInterval { get { return neutralTimeInterval; } }
    [SerializeField]
    private float accumulatedForceDecrease = 3f;
    public float AccumulatedForceDecrease { get { return accumulatedForceDecrease; } }
    [SerializeField]
    private float springConstant = 3f;
    public float SpringConstant { get { return springConstant; } }
    [SerializeField]
    private float dampingConstant = 3f;
    public float DampingConstant { get { return dampingConstant; } }
    [SerializeField]
    private float mass = 3f;
    public float Mass { get { return mass; } }
}
