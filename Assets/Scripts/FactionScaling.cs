using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionScaling
{
    List<Oscillation> oscillations;

    float lastInjectionTime;
    float neutralTimeInterval;

    float accumulatedForceDecrease;
    public float accumulatedForce;


    public void Initialize()
    {
        neutralTimeInterval = GlobalSettings.Instance.gameSettings.NeutralTimeInterval;
        accumulatedForceDecrease = GlobalSettings.Instance.gameSettings.AccumulatedForceDecrease;
        accumulatedForce = 0;
        lastInjectionTime = Time.time;
        float springConstant = GlobalSettings.Instance.gameSettings.SpringConstant;
        float dampingConstant = GlobalSettings.Instance.gameSettings.DampingConstant;
        float mass = GlobalSettings.Instance.gameSettings.Mass;

        List<FactionSettings> factionSettings = GlobalSettings.Instance.factionSettings;
        for ( int i = 2; i < factionSettings.Count-2; i++)
        {
            Oscillation oscillation = new Oscillation();
            oscillation.Initialize(springConstant, dampingConstant, mass, factionSettings[i].ConquerStrength);
            oscillations.Add(oscillation);
        }
    }

    public List<float> OnInjectPixels()
    {
        List<float> result = new List<float>();
        foreach (Oscillation oscillation in oscillations)
        {
            oscillation.AddForce(GetForce());
            result.Add(oscillation.GetAmplitude());
        }
        lastInjectionTime = Time.time;
        return result;
    }

    public void Update()
    {
        accumulatedForce = Mathf.Clamp(accumulatedForce - accumulatedForceDecrease, 0, 1);
        foreach (Oscillation oscillation in oscillations)
        {
            oscillation.Update();
        }
    }

    private float GetForce()
    {
        float timeInterval = Time.time - lastInjectionTime;
        if (timeInterval < neutralTimeInterval)
        {
            accumulatedForce += Mathf.Clamp((neutralTimeInterval - timeInterval)/100,0,1);
        }
        return accumulatedForce;
    }
}
