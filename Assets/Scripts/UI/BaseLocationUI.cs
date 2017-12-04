using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseLocationUI : MonoBehaviour {
    public Image BackgroundImage;
    public GameObject BuyAddSellUI;
    public GameObject BuyInformationUI;
    public GameObject UseInformationUI;

    public void GenerateUI(Location location)
    {
        List<Item> itemsToBuy = new List<Item>();
        //Create BuySell
        if (location.Owner.Occupation.BuyItems.Count > 0)
        {
            
        }
    }

}
