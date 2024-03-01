using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Oscillation
{
    // system constants
    float springConstant;
    float dampingConstant;
    float mass;

    float inducedAmplitude;
    float timeSinceInduction;

    float baseAmplitude;
    float eigenAngularFrequency;
    float gamma;


    public void Initialize(float springConstant, float dampingConstant, float mass, float baseForce)
    {
        this.springConstant = springConstant;
        this.dampingConstant = dampingConstant;
        this.mass = mass;
        this.gamma = dampingConstant / (2 * mass);
        this.baseAmplitude = baseForce / springConstant;
        timeSinceInduction = 0;

        Debug.Log("gamma: " + gamma);
        Debug.Log("baseAmplitude: " + baseAmplitude);

        GetEigenAngularFrequency();
        GetAngularFrequency();
    }


    public void Update()
    {
        timeSinceInduction += Time.deltaTime;
        Debug.Log("base amplitude: "+ baseAmplitude);
        Debug.Log("induced amplitude: " + inducedAmplitude);
    }

    // without damping the amplitude is arbitrary, so we choose a amplitude for our oscillation
    private void GetEigenAngularFrequency()
    {
        eigenAngularFrequency = Mathf.Sqrt(springConstant/mass); 
    }

    private void GetAngularFrequency()
    {
        if (eigenAngularFrequency <= gamma)
        {
            Debug.LogError("Damping factor too high for regualr oscillation!");
        }
        eigenAngularFrequency = Mathf.Sqrt(eigenAngularFrequency*eigenAngularFrequency - gamma*gamma);
    }

    // constant inducedfrequency
    public void SetBaseForce(float baseForce)
    {
        baseAmplitude = baseForce/springConstant;
    }

    public void AddForce( float force)
    {
        inducedAmplitude = GetAmplitudeDecline() + force / springConstant;
        timeSinceInduction = 0;
    }

    public float GetAmplitude()
    {
        return Mathf.Clamp01(baseAmplitude + inducedAmplitude);
    }

    private float GetAmplitudeDecline()
    {
        return inducedAmplitude * Mathf.Exp(-gamma*timeSinceInduction);
    }

    //public float GetInducedAmplitude(float inducingFrequency)
    //{
    //    return (inducingForce/mass) / (Mathf.Sqrt(square(eigenAngularFrequency*eigenAngularFrequency - inducingFrequency*inducingFrequency)
    //                                              + square(2*gamma*inducingFrequency)));
    //}

    private float square(float x)
    {
        return x * x;
    }
}
