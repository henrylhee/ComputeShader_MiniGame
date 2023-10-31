using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fraction
{
    readonly int id;
    readonly FractionSettings settings;

    public FractionState state { get; private set; }


    public Fraction(int id, FractionSettings fractionSettings)
    {
        this.id = id;
        settings = fractionSettings;
    }

    public void Update()
    {
        
    }
}
