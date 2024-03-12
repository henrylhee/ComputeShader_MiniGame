using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionDataManager
{
    FactionDataConst[] factionDataConstant;
    FactionDataDyn[] factionDataDynamic;

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
        factionDataConstant = new FactionDataConst[factionCount];
        factionDataConstant[0] = new FactionDataConst();

        factionDataDynamic = new FactionDataDyn[factionCount];
        factionDataDynamic[0] = new FactionDataDyn();

        for (int i = 1; i < factionCount; i++)
        {
            factionDataConstant[i] = new FactionDataConst();
            factionDataConstant[i].conquerRate = settings[i].ConquerRate;
            factionDataConstant[i].dummy = settings[i].ConquerStrength;
            factionDataConstant[i].expansionRate = settings[i].ExpansionRate;
            factionDataConstant[i].expansionStrength = settings[i].ExpansionStrength;
            factionDataConstant[i].color = settings[i].Color;

            factionDataDynamic[i] = new FactionDataDyn();
            factionDataDynamic[i].conquerStrength = settings[i].ConquerStrength;
            factionDataDynamic[i].dummy1 = 0;
            factionDataDynamic[i].dummy2 = 0;
            factionDataDynamic[i].dummy3 = 0;
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
        accumulatedForce = Mathf.Clamp01(accumulatedForce - accumulatedForceDecrease);
        foreach (Oscillation oscillation in oscillations)
        {
            oscillation.Update();
        }

        for (int i = 2; i < factionCount; i++)
        {
            factionDataDynamic[i].conquerStrength = oscillations[i - 2].GetAmplitude();
        }
    }

    public FactionDataConst[] GetFactionDataConstant()
    {
        return factionDataConstant;
    }

    public FactionDataDyn[] GetFactionDataDynamic()
    {
        return factionDataDynamic;
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
