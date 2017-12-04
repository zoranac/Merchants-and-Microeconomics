using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Occupation : MonoBehaviour
{
    /*public Occupation(FromJsonOccupation fromJson)
    {
        Name = fromJson.Name;
        if (fromJson.BuyItems != null)
        {
            foreach (var item in fromJson.BuyItems)
            {
                var i = GameController.AllItemTypes.Where(x => x.Name == item).First();
                BuyItems.Add(i);
            }
        }
        if (fromJson.SellItems != null)
        {
            foreach (var item in fromJson.SellItems)
            {
                SellItems.Add(GameController.AllItemTypes.Where(x => x.Name == item).First());
            }
        }
        ProducesItems = fromJson.ProducesItems;
        ProductionSpeed = fromJson.ProductionSpeed;
        BaseGossipAmount = fromJson.BaseGossipAmount;
        NormalAmountPurchasedForProduction = fromJson.NormalAmountPurchasedForProduction;
        NormalNumberOfSales = fromJson.NormalNumberOfSales;
    }*/

    public string Name;
    public List<Item> BuyItems = new List<Item>();
    public List<Item> SellItems = new List<Item>();
    public bool ProducesItems;
    public float ProductionSpeed;
    public int BaseGossipAmount;
    //Used for daily generation
    public int NormalAmountPurchasedForProduction;  //Does not show up in inventory to sell, but reduces money
    public int NormalNumberOfSales;              //randomly subtracted from inventory

    public virtual void GenerateChangesForDay(Person person, int dayType, List<Town.Price> prices, out Inventory inv, out int money, out float productionStatus)
    {
        inv = person.Inventory;
        money = person.Money;
        productionStatus = person.ProductionStatus;

        if (ProducesItems)
        {
            int boughtAmountForProduction = 0;  //Does not show up in inventory to sell, but reduces money
            Item specialItemBought = null;             //only set on daytype 0
            int numberOfSales = 0;              //randomly subtracted from inventory
            float productionStatusIncreaseAmount = 0;   //amount of production completed in the day (everytime this hits 1 an item is added to the inventory based on occupation;
            int numberOfItemsProduced = 0;      //Determined by productionStatusIncreaseAmount 

            switch (dayType)
            {
                case 0: //Buy Sales
                    {
                        Debug.Log("Buy Sales");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction;
                        specialItemBought = GameController.AllItemTypes[Random.Range(0, GameController.AllItemTypes.Count)];
                        numberOfSales = NormalNumberOfSales;
                        productionStatusIncreaseAmount = ProductionSpeed;
                    }
                    break;
                case 1: //High Sales
                    {
                        Debug.Log("High Sales");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction;
                        numberOfSales = NormalNumberOfSales * 2;
                        productionStatusIncreaseAmount = ProductionSpeed;
                    }
                    break;
                case 2: //productive day
                    {
                        Debug.Log("Productive Day");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction;
                        numberOfSales = NormalNumberOfSales;
                        productionStatusIncreaseAmount = ProductionSpeed * 2;
                    }
                    break;
                case 3:  //Normal Day
                    {
                        Debug.Log("Normal Day");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction;
                        numberOfSales = NormalNumberOfSales;
                        productionStatusIncreaseAmount = ProductionSpeed;
                    }
                    break;
                case 4: //Low Sales
                    {
                        Debug.Log("Low Sales");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction;
                        numberOfSales = Mathf.CeilToInt(NormalNumberOfSales / 2);
                        productionStatusIncreaseAmount = ProductionSpeed;
                    }
                    break;
                case 5: //Unproductive
                    {
                        Debug.Log("Unproductive Day");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction;
                        numberOfSales = NormalNumberOfSales;
                        productionStatusIncreaseAmount = ProductionSpeed / 2;
                    }
                    break;
                case 6: //Resources Wasted
                    {
                        Debug.Log("Resources Wasted");
                        boughtAmountForProduction = NormalAmountPurchasedForProduction * 2;
                        numberOfSales = NormalNumberOfSales;
                        productionStatusIncreaseAmount = ProductionSpeed;
                    }
                    break;
                default:
                    break;
            }

            //Calcuate Money
            int moneyChange = 0;
            for (int i = 0; i < numberOfSales; i++)
            {
                var sellableInv = inv.Where(x => x.Quantity > 0).ToList();
                //If there are no more items to sell, stop generating sales
                if (sellableInv.Count <= 0)
                {
                    Debug.LogWarning("Person: " + Name + " ran out of items when generating inventory, fix balancing to try and avoid this");
                    break;
                }
                var itemSold = sellableInv[Random.Range(0, sellableInv.Count)].Item;
                //Update inventory
                person.Inventory.Remove(itemSold, 1);
                //Update Change in Money
                moneyChange = moneyChange + prices.Where(x => x.item == itemSold).First().price;
            }

            //Buy Special Item
            if (specialItemBought != null)
            {
                if (money + moneyChange - prices.Where(x => x.item == specialItemBought).First().price < 0)
                {
                    Debug.LogWarning("Person: " + Name + " ran out of money when buying production items, fix balancing to try and avoid this");
                }
                else
                {
                    moneyChange = moneyChange - prices.Where(x => x.item == specialItemBought).First().price;
                }
            }

            //Buy Items Needed For Production
            for (int i = 0; i < boughtAmountForProduction; i++)
            {
                Item item = BuyItems[Random.Range(0, BuyItems.Count)];
                //If there is no more money to buy things
                if (money + moneyChange - prices.Where(x => x.item == item).First().price < 0)
                {
                    Debug.LogWarning("Person: " + Name + " ran out of money when buying production items, fix balancing to try and avoid this");
                    //reduce the production rate depending on the amount able to buy
                    productionStatusIncreaseAmount = productionStatusIncreaseAmount * (i / boughtAmountForProduction);
                    break;
                }
                //Update Change in Money
                moneyChange = moneyChange - prices.Where(x => x.item == item).First().price;
            }


            //Calculate Production
            productionStatus += productionStatusIncreaseAmount;
            while (productionStatus >= 1)
            {
                numberOfItemsProduced += 1;
                productionStatus -= 1;
            }

            //Calcuate Inventory
            for (int i = 0; i < numberOfItemsProduced; i++)
            {
                var itemMade = SellItems[Random.Range(0, SellItems.Count)];
                inv.Add(itemMade, itemMade.AmountProduced);
            }

            money += moneyChange;
        }
    }
}
