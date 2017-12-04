using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class IntRangeValue : RangeValue<int>
{
    public IntRangeValue(int _min, int _max) : base(_min, _max)
    {
    }
    public IntRangeValue(int _min, int _max,int _value) : base(_min, _max, _value)
    {
    }
}
