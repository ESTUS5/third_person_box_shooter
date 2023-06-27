using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stat
{
    public string name ;
    [SerializeField] private float value;

    [SerializeField]private List<float> modifiers = new List<float>();

    public float GetValue()
    {
        float addedValue = value;
        foreach (var modifier in modifiers)
        {
            addedValue += modifier;
        }
        return addedValue;
    }
    public void AddModifier(float modifier)
    {
        modifiers.Add(modifier);
    }
}
