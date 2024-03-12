using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dendry
{
    [SerializeField, Range(1,6)]
    int nIterations;
    int seed;
    [SerializeField]
    Vector2 limit;
    [SerializeField, Range (0,1)] // can vary for each iter
    float[] displacement;
}
