using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TownType {

    public enum Types
    {
        Village,
        City,
        None
    }
    public enum Sizes
    {
        Small,
        Medium,
        Large
    }

    public class LocationInTown
    {
        public LocationInTown(LocationType type, int amount)
        {
            locationType = type;
            Amount = amount;
        }

        public LocationType locationType;
        public int Amount;
    }

    public TownType(FromJsonTown fromJson)
    {
        Name = fromJson.Name;
        Type = (Types)Enum.Parse(typeof(Types), fromJson.Type);
        Size = (Sizes)Enum.Parse(typeof(Sizes), fromJson.Size);
        foreach (var item in fromJson.RequiredLocations)
        {
            var i = GameController.AllLocationTypes.Where(x => x.Name == item.LocationType).First();
            RequiredLocations.Add(new LocationInTown(i, item.Amount));
        }
        foreach (var item in fromJson.OptionalLocations)
        {
            var i = GameController.AllLocationTypes.Where(x => x.Name == item).First();
            OptionalLocations.Add(i);
        }
        MinLocations = RequiredLocations.Count();
        MaxLocations = fromJson.MaxLocations;
    }

    public string Name;
    public Types Type;
    public Sizes Size;
    public List<LocationInTown> RequiredLocations = new List<LocationInTown>();
    public List<LocationType> OptionalLocations = new List<LocationType>();
    public int MinLocations;
    public int MaxLocations;
}
