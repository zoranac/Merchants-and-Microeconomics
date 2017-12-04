using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeValue<T> : Range<T> where T : IComparable<T>,
                                IComparable,
                                IConvertible,
                                IEquatable<T>,
                                IFormattable
{
    [NonSerialized]
    public T Value;
    /// <summary>
    /// Set the value within a range.
    /// </summary>
    /// <param name="_min">min value</param>
    /// <param name="_max">max value</param>
    /// <param name="_value">the value you wish to set</param>
    public RangeValue(T _min, T _max, T _value) : base(_min, _max)
    {
        Value = Clamp(_value);
    }
    /// <summary>
    /// Set Value to a random value in range.
    /// </summary>
    /// <param name="_min">min value</param>
    /// <param name="_max">max value</param>
    public RangeValue(T _min, T _max) : base(_min, _max)
    {
        Value = RandomValueInRange();
    }
}
