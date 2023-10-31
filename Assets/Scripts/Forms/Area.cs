using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    List<Point> points;
    List<Vector2Int> positions;

    public Area(List<Point> points)
    {
        this.points = points;
        foreach (var point in points)
        {
            positions.Add(point.position);
        }
    }
}
