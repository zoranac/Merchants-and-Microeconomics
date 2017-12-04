using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private class PriceTimelineObject
    {
        public PriceTimelineObject(LineRenderer _priceTimeline, Item _item)
        {
            priceTimeline = _priceTimeline;
            item = _item;
        }

        public LineRenderer priceTimeline;
        public Item item;
    }

    public Text GameTimeUI;
    public GameObject LocationButtonPrefab;
    public GameObject PriceTimelinePrefab;

    private List<PriceTimelineObject> PriceTimeline = new List<PriceTimelineObject>();

    public void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        GameController.Tick += GameController_Tick;
        GameController.GameTime.NewDay += GameTime_NewDay;

        foreach (var item in GameController.AllItemTypes)
        {
            var v = GameObject.Instantiate(PriceTimelinePrefab, Vector3.zero, Quaternion.identity);
            PriceTimeline.Add(new PriceTimelineObject(v.GetComponent<LineRenderer>(), item));
        }
    }

    private void GameTime_NewDay()
    {
        foreach (var item in PriceTimeline)
        {
            var price = GameController.CurrentTown.ItemsPriceTimeLine.GetDataInDay(GameController.GameTime)[0].prices.Where(x=>x.item.Name == item.item.Name).ToList()[0].price;
            item.priceTimeline.positionCount = item.priceTimeline.positionCount + 1;
            item.priceTimeline.SetPosition(item.priceTimeline.positionCount - 1, new Vector3((float)(item.priceTimeline.positionCount - 1)/10, (float)price/100));
        }

    }

    private void GameController_Tick()
    {
        GameTimeUI.text = GameController.GameTime.ToString();
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch (scene.name)
        {
            case "LocationScene":
                GenerateLocationUI(GameController.CurrentLocation);
                break;
            case "TownScene":
                GenerateTownUI(GameController.CurrentTown);
                break;
            default:
                break;
        }
    }


    public void GenerateLocationUI(Location CurrentLocation)
    {
        
    }

    public void GenerateTownUI(Town CurrentTown)
    {
        float buttonH = LocationButtonPrefab.GetComponent<Button>().image.sprite.rect.height;
        float buttonW = LocationButtonPrefab.GetComponent<Button>().image.sprite.rect.width;

        float xPos = Screen.width/3f - buttonW - 25;
        float yPos = buttonH + 25;

        foreach (var item in CurrentTown.Locations)
        {
            if (item.Discovered)
            {
                //Create Button
                var button = Instantiate(LocationButtonPrefab, new Vector3(0, 0), Quaternion.identity);
                button.GetComponentInChildren<Text>().text = item.Name;
                button.transform.SetParent(GameObject.Find("Canvas").transform);
                button.transform.localPosition = new Vector3(xPos, yPos);
                button.GetComponent<Button>().onClick.AddListener(item.DisplayLocation);
                yPos -= buttonH + 10;
            }
        }
    }
}
