using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public delegate void TickHandler();

public class GameController : MonoBehaviour {

    public static GameController gameController;

    public static Clock GameTime = new Clock();
    public List<Item> ItemTypes = new List<Item>();
    public List<Occupation> OccupationTypes = new List<Occupation>();
    public UIController uiController;
    public static bool inGame = true;
    public static bool isPaused = false;
    public static event TickHandler Tick;

    public static Timeline<Event> EventTimeline = new Timeline<Event>();

    public static List<Occupation> AllOccupations = new List<Occupation>();
    public static List<EventType> AllEventTypes = new List<EventType>();
    public static List<Item> AllItemTypes = new List<Item>();
    public static List<TownType> AllTownTypes = new List<TownType>();
    public static List<LocationType> AllLocationTypes = new List<LocationType>();
    public static List<Town> AllTowns = new List<Town>();
    public static List<Location> AllLocations = new List<Location>();

    public static Town CurrentTown;
    public static Location CurrentLocation;

    public void TEST()
    {
        SceneManager.LoadScene(1);
    }

    void Awake()
    {
        if (!gameController)
        {
            gameController = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
        foreach (var item in ItemTypes)
        {
            AllItemTypes.Add(item);
        }
        foreach (var item in OccupationTypes)
        {
            AllOccupations.Add(item);
        }

        DeserializeClasses();

        GenerateTowns();

        GameTime.NewHour += NewHour;
        GameTime.NewDay += NewDay;
        GameTime.NewSeason += NewSeason;
        GameTime.NewYear += NewYear;

        uiController = GetComponent<UIController>();

        uiController.GenerateTownUI(CurrentTown);

        GenerateOccuringEvents(60, new DateTime(0, 0, 1, 1));
    }

    public void DeserializeClasses()
    {
        //LOCATIONS
        var locationsInfo = new DirectoryInfo(Application.streamingAssetsPath + "\\Json\\Locations\\");
        foreach (var item in locationsInfo.GetFiles())
        {
            if (item.Name.EndsWith(".json") && !item.Name.Contains("schema"))
            {
                string filename = item.DirectoryName + "\\" + item.Name;
                var a = JsonUtility.FromJson<FromJsonLocation>(File.ReadAllText(filename));
                AllLocationTypes.Add(new LocationType(a));
            }
        }
        //TOWNS
        var townsInfo = new DirectoryInfo(Application.streamingAssetsPath + "\\Json\\Towns\\");
        foreach (var item in townsInfo.GetFiles())
        {
            if (item.Name.EndsWith(".json") && !item.Name.Contains("schema"))
            {
                string filename = item.DirectoryName + "\\" + item.Name;
                var a = JsonUtility.FromJson<FromJsonTown>(File.ReadAllText(filename));
                AllTownTypes.Add(new TownType(a));
            }
        }
        //EVENT TYPES
        var eventTypesInfo = new DirectoryInfo(Application.streamingAssetsPath + "\\Json\\EventTypes\\");
        foreach (var item in eventTypesInfo.GetFiles())
        {
            if (item.Name.EndsWith(".json") && !item.Name.Contains("schema"))
            {
                string filename = item.DirectoryName + "\\" + item.Name;
                var a = JsonUtility.FromJson<FromJsonEventType>(File.ReadAllText(filename));
                AllEventTypes.Add(new EventType(a));
            }
        }
    }

    public void GenerateTowns()
    {
        foreach (var item in AllTownTypes)
        {
            AllTowns.Add(new Town(item));
        }

        //TEMP
        CurrentTown = AllTowns[0];
    }

    //Generate town events (everyday has a 0+x days in 100 chance)
    //Generate Location events (everyday has a 0+x days in 100 chance)
    public void GenerateOccuringEvents(int numberOfDays, DateTime startDate)
    {
        foreach (var town in AllTowns)
        {
            int daysWithoutEvent = 0;
            for (int i = 0; i < numberOfDays; i++)
            {
                //Generate Town Base Events
                if (UnityEngine.Random.Range(0, 100 - daysWithoutEvent) == 0)
                {
                    var e = GenerateEvent(EventType.Scales.Town, DateTime.CreateFromDays(startDate.ToDay() + i), town, null);
                    EventTimeline.Insert(new DateTime(e.StartDate), new DateTime(e.EndDate), e);
                }
            }
            foreach (var location in town.Locations)
            {
                daysWithoutEvent = 0;
                //Genrate location based events
                for (int i = 0; i < numberOfDays; i++)
                {
                    //Generate Town Base Events
                    if (UnityEngine.Random.Range(0, 100 - daysWithoutEvent) == 0)
                    {
                        var e = GenerateEvent(EventType.Scales.Location, DateTime.CreateFromDays(startDate.ToDay() + i), null, location);
                        if (e == null)
                        {
                            break;
                        }
                        EventTimeline.Insert(new DateTime(e.StartDate), new DateTime(e.EndDate), e);
                    }
                }
            }
        }
    }

    public Event GenerateEvent(EventType.Scales eventType = EventType.Scales.None, DateTime startDate = null, Town town = null, Location location = null)
    {
        //Any scale of event
        Event ev = ScriptableObject.CreateInstance<Event>();
        if (eventType == EventType.Scales.None)
        {
            //Get an event that can be known before it happens. This is for fake events only.
            var list = AllEventTypes.Where(x => x.FutureKnowledgeChance.Where(y => y.chance > 0).Count() > 0).ToList();
            ev.Init(list[UnityEngine.Random.Range(0, list.Count)]);
        }
        //Town Scale Event
        else if (town != null)
        {
            if (location != null)
            {
                var list = AllEventTypes.Where(x =>
                    x.Scale == eventType &&
                    x.TownTypesAffected.Contains(town.Type.Type) &&
                     x.LocationTypesAffected.Where(y => y.Name == location.Type.Name).Count() > 0
                    ).ToList();
                ev.Init(list[UnityEngine.Random.Range(0, list.Count)], startDate, location, town);
            }
            else
            {
                var list = AllEventTypes.Where(x =>
                  x.Scale == eventType &&
                  x.TownTypesAffected.Contains(town.Type.Type)
                  ).ToList();
                ev.Init(list[UnityEngine.Random.Range(0, list.Count)], startDate, null, town);
            }
        }
        //Location Scale Event
        else if (location != null)
        {
            var list = AllEventTypes.Where(x =>
               x.Scale == eventType &&
               x.LocationTypesAffected.Where(y => y.Name == location.Type.Name).Count() > 0
               ).ToList();

            if (list.Count <= 0)
            {
                return null;
            }

            ev.Init(list[UnityEngine.Random.Range(0, list.Count)], startDate, location);
        }

        return ev;
    } 


    public void PassTime(int hoursPassed)
    {
        for (int i = 0; i < hoursPassed; i++)
        {
            Tick();
        }
    }

    void NewHour()
    {

    }

    void NewDay()
    {
        var text = "";
        EventTimeline.GetDataInDay(GameTime).ForEach(x => text += x.TypeOfEvent.Name + " ");
        GameObject.Find("EventNameText").GetComponent<Text>().text = text;
    }

    void NewSeason()
    {
        GenerateOccuringEvents(30, new DateTime(0, GameTime.ToDay() + 30, 0, 0));
    }

    void NewYear()
    {

    }
}
