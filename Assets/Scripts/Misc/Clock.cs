using UnityEngine;
using System.Collections;

public class Clock : DateTime
{
    public event TickHandler NewHour;
    public event TickHandler NewDay;
    public event TickHandler NewSeason;
    public event TickHandler NewYear;
    public Clock()
    {
        GameController.Tick += new TickHandler(Tick);
        Day = 1;
        Season = 1;
        Year = 1;
    }

    public void SubscribeToTickHandler(TickHandler tickHandler)
    {
        tickHandler += new TickHandler(Tick);
    }

    public void Tick()
    {
        Hour++;
        NewHour();
        if (Hour > 11)
        {
            Day++;
            Hour = 0;
            NewDay();
            if (Day > 30)
            {
                Season++;
                Day = 1;
                NewSeason();
                if (Season > 4)
                {
                    Year++;
                    Season = 1;
                    NewYear();
                }
            }
        }
        //Debug.Log(this.Date_Time);
    }
}
