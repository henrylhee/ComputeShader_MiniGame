using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    List<Point> points;
    List<Vector2Int> positions;

    public Line(List<Point> points)
    {
        this.points = points;
        foreach (var point in points)
        {
            positions.Add(point.position);
        }
    }
}
