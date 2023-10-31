using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class FractionState
{
    public Color color {  get; private set; }
    public List<Point> inactivePoints { get; private set; }
    public List<Point> activePoints {  get; private set; }
    public List<Point> newPoints { get; private set; }


    public void Initialize(Color color, List<Point> start)
    {
        this.color = color;
        this.activePoints = start;
    }

    public void Update(Color color, List<Point> inactivePoints, List<Point> activePoints, List<Point> newPoints)
    {
        this.color = color;
        this.inactivePoints = inactivePoints;
        this.activePoints = activePoints;
        this.newPoints = newPoints;
    }
}