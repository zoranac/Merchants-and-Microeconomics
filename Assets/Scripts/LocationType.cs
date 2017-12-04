using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationType {

    public LocationType(FromJsonLocation fromJson)
    {
        Name = fromJson.Name;
        OwnerOccupation = GameController.AllOccupations.Where(x => x.Name == fromJson.OwnerOccupation).First();
        DiscoveryChanceMultiplier = fromJson.DiscoveryChanceMultiplier;
        BaseOpenTime = new IntRange(fromJson.BaseOpenTimeMin, fromJson.BaseOpenTimeMax);
        BaseCloseTime = new IntRange(fromJson.BaseCloseTimeMin, fromJson.BaseCloseTimeMax);
    }

    public string Name;
    public Occupation OwnerOccupation;
    public float DiscoveryChanceMultiplier;
    public IntRange BaseOpenTime;
    public IntRange BaseCloseTime;
}
