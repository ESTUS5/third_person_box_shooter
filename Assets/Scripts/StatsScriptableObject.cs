using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats")]
public class StatsScriptableObject : ScriptableObject
{
    public List<Stat> statsList;
    //public Dictionary<Stat, float> statsDictionary;
}
