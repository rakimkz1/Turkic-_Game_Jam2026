using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Minstrel", menuName = "Souls/Minstrel")]
public class Minstrel : Souls
{
    public override void Bonus(BoilerManager boilerManager)
    {
        Debug.Log("Minstrel Bonus Activated");
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        Debug.Log("Minstrel Bonus Removed");
    }
}