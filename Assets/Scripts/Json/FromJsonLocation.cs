using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FromJsonLocation {

    public FromJsonLocation()
    {

    }
    public string Name;
    public string OwnerOccupation;
    public float DiscoveryChanceMultiplier;
    public int BaseOpenTimeMin;
    public int BaseOpenTimeMax;
    public int BaseCloseTimeMin;
    public int BaseCloseTimeMax;
}
