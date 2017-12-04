using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class FloatRangeValue : RangeValue<float> {

    public FloatRangeValue(float _min, float _max) : base(_min, _max)
    {
    }
    public FloatRangeValue(float _min, float _max, float _value) : base(_min, _max, _value)
    {
    }
}
