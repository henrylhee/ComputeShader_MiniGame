using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rasterization
{
    public static void GetPixelsInCircle(int centerX, int centerY, int radius)
    {
        int diameter = radius * 2 + 1;

        byte[,] buffer = new byte[diameter, diameter];
        circle(buffer, 25, 25, 20);

        for (int y = 0; y < 50; ++y)
        {
            for (int x = 0; x < 50; ++x)
                Debug.Log(buffer[y, x].ToString());

            Debug.Log("");
        }
    }

    static void circle(byte[,] buffer, int cx, int cy, int radius)
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
                    plot4points(buffer, cx, cy, lastY, x);

                error -= x;
                --x;
                error -= x;
            }
        }
    }

    static void plot4points(byte[,] buffer, int cx, int cy, int x, int y)
    {
        horizontalLine(buffer, cx - x, cy + y, cx + x);
        if (y != 0)
            horizontalLine(buffer, cx - x, cy - y, cx + x);
    }

    static void setPixel(byte[,] buffer, int x, int y)
    {
        buffer[y, x]++;
    }

    static void horizontalLine(byte[,] buffer, int x0, int y0, int x1)
    {
        for (int x = x0; x <= x1; ++x)
            setPixel(buffer, x, y0);
    }
}
