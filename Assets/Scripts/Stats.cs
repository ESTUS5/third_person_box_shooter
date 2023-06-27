using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] private List<Stat> statsList = new List<Stat>();

    public Stat GetStat(string statName)
    {
        return statsList.Find(x=>x.name == statName);
    }
    public void AddStat(Stat stat)
    {
        statsList.Find(x=>x.name == stat.name);
        statsList.Add(stat);
    }
    public List<Stat> GetStatList()
    {
        return statsList;
    }
}

