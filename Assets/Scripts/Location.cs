using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[System.Serializable]
public class Location {
    public string Name;
    public LocationType Type;
    public Person Owner;
    public Town Town;
    public bool Discovered;
    internal int OpenTime;
    internal int CloseTime;
    public bool IsOpen;

    public void InstantiateOwner()
    {
        Owner = new Person(Type.OwnerOccupation,this,Town);
    }

    public void GenerateName()
    {
        Name = "Joe's " + Type.Name;
    }

    public Location(Town mytown, LocationType type)
    {
        //TEMP
        Discovered = true;

        Type = type;
        Town = mytown;
        GenerateName();
        OpenTime = Type.BaseOpenTime.RandomValueInRange();
        CloseTime = Type.BaseCloseTime.RandomValueInRange();
        InstantiateOwner();
        GameController.GameTime.NewHour += GameTime_NewHour;
       // GameController.GameTime.NewDay += GameTime_NewDay;
    }

    public void DisplayLocation()
    {
        Owner.GenerateInventory();
        Debug.Log("You went to: " + Name + ". They have " + Owner.Money + " Money.");
        foreach (var item in Owner.KnownInformation)
        {
            Debug.Log("Info: " + item.Event.TypeOfEvent.Name + ", Price: " + item.Price + ", TimeState: " + item.TimeState + ", Type: " + item.Type);
        }
    }


    private void GameTime_NewDay()
    {
        throw new System.NotImplementedException();
    }

    public void GameTime_NewHour()
    {
        if (GameController.GameTime.Hour == OpenTime)
        {
            IsOpen = true;
        }
        else if (GameController.GameTime.Hour == CloseTime){
            IsOpen = false;
        }
    }
}
