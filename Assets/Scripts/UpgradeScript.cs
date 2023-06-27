using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UpgradeScript : MonoBehaviour
{
    [SerializeField] private StatsScriptableObject statsToUpgrade;

    void UpgradeStats(Stats statsList)
    {
        foreach(Stat upgrade in statsToUpgrade.statsList) {
            var stat = statsList.GetStat(upgrade.name);
            if(stat != null)
                stat.AddModifier(upgrade.GetValue());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            UpgradeStats(other.gameObject.GetComponent<Stats>());
            Destroy(gameObject);
        }
    }
}
