using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rasterization
{
    public static List<int[]> GetPointsInCircle(int centerX, int centerY, int radius)
    {
        int diameter = radius * 2 + 1;

        List<int[]> buffer = new List<int[]>();
        circle(buffer, centerX, centerY, radius);
        return buffer;
    }

    static void circle(List<int[]> buffer, int cx, int cy, int radius)
    {
        int error = -radius;
        int x = radius;
        int y = 0;

        while (x >= y)
        {
            int lastY = y;

            error += y;
            ++y;
            error += y;

            plot4points(buffer, cx, cy, x, lastY);

            if (error >= 0)
            {
                if (x != lastY)
                {
                    plot4points(buffer, cx, cy, lastY, x);
                }

                error -= x;
                --x;
                error -= x;
            }
        }
    }

    static void plot4points(List<int[]> buffer, int cx, int cy, int x, int y)
    {
        horizontalLine(buffer, cx - x, cy + y, cx + x);
        if (y != 0)
        {
            horizontalLine(buffer, cx - x, cy - y, cx + x);
        }
    }

    static void horizontalLine(List<int[]> buffer, int x0, int y0, int x1)
    {
        for (int x = x0; x <= x1; ++x)
        {
            buffer.Add(new int[] {x, y0});
        }   
    }
}
