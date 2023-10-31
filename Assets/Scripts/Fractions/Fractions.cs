using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fractions
{
    public List<Fraction> fractions { get; private set; }
    public Dictionary<int, FractionSettings> settings { get; private set; }

    public void Initialize(List<FractionSettings> settings)
    {
        fractions = new List<Fraction>();
        this.settings = new Dictionary<int, FractionSettings>();
        for (int i = 1; i <= settings.Count; i++)
        {
            fractions.Add(new Fraction(i, settings[i-1]));
            this.settings.Add(i, settings[i-1]);
        }
       
    }

    public void Update()
    {
        foreach (Fraction fraction in fractions)
        {
            fraction.Update();
        }
    }

    public List<FractionState> GetStates()
    {
        List < FractionState > states = new List<FractionState >();
        foreach (Fraction fraction in fractions)
        {
            states.Add(fraction.state);
        }
        return states;
    }

    
}

