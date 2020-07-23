using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public Vector2 Center => new Vector2((MinX + MaxX) / 2f, (MinY + MaxY) / 2f);
    public Vector2 Bounds => new Vector2(Mathf.Abs(MaxX - MinX), Mathf.Abs(MaxY - MinY));
    public Vector2 Extends => Bounds / 2f;

    public float MinX = float.MaxValue;
    public float MaxX = float.MinValue;
    public float MinY = float.MaxValue;
    public float MaxY = float.MinValue;

    public Vector2 Normalize(Vector2 value)
    {
        value.x = NormalizeX(value.x);
        value.y = NormalizeY(value.y);
        return value;
    }

    public float NormalizeX(float value)
    {
        return (value - MinX) / (MaxX - MinX);
    }

    public float NormalizeY(float value)
    {
        return (value - MinY) / (MaxY - MinY);
    }

    public void CheckXY(Vector2 pos)
    {
        CheckX(pos.x);
        CheckY(pos.y);
    }

    public void CheckX(float value)
    {
        if (value < MinX)
            MinX = value;
        if (value > MaxX)
            MaxX = value;
    }

    public void CheckY(float value)
    {
        if (value < MinY)
            MinY = value;
        if (value > MaxY)
            MaxY = value;
    }
}
