using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Event : ScriptableObject {
    public EventType TypeOfEvent;
    public List<Town> TownsAffected = new List<Town>();
    public List<Location> LocationsAffected = new List<Location>();
    public DateTime StartDate;
    public DateTime EndDate;
    public DateTime PreemptiveKnowledgeStartDate;

    public void Init(EventType typeofEvent, DateTime startDate = null, Location location = null, Town town = null)
    {
        TypeOfEvent = typeofEvent;
        switch (TypeOfEvent.Scale)
        {
            //TODO Make this based on regions, so it's not quite so random
            case EventType.Scales.Global:
                {
                    foreach (var item in GameController.AllTowns)
                    {
                        TownsAffected.Add(item);
                    }
                    foreach (var item in GameController.AllLocations)
                    {
                        LocationsAffected.Add(item);
                    }
                }
                break;
            case EventType.Scales.Towns:
                {
                    //not implemented
                }
                break;
            case EventType.Scales.Town:
                {
                    if (town != null)
                    {
                        TownsAffected.Add(town);
                    }
                    else
                    {
                        var t = GameController.AllTowns
                            .Where(x => TypeOfEvent.TownTypesAffected
                            .Any(y => x.Type.ToString() == y.ToString()))
                            .ToArray();
                        //Select a random town from a list of towns that can be affected
                        TownsAffected.Add(t[UnityEngine.Random.Range(0, t.Length)]);
                    }
                }
                break;
            case EventType.Scales.Locations:
                {
                    //not implemented
                }
                break;
            case EventType.Scales.Location:
                {
                    if (location != null)
                    {
                        LocationsAffected.Add(location);
                    }
                    else
                    {
                        //Select a random location from a list of locations that can be affected
                        var l = GameController.AllLocations
                            .Where(x => TypeOfEvent.LocationTypesAffected
                            .Any(y => x.Type.Name == y.Name))
                            .ToArray();
                        LocationsAffected.Add(l[UnityEngine.Random.Range(0, l.Length)]);
                    }
                }
                break;
            default:
                break;
        }
        //If a start date is set, use as start date, otherwise generate a date within the next 30 days
        if (startDate != null)
        {
            StartDate = new DateTime(startDate);
            EndDate = DateTime.CreateFromDays(StartDate.ToDay() + UnityEngine.Random.Range(TypeOfEvent.MinDuration, TypeOfEvent.MaxDuration));
        }
        else
        {
            StartDate = DateTime.CreateFromDays(UnityEngine.Random.Range(GameController.GameTime.ToDay(), GameController.GameTime.ToDay() + 30));
            EndDate = DateTime.CreateFromDays(StartDate.ToDay() + UnityEngine.Random.Range(TypeOfEvent.MinDuration, TypeOfEvent.MaxDuration));
        }

        if(StartDate.ToDay() - TypeOfEvent.PreemptiveKnowledgeDuration <= 0)
        {
            PreemptiveKnowledgeStartDate = new DateTime();
        }
        else
        {
            PreemptiveKnowledgeStartDate = DateTime.CreateFromDays(StartDate.ToDay() - TypeOfEvent.PreemptiveKnowledgeDuration);
        }
    }
}
