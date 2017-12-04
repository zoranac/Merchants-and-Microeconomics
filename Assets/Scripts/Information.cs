using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information {
    public enum Types
    {
        Valid,
        Gossip
    }
    public enum TimeStates
    {
        Future, //has not happened
        Current,//is happening
        Old     //within 10 days of starting
    }

    public Information(Event e, Types type, int basePrice)
    {
        Event = e;
        Type = type;
        Price = basePrice;
        setTimeState();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>If true, this information should be deleted as it is more than a month past the event start date</returns>
    public bool UpdateInformation()
    {
        if (GameController.GameTime.ToDay() > Event.StartDate.ToDay() + 30)
        {
            return true;
        }
        else
        {
            setTimeState();
            return false;
        }
       
    }

    private void setTimeState()
    {
        if (Event.StartDate.ToDay() > GameController.GameTime.ToDay())
        {
            TimeState = TimeStates.Future;
            Price = (int)(Price * 1.5f);
        }
        else if (Event.StartDate.ToDay() <= GameController.GameTime.ToDay() && GameController.GameTime.ToDay() < Event.StartDate.ToDay() + 10)
        {
            TimeState = TimeStates.Current;
        }
        else
        {
            TimeState = TimeStates.Old;
            Price = (int)(Price * .5f);
        }
    }

    public Event Event;
    public Types Type;
    public TimeStates TimeState;
    public int Price;


}
