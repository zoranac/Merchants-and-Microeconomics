using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Town {

    public class Price
    {
        public Item item;
        public int price;
        public int HighDemandChance = 50;
        public Price(Item _item, int _price)
        {
            item = _item;
            price = _price;
        }
    }
    public class PriceTimeline
    {
        public List<Price> prices;
        public List<Event> events;
        public PriceTimeline(List<Price> _prices, List<Event> _events)
        {
            prices = _prices;
            events = _events;
        }
    }

    public string Name;
    public TownType Type;
    //[HideInInspector]
    public List<Location> Locations = new List<Location>();
    //public List<Event> TownEvents = new List<Event>();
    public Timeline<PriceTimeline> ItemsPriceTimeLine = new Timeline<PriceTimeline>();
    public List<Price> ItemPrices = new List<Price>();

    public Town(TownType type)
    {
        Type = type;

        if (Type.MinLocations < Type.RequiredLocations.Count)
            Type.MinLocations = Type.RequiredLocations.Count;
        if (Type.MaxLocations < Type.RequiredLocations.Count)
            Type.MaxLocations = Type.RequiredLocations.Count;

        if (Locations.Count <= 0)
        {
            GenerateTown();
            GeneratePrices();
        }

        GameController.GameTime.NewDay += GeneratePrices;
    }

    public void GeneratePrices()
    {
        //Get Events for this day that occur within this town
        List<Event> _events = GameController.EventTimeline.GetDataInDay(GameController.GameTime)
                        .Where( x => x.TownsAffected.Contains(this) || 
                                x.LocationsAffected.Where(y=>y.Town == this).Count() > 0)
                        .ToList();

        //if itemprices is empty, add prices at base price.
        if (ItemPrices.Count <= 0)
        {
            foreach (var item in GameController.AllItemTypes)
            {
                ItemPrices.Add(new Price(item, item.BasePrice));
            }
        }
        //then adjust based items' max price adjustment, and events affecting the town.
        for (int i = 0; i < ItemPrices.Count; i++)
        {
            var itemPriceEventChange = 0; //Base price change from events
            var wantChanceChange = 0;
            foreach (var e in _events)
            {
                if (e.TownsAffected.Contains(this) && e.StartDate == GameController.GameTime)
                {
                    foreach (var effect in e.TypeOfEvent.Effects)
                    {
                        if (effect.ItemAffected.Name == ItemPrices[i].item.Name) {
                            itemPriceEventChange += effect.BasePriceChange;
                            wantChanceChange += effect.WantChanceChange;
                        }
                    }
                }
            }
            
            //////This is subject to change!!!!!
            ItemPrices[i].HighDemandChance += wantChanceChange;
            /////////////////////////////////////
            if (Random.Range(0,100) < ItemPrices[i].HighDemandChance)
            {
                ItemPrices[i].HighDemandChance -= 5;
                ItemPrices[i].price += Random.Range(0, ItemPrices[i].item.MaxPriceAdjustment);
            }
            else
            {
                ItemPrices[i].HighDemandChance += 5;
                ItemPrices[i].price += Random.Range(-ItemPrices[i].item.MaxPriceAdjustment, 0);
            }

            ItemPrices[i].price += itemPriceEventChange;

            if (ItemPrices[i].price < ItemPrices[i].item.MinPrice)
            {
                ItemPrices[i].price = ItemPrices[i].item.MinPrice;
            }
            else if (ItemPrices[i].price > ItemPrices[i].item.MaxPrice)
            {
                ItemPrices[i].price = ItemPrices[i].item.MaxPrice;
            }
        }
        ItemsPriceTimeLine.Insert(new DateTime(GameController.GameTime), DateTime.CreateFromDays(GameController.GameTime.ToDay() + 1), new PriceTimeline(ItemPrices, _events));
    }

    public void GenerateTown()
    {
        generateName();
        //Create each required location
       
        Type.RequiredLocations.ForEach(x =>
        {
            for (int i = 0; i < x.Amount; i++)
            {
                Location l = new Location(this, x.locationType);
                Locations.Add(l);
                GameController.AllLocations.Add(l);
            }    
        }
        );
        //Randomly Create Optional Locations
        while (Locations.Count < Type.MaxLocations)
        {
            var chance = (float)((1+Locations.Count - Type.RequiredLocations.Count)/1.5f) / (float)(1+ Type.MaxLocations - Type.RequiredLocations.Count);
            if (Random.Range(0f,1f) < chance)
            {
                break;
            }

            Location l = new Location(this, Type.OptionalLocations[Random.Range(0, Type.OptionalLocations.Count)]);
            Locations.Add(l);
            GameController.AllLocations.Add(l);
        }
    }

    private void generateName()
    {

    }
}
