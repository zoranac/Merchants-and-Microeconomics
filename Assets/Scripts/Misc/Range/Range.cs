using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

[Serializable]
public class Range<T> : object where T : IComparable<T>,
                                IComparable,
                                IConvertible,
                                IEquatable<T>,
                                IFormattable
{
    [SerializeField]
    private T max;
    [SerializeField]
    private T min;

    public Range(T _min, T _max)
    {
        if (max.CompareTo(min) < 0)
        {
            throw new System.Exception("The Min value cannot be greater than the Max Value");
        }
        min = _min;
        max = _max;
    }

    public T Max
    {
        get { return max; }
        set
        {
            if (value.CompareTo(min) < 0)
            {
                throw new System.Exception("The Max value cannot be less than the Min Value");
            }
            max = value;
        }
    }
    public T Min
    {
        get { return min; }
        set
        {
            if (max.CompareTo(value) < 0)
            {
                throw new System.Exception("The Min value cannot be greater than the Max Value");
            }
            min = value;
        }
    }

    public T Clamp(T value)
    {

        if (value.CompareTo(min) < 0)
        {
            value = min;
        }
        else if (value.CompareTo(max) > 0)
        {
            value = max;
        }
        return value;
    }
    //Test if within the range
    public bool WithinRange(T value)
    {
        if (value.CompareTo(min) > 0 && value.CompareTo(max) < 0)
        {
            return true;
        }
        return false;
    }
    //Test if outside of the range
    public bool OutsideOfRange(T value)
    {
        if (value.CompareTo(min) < 0 && value.CompareTo(max) > 0)
        {
            return true;
        }
        return false;
    }
    //test if range is encompassed by the range
    public bool EncompassedByRange(Range<T> range)
    {
        if (range.Min.CompareTo(min) < 0 && range.Max.CompareTo(Max) > 0)
        {
            return true;
        }
        return false;
    }
    //Test if the range is encompassed by another range
    public bool EncompassesRange(Range<T> range)
    {
        if (range.Min.CompareTo(min) > 0 && range.Max.CompareTo(Max) < 0)
        {
            return true;
        }
        return false;
    }
    public T RandomValueInRange()
    {
        if (typeof(T) != typeof(float))
        {
            var v = (IConvertible)UnityEngine.Random.Range(min.ToInt32(CultureInfo.CurrentCulture), max.ToInt32(CultureInfo.CurrentCulture));
            return (T)v.ToType(typeof(T), CultureInfo.CurrentCulture);
        }
        else
        {
            var v = (IConvertible)UnityEngine.Random.Range(min.ToSingle(CultureInfo.CurrentCulture), max.ToSingle(CultureInfo.CurrentCulture));
            return (T)v.ToType(typeof(T), CultureInfo.CurrentCulture);
        }
    }
    public void OnGUI()
    {
        if ((typeof(T) == typeof(float)))
        {
            var v = (IConvertible)EditorGUILayout.FloatField(Max.ToSingle(CultureInfo.CurrentCulture));
            Max = (T)v.ToType(typeof(T), CultureInfo.CurrentCulture);
        }
        if ((typeof(T) == typeof(int)))
        {
            var v = (IConvertible)EditorGUILayout.IntField(Max.ToInt32(CultureInfo.CurrentCulture));
            Max = (T)v.ToType(typeof(T), CultureInfo.CurrentCulture);
        }
    }
}

