using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FromJsonTown {

    [Serializable]
    public class LocationsInTown
    {
        public string LocationType;
        public int Amount;
    }

    public FromJsonTown()
    {

    }

    public string Name;
    public string Type;
    public string Size;
    public LocationsInTown[] RequiredLocations;
    public string[] OptionalLocations;
    public int MaxLocations;

}
