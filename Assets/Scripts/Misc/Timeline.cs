using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[Serializable]
public class Timeline<T> : IEnumerable<T>
{
    public class Point
    {
        public DateTime startDateTime;
        public DateTime endDateTime;
        public T item;

        public Point(DateTime _startdatetime, DateTime _enddatetime, T _item)
        {
            startDateTime = _startdatetime;
            endDateTime = _enddatetime;
            item = _item;
        }
    }

    public int LengthGroupedByStartDate {
        get {
            return Points.GroupBy(x => x.startDateTime).Count();
        }
    }


    public int TotalLength
    {
        get
        {
            return Points.Count();
        }
    }

    public void Insert(DateTime startdatetime, DateTime enddatetime, T item)
    {
        int index = 0;

        for (int i = 0; i < Points.Count; i++)
        {
            if (Points[i].startDateTime.ToHour()  > startdatetime.ToHour())
            {
                index = i;
                break;
            }
        }

        Points.Insert(index, new Point(startdatetime, enddatetime, item));
    }

    public List<T> GetDataInHour(DateTime datetime)
    {
        List<T> temp = new List<T>();
        Points.Where(x => datetime.ToHour() >= x.startDateTime.ToHour() && datetime.ToHour() < x.endDateTime.ToHour())
            .ToList()
            .ForEach(x => temp.Add(x.item));
        return temp;
    }

    public List<T> GetDataInDay(DateTime datetime)
    {
        List<T> temp = new List<T>();
        Points.Where(x => {
            var start = x.startDateTime.ToDay();
            var end = x.endDateTime.ToDay();
            var d = datetime.ToDay();
            return d >= start && d < end ;
        })
        .ToList()
        .ForEach(x => temp.Add(x.item));
        return temp;
    }

    public List<T> GetDataInRange(DateTime mindatetime, DateTime maxdatetime)
    {
        List<T> temp = new List<T>();
        Points.Where(x => {
            var start = x.startDateTime.ToHour() >= mindatetime.ToHour() && x.startDateTime.ToHour() <= maxdatetime.ToHour();
            var end = x.endDateTime.ToHour() >= mindatetime.ToHour() && x.endDateTime.ToHour() <= maxdatetime.ToHour();
            return (start || end);
        })
        .ToList()
        .ForEach(x => temp.Add(x.item));
        return temp;
    }
    /// <summary>
    /// Gets data where the start date is after the given datetime. Excludes currently occuring points
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public List<T> GetFutureDataAfterDate(DateTime datetime)
    {
        List<T> temp = new List<T>();
        Points.Where(x => {
            return x.startDateTime.ToHour() > datetime.ToHour();
        })
        .ToList()
        .ForEach(x => temp.Add(x.item));
        return temp;
    }

    /// <summary>
    /// Gets data where the end date is after the given datetime. Includes currently occuring points
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public List<T> GetCurrentDataAfterDate(DateTime datetime)
    {
        List<T> temp = new List<T>();
        Points.Where(x => {
            return x.endDateTime.ToHour() > datetime.ToHour() && x.startDateTime.ToHour() <= datetime.ToHour();
        })
        .ToList()
        .ForEach(x => temp.Add(x.item));
        return temp;
    }

    public List<T> GetDataBeforeDate(DateTime datetime)
    {
        List<T> temp = new List<T>();
        Points.Where(x => {
            return x.startDateTime.ToHour() < datetime.ToHour();
        })
        .ToList()
        .ForEach(x => temp.Add(x.item));
        return temp;
    }


    public List<Point> GetAllData()
    {
        return Points;
    }

    List<Point> Points = new List<Point>();


    public IEnumerator<T> GetEnumerator()
    {
        List<T> temp = new List<T>();
        Points.ForEach(x => temp.Add(x.item));
        return temp.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Points.GetEnumerator();
    }
}
