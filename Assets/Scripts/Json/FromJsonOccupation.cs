using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FromJsonOccupation
{

    public FromJsonOccupation()
    {

    }

    public string Name;
    public string[] BuyItems;
    public string[] SellItems;
    public bool ProducesItems;
    public float ProductionSpeed;
    public int BaseGossipAmount;
    public int NormalAmountPurchasedForProduction;
    public int NormalNumberOfSales;
}
