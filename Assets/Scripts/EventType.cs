using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventType {

    [System.Serializable]
    public class KnowledgeChance
    {
        public KnowledgeChance(Occupations _occupation, int _chance)
        {
            occupation = _occupation;
            chance = _chance;
        }

        [SerializeField]
        public Occupations occupation;
        [SerializeField]
        public int chance;
    }

    public enum Scales
    {
        Global,
        Towns,
        Town,
        Locations,
        Location,
        None
    }

    public EventType(FromJsonEventType fromJson)
    {
        Name = fromJson.Name;
        Scale = (Scales)Enum.Parse(typeof(Scales), fromJson.Scale);
        MinDuration = fromJson.MinDuration;
        MaxDuration = fromJson.MaxDuration;
        PreemptiveKnowledgeDuration = fromJson.PreemptiveKnowledgeDuration;
        OccurringChanceMultiplier = fromJson.OccurringChanceMultiplier;
        BasePrice = fromJson.BasePrice;
        foreach (var effect in fromJson.Effects)
        {
            Item i = GameController.AllItemTypes.Where(x => x.Name == effect.Item).ToList()[0];
            Effects.Add(new EventEffect(i, effect.BasePriceChange, effect.WantChanceChange, effect.AvalibilityChange, effect.ProductionRateChage));
        }
        if (fromJson.LocationTypesAffected != null)
        {
            foreach (var location in fromJson.LocationTypesAffected)
            {
                LocationTypesAffected.Add(GameController.AllLocationTypes.Where(x => x.Name == location).ToList()[0]);
            }
        }
        if (fromJson.TownTypesAffected != null)
        {
            foreach (var town in fromJson.TownTypesAffected)
            {
                TownTypesAffected.Add((TownType.Types)Enum.Parse(typeof(TownType.Types), town));
            }
        }
        foreach (var item in fromJson.FutureKnowledgeChance)
        {
            FutureKnowledgeChance.Add(new KnowledgeChance((Occupations)Enum.Parse(typeof(Occupations), item.occupation), item.chance));
        }
        foreach (var item in fromJson.CurrentKnowledgeChance)
        {
            CurrentKnowledgeChance.Add(new KnowledgeChance((Occupations)Enum.Parse(typeof(Occupations), item.occupation), item.chance));
        }
    }
    public string Name;
    public Scales Scale;
    public int MinDuration; //in Days
    public int MaxDuration; //in Days
    public int PreemptiveKnowledgeDuration; //in days
    public float OccurringChanceMultiplier = 1;
    public List<EventEffect> Effects = new List<EventEffect>();
    public List<LocationType> LocationTypesAffected = new List<LocationType>();
    public List<TownType.Types> TownTypesAffected = new List<TownType.Types>();
    public List<KnowledgeChance> FutureKnowledgeChance = new List<KnowledgeChance>();
    public List<KnowledgeChance> CurrentKnowledgeChance = new List<KnowledgeChance>();
    public bool Experienceable;
    public int BasePrice;
}
