using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Line(float m, float b)
    {
        this.m = m;
        this.b = b;
    }

    public Line(float x1, float y1, float x2, float y2)
    {
        m = (y1 - y2) / (x1 - x2);
        b = y1 - m * x1;
    }

    public Line(Vector2 input, Vector2 output) : this(input.x, output.x, input.y, output.y){}

    float m;
    float b;

    public float EvaluateAt(float x)
    {
        return m * x + b;
    }
}
