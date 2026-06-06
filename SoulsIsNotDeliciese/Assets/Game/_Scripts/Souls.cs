using System.Collections.Generic;
using UnityEngine;

public class Souls : ScriptableObject
{
    public string name;
    public List<string> replices;
    public string bonusDescription;


    public virtual void Bonus(BoilerManager boilerManager)
    {

    }
    public virtual void RemoveBonus(BoilerManager boilerManager)
    {

    }
}