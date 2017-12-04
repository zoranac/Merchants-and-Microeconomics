using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class Inventory : IEnumerable<Inventory.Slot>
{
    [System.Serializable]
    public class Slot
    {
        public Slot(Item item)
        {
            Item = item;
            quantity = 0;
        }
        public Item Item;
        public int Price;
        private int quantity;
        public int Quantity { get { return quantity; } }
        public void AddToQuantity(int numberToAdd)
        {
            quantity += numberToAdd;
        }
        public int SubtractFromQuantity(int numberToSubtract)
        {
            if (quantity - numberToSubtract < 0)
            {
                //return the negative number and don't subtract (this is an error state)
                return quantity - numberToSubtract;
            }
            else
            {
                //subtract from quantity
                quantity -= numberToSubtract;
                return quantity;
            }
        } 
    }

    public delegate void InventoryUpdated(int indexUpdate, Slot slotUpdated);
    public event InventoryUpdated inventoryUpdatedEvent;

    List<Slot> Slots = new List<Slot>();

    public Inventory()
    {
        setupSlots();
    }

    public Slot this[int index]
    {
        get
        {
            // This indexer is very simple, and just returns or sets
            // the corresponding element from the internal array.
            return Slots[index];
        }
        set
        {
            Slots[index] = value;
        }
    }

    public bool Add(Item item, int quantity)
    {
        var slot = Slots.Where(x => x.Item.Name == item.Name).First();
        slot.AddToQuantity(quantity);  //ADD QUANTITY TO INVENTORY

        updatedInventory(Slots.IndexOf(slot), slot);
        return true;
    }

    public bool Remove(Item item, int quantity)
    {
        var slot = Slots.Where(x => x.Item.Name == item.Name).First();
        var result = slot.SubtractFromQuantity(quantity);  //ADD QUANTITY TO INVENTORY

        if (result < 0)
        {
            Debug.LogError("Attepted to subtract less than the amount in inventory");
            return false;
        }
        else
        {
            updatedInventory(Slots.IndexOf(slot), slot);
            return true;
        }
    }

    public bool SetPrice(Item item, int price)
    {
        var slot = Slots.Where(x => x.Item.Name == item.Name).First();

        if (price <= 0)
        {
            Debug.LogError("Attepted To Set Invalid Price");
            return false;
        }
        else
        {
            slot.Price = price;  //Update price
            return true;
        }

    }

    public IEnumerator<Slot> GetEnumerator()
    {
        return Slots.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void setupSlots()
    {
        foreach (var item in GameController.AllItemTypes)
        {
            Slots.Add(new Slot(item));
        }
    }
    private void updatedInventory(int indexUpdated, Slot slotUpdated)
    {
        InventoryUpdated handler = inventoryUpdatedEvent;
        if (handler != null)
        {
            handler(indexUpdated, slotUpdated);
        }
    }
}
