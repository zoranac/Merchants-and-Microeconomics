using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEffect {

    public EventEffect(Item item, int priceChange, int wantChange, int avalibilityChange, float productionRateChange)
    {
        ItemAffected = item;
        BasePriceChange = priceChange;
        WantChanceChange = wantChange;
        AvalibilityChange = avalibilityChange;
        ProductionRateChage = productionRateChange;
    }

    public Item ItemAffected;
    public int BasePriceChange;
    public int WantChanceChange;
    public int AvalibilityChange;
    public float ProductionRateChage;
}
