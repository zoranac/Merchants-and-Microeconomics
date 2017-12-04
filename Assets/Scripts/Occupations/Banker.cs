using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banker : Occupation {

    public override void GenerateChangesForDay(Person person, int dayType, List<Town.Price> prices, out Inventory inv, out int money, out float productionStatus)
    {
        inv = person.Inventory;
        money = person.Money;
        productionStatus = person.ProductionStatus;
    }
}
