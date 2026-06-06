using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "GrandFather", menuName = "Souls/GrandFather")]
public class GrandFather : Souls
{
    public override void Bonus(BoilerManager boilerManager)
    {
        Debug.Log("GrandFatherBonus");
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        Debug.Log("GrandFather Bonus Removed!");
    }
}