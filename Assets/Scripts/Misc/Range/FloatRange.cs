using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatRange : Range<float>
{
    public FloatRange(float _min, float _max) : base(_min, _max)
    {
    }
}
