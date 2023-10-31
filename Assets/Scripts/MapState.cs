using System.Collections.Generic;
using UnityEngine;
public class MapState
{
    public Color[] colorMap { get; private set; }
    

    public void Initialize(Color[] colorMap)
    {
        this.colorMap = colorMap;
    }

    public void Update()
    {
        
    }
}