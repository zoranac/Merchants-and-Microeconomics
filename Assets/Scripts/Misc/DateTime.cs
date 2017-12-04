using UnityEngine;
using System.Collections;

public class DateTime
{
    public int Year;
    public int Season;
    public int Day;
    public int Hour;

    public string Date
    {
        get { return  this.Day + "/" + this.Season + "/" + this.Year; }
        set { this.Date = value; }
    }

    public string Time
    {
        get
        {
            string tempHour;

            if (this.Hour < 10)
                tempHour = "0" + this.Hour;
            else
                tempHour = this.Hour.ToString();

            return tempHour;
        }
        set { this.Time = value; }
    }

    public string Date_Time
    {
        get { return Date + " " + Time; }
        set { this.Date_Time = value; }
    }


    public DateTime()
    {
        Hour = 0;
        Day = 0;
        Season = 0;
        Year = 0;
    }

    public DateTime(DateTime dateTime)
    {
        Hour = dateTime.Hour;
        Day = dateTime.Day;
        Season = dateTime.Season;
        Year = dateTime.Year;
    }

    public DateTime(int h, int d, int s, int y)
    {
        int r = 0;
        if (h > 12)
        {
            Hour = h % 12;
            r = Mathf.FloorToInt((float)h / 12);
        }
        else
        {
            Hour = h;
        }

        if (d + r > 30)
        {
            Day = (d+r) % 30;
            r = Mathf.FloorToInt((float)(d + r) / 30);
        }
        else
        {
            Day = d + r;
            r = 0;
        }

        if (s + r > 4)
        {
            Season = (s + r) % 4;
            r = Mathf.FloorToInt((float)(s + r) / 4);
        }
        else
        {
            Season = s + r;
            r = 0;
        }

        Year = y+r;
    }
    public override string ToString()
    {
        return Date_Time;
    }
    public ulong ToHour()
    {
        return (ulong)((Year * 1440) + (Season * 360) + (Day * 12) + Hour);
    }
    public int ToDay()
    {
        return (Year * 120) + (Season * 30) + Day;
    }

    public static DateTime CreateFromHours(int hours)
    {
        return new DateTime(hours, 0, 0, 0);
    }

    public static DateTime CreateFromDays(int days)
    {
        return new DateTime(0, days, 0, 0);
    }

    public static DateTime CreateFromSeasons(int seasons)
    {
        return new DateTime(0, 0, seasons, 0);
    }

    public static DateTime operator -(DateTime a, DateTime b)
    {
        DateTime returnDateTime = new DateTime();

        returnDateTime.Hour = a.Hour - b.Hour + returnDateTime.Hour;
        if (returnDateTime.Hour < 0)
        {
            returnDateTime.Hour = 12 - returnDateTime.Hour;
            returnDateTime.Day = -1;
        }
        returnDateTime.Day = a.Day - b.Day + returnDateTime.Day;
        if (returnDateTime.Day < 0)
        {
            returnDateTime.Day = 30 - returnDateTime.Day;
            returnDateTime.Season = -1;
        }
        returnDateTime.Season = a.Season - b.Season + returnDateTime.Season;
        if (returnDateTime.Season < 0)
        {
            returnDateTime.Season = 4 - returnDateTime.Season;
            returnDateTime.Year = -1;
        }
        returnDateTime.Year = a.Year - b.Year + returnDateTime.Year;
        if (returnDateTime.Year < 0)
        {
            returnDateTime.Year = 0;
        }
        return returnDateTime;
    }
    public static DateTime operator +(DateTime a, DateTime b)
    {
        DateTime returnDateTime = new DateTime();

        returnDateTime.Hour = a.Hour + b.Hour + returnDateTime.Hour;
        while (returnDateTime.Hour > 11)
        {
            returnDateTime.Hour = returnDateTime.Hour -12;
            returnDateTime.Day += 1;
        }
        returnDateTime.Day = a.Day + b.Day + returnDateTime.Day;
        while (returnDateTime.Day > 30)
        {
            returnDateTime.Day = returnDateTime.Day - 31;
            returnDateTime.Season += 1;
        }
        returnDateTime.Season = a.Season + b.Season + returnDateTime.Season;
        while (returnDateTime.Season > 4)
        {
            returnDateTime.Season = returnDateTime.Season - 5;
            returnDateTime.Year += 1;
        }
        returnDateTime.Year = a.Year + b.Year + returnDateTime.Year;
        if (returnDateTime.Year < 0)
        {
            returnDateTime.Year = 0;
        }
        return returnDateTime;
    }


}
