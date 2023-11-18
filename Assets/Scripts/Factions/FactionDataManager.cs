using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionDataManager
{
    FactionData[] factionData;
    public int factionCount;

    List<Oscillation> oscillations;

    float lastInjectionTime;
    float neutralTimeInterval;

    float accumulatedForce;
    float accumulatedForceDecrease;


    public void Initialize()
    {
        SetFactionData();

        neutralTimeInterval = GlobalSettings.Instance.gameSettings.NeutralTimeInterval;
        accumulatedForceDecrease = GlobalSettings.Instance.gameSettings.AccumulatedForceDecrease;
        accumulatedForce = 0;
        lastInjectionTime = Time.time;
        

        List<FactionSettings> factionSettings = GlobalSettings.Instance.factionSettings;
        
    }

    public void SetFactionData()
    {
        List<FactionSettings> settings = GlobalSettings.Instance.factionSettings;
        factionCount = settings.Count;
        factionData = new FactionData[factionCount];
        factionData[0] = new FactionData();
        for (int i = 1; i < factionCount; i++)
        {
            factionData[i] = new FactionData();
            factionData[i].conquerRate = settings[i].ConquerRate;
            factionData[i].conquerStrength = settings[i].ConquerStrength;
            factionData[i].expansionRate = settings[i].ExpansionRate;
            factionData[i].expansionStrength = settings[i].ExpansionStrength;
            factionData[i].color = settings[i].Color;
        }

        float springConstant = GlobalSettings.Instance.gameSettings.SpringConstant;
        float dampingConstant = GlobalSettings.Instance.gameSettings.DampingConstant;
        float mass = GlobalSettings.Instance.gameSettings.Mass;
        oscillations = new List<Oscillation>();
        for (int i = 2; i < factionCount; i++)
        {
            Oscillation oscillation = new Oscillation();
            oscillation.Initialize(springConstant, dampingConstant, mass, settings[i].ConquerStrength);
            oscillations.Add(oscillation);
        }
    }

    public void Update()
    {
        foreach (Oscillation oscillation in oscillations)
        {
            oscillation.Update();
        }

        for (int i = 2; i < factionData.Length; i++)
        {
            factionData[i].conquerStrength = oscillations[i - 2].GetAmplitude();
        }
    }

    public FactionData[] GetFactionData()
    {
        return factionData;
    }

    public void PixelsInjected()
    {
        foreach (Oscillation oscillation in oscillations)
        {
            oscillation.AddForce(GetForce());
        }
        lastInjectionTime = Time.time;
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
