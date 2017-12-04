using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class IntRange : Range<int> {
    public IntRange(int _min, int _max) : base(_min, _max)
    {
    }
}
