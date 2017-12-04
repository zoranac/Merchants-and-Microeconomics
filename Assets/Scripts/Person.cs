using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum Occupations
{
    Innkeeper,
    Monk,
    Farmer,
    Blacksmith,
    Jewler,
    Alchemist,
    Lumberjack,
    Miner,
    Quartermaster,
    Trader,
    Banker,
    Doctor,
    InfoBroker,
    Mage,
    Adventurer,
    Butcher,
    Baker,
    Fletcher,
    Woodworker
}

public class Person {
    public string Name;
    public int Bond;
    public Location Location;
    public Town Town;
    public Occupation Occupation;
    //Personallity
    public Inventory Inventory = new Inventory();
    public int Money;
    public float ProductionStatus;
    //List of Knowledge
    public List<Information> KnownInformation = new List<Information>();
    //List of Item Likes
    //List of Item Dislikes

    //PRIVATE
    private DateTime lastTimePlayerInteraction;

    public Person(Occupation _occupation, Location _location, Town _town)
    {
        Name = Random.Range(1,100).ToString();
        Occupation = _occupation;
        Location = _location;
        Town = _town;
        GenerateInventory();
        GameController.GameTime.NewHour += GameTime_NewHour;
    }

    private void GameTime_NewHour()
    {
        if (GameController.GameTime.Hour == Location.OpenTime)
        {
            //GenerateInventory();
        }
    }

    public void GenerateInventory()
    {
        if (lastTimePlayerInteraction == null)
        {
            //TODO have this be based on other factors
            Money = Random.Range(100, 500);
            //Generate Starting Money
            //If Poor
            //If Middle Class
            //If Rich

            //Generate Starting Items
            foreach (var item in Occupation.SellItems)
            {
                //TODO have this be based on other factors
                Inventory.Add(item, Random.Range(2 * item.AmountProduced, 10 * item.AmountProduced));
            }

        }
        else
        {
            //It has been over a day since you last interacted with the player or it is the next day
            if (GameController.GameTime.ToHour() - lastTimePlayerInteraction.ToHour() > 12 || 
                (GameController.GameTime.ToDay() > lastTimePlayerInteraction.ToDay()))
            {
                int numberOfDaysToSimulate = GameController.GameTime.ToDay() - lastTimePlayerInteraction.ToDay();
                int dayQuality = 0;
                for (int i = 0; i < numberOfDaysToSimulate; i++)
                {
                    GenerateInformation(DateTime.CreateFromDays(GameController.GameTime.ToDay() - numberOfDaysToSimulate + i));
                    //simulate traffic (good day vs. bad day)
                    //good day    0  - buys some items that they found on sale (less money, larger variety of sell items)
                    //            1  - sells larger amount of sell items (less stock, more money)
                    //            2  - productive day (less money, more items in stock) 
                    //
                    //normal day  3 -normal sales (normal money, normal stock)
                    //
                    //bad day     4  - low sales (less money, normal stock)
                    //            5  - unproductive day (normal money, less stock)
                    //            6  - resources wasted (less money)
                    dayQuality = Random.Range(0, 5);
                    var dayPrices = Town.ItemsPriceTimeLine.GetDataInDay(new DateTime(0, GameController.GameTime.ToDay() - numberOfDaysToSimulate + i, 0, 0))[0].prices;
                    switch (dayQuality)
                    {
                        case 0:     //Bad Day
                            {
                                int effect = Random.Range(0, 3);
                                if (effect == 0)//resources wasted
                                {
                                    Occupation.GenerateChangesForDay(this, 6, dayPrices, out Inventory, out Money, out ProductionStatus);
                                }
                                else if (effect == 1)   //Unproductive
                                {
                                    Occupation.GenerateChangesForDay(this, 5, dayPrices, out Inventory, out Money, out ProductionStatus);
                                }
                                else if (effect == 2)   //Low Sales
                                {
                                    Occupation.GenerateChangesForDay(this, 4, dayPrices, out Inventory, out Money, out ProductionStatus);
                                }
                            }
                            break;
                        case 1:     //Normal Day
                        case 2:
                        case 3:
                            {
                                Occupation.GenerateChangesForDay(this, 3, dayPrices, out Inventory, out Money, out ProductionStatus);
                            }
                            break;
                        case 4:     //Good Day
                            {
                                int effect = Random.Range(0, 3);
                                if (effect == 0)        //productive day
                                {
                                    Occupation.GenerateChangesForDay(this, 2, dayPrices, out Inventory, out Money, out ProductionStatus);
                                }
                                else if (effect == 1)   //High Sales
                                {
                                    Occupation.GenerateChangesForDay(this, 1, dayPrices, out Inventory, out Money, out ProductionStatus);
                                }
                                else if (effect == 2)   //Buy Sales
                                {
                                    Occupation.GenerateChangesForDay(this, 0, dayPrices, out Inventory, out Money, out ProductionStatus);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                   
                }
            }
        }
        lastTimePlayerInteraction = new DateTime(GameController.GameTime);
    }

    public void GenerateInformation(DateTime generatingFordate)
    {
        var random = Random.Range(0, 100);
        //Get all currently happening to location / town
        List<Event> knownEvents = new List<Event>();
        List<Event> rumouredEvents = new List<Event>();
        foreach (var item in GameController.EventTimeline.GetCurrentDataAfterDate(generatingFordate))
        {
            if (item.LocationsAffected.Contains(this.Location) || item.TownsAffected.Contains(this.Town))
            {
                knownEvents.Add(item);
            }
            else
            {
                random = Random.Range(0, 100);
                if (random < item.TypeOfEvent.CurrentKnowledgeChance.Where(x=>x.occupation.ToString() == this.Occupation.Name).ToList()[0].chance)
                {
                    knownEvents.Add(item);
                }
            }
        }
        //Get %  of future events
        foreach (var item in GameController.EventTimeline.GetFutureDataAfterDate(generatingFordate))
        {
            if (item.LocationsAffected.Contains(this.Location) || item.TownsAffected.Contains(this.Town))
            {
                random = Random.Range(0, 100);
                if (random < item.TypeOfEvent.FutureKnowledgeChance.Where(x => x.occupation.ToString() == this.Occupation.Name).ToList()[0].chance)
                {
                    knownEvents.Add(item);
                }
            }
            else
            {
                random = Random.Range(0, 100);
                if (random < item.TypeOfEvent.FutureKnowledgeChance.Where(x => x.occupation.ToString() == this.Occupation.Name).ToList()[0].chance/2)
                {
                    knownEvents.Add(item);
                }
            }
        }
        //Get chance of rumors known based on occupation and personallity
        var gossipChance = this.Occupation.BaseGossipAmount;
       
        float loops = 1;
        while (true)
        {
            var newrandom = Random.Range(0, 100);
            if (newrandom < ((float)gossipChance / loops))
            {
                //Get whether it is fake or real
                newrandom = Random.Range(0, 4);
                if (newrandom == 0)
                {
                    //real event
                    //All events that have a preemptive knowledge date and is within that date and the start date
                    var knowablefutureevents = GameController.EventTimeline.GetFutureDataAfterDate(generatingFordate)
                        .Where(x => generatingFordate.ToDay() >= x.PreemptiveKnowledgeStartDate.ToDay() && generatingFordate.ToDay() < x.StartDate.ToDay()).ToList();

                    if (knowablefutureevents.Count() > 0)
                    {
                        rumouredEvents.Add(knowablefutureevents[Random.Range(0, (int)knowablefutureevents.Count())]);
                    }
                }
                else
                {
                    //fake event
                    var e = GameController.gameController.GenerateEvent(EventType.Scales.None, DateTime.CreateFromDays(generatingFordate.ToDay() + Random.Range(5, 31)));
                    while (e.LocationsAffected.Contains(this.Location))
                    {
                        e = GameController.gameController.GenerateEvent(EventType.Scales.None, DateTime.CreateFromDays(generatingFordate.ToDay() + Random.Range(5, 31)));
                    }

                    rumouredEvents.Add(e);
                }

                loops++;
            }
            else
            {
                break;
            }
        }
        //Generate price for valid events
        foreach (var e in knownEvents)
        {
            int eventPrice = e.TypeOfEvent.BasePrice;  
            //event is not already known
            if(KnownInformation.Where(x=>x.Event == e).Count() == 0)
            {
                //Event Affects this location
                if (e.LocationsAffected.Contains(this.Location))
                {
                    //If positive for the person (increased sell price/production or decreased buy price), cost = 0;
                    if (e.TypeOfEvent.Effects.Where(x => (this.Occupation.SellItems.Contains(x.ItemAffected) && (x.BasePriceChange > 0 || x.ProductionRateChage > 0)) ||
                                                        (this.Occupation.BuyItems.Contains(x.ItemAffected) && (x.BasePriceChange < 0))).Count() > 0)
                    {
                        eventPrice = 0;
                    }
                    //if negitive for the person, cost * 100
                    if (e.TypeOfEvent.Effects.Where(x => (this.Occupation.SellItems.Contains(x.ItemAffected) && (x.BasePriceChange < 0 || x.ProductionRateChage < 0)) ||
                                                        (this.Occupation.BuyItems.Contains(x.ItemAffected) && (x.BasePriceChange > 0))).Count() > 0)
                    {
                        eventPrice = eventPrice * 100;
                    }
                }
                KnownInformation.Add(new Information(e, Information.Types.Valid, eventPrice));
            }
        }
        //generate price for gossip
        foreach (var e in rumouredEvents)
        {
            int eventPrice = (int)(e.TypeOfEvent.BasePrice * .5); //gossip costs half as much
            if (KnownInformation.Where(x => x.Event == e).Count() == 0)
            {
                KnownInformation.Add(new Information(e, Information.Types.Gossip, eventPrice));
            }
        }

        //Update information and remove very old information
        foreach (var item in KnownInformation)
        {
            if (item.UpdateInformation())
            {
                KnownInformation.Remove(item);
            }
        }
    }
}
