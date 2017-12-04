using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FromJsonEventType {

    public FromJsonEventType()
    {

    }

    [Serializable]
    public struct FromJsonEventEffect
    {
        public string Item;
        public int BasePriceChange;
        public int WantChanceChange;
        public int AvalibilityChange;
        public float ProductionRateChage;
    }
    [Serializable]
    public struct FromJsonKnowledgeChance
    {
        public string occupation;
        public int chance;
    }
    public string Name;
    public string Scale;
    public int MinDuration; //in Days
    public int MaxDuration; //in Days
    public int PreemptiveKnowledgeDuration; //in days
    public float OccurringChanceMultiplier;
    public FromJsonEventEffect[] Effects;
    public string[] LocationTypesAffected;
    public string[] TownTypesAffected;
    public FromJsonKnowledgeChance[] FutureKnowledgeChance;
    public FromJsonKnowledgeChance[] CurrentKnowledgeChance;
    public int BasePrice;
}
