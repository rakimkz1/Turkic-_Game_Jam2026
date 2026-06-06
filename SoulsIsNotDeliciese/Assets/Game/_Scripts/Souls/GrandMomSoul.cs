using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GrandMom", menuName = "Souls/GrandMom")]
public class GrandMomSoul : Souls
{
    public override void Bonus()
    {
        Debug.Log("GrandMomBonus");
    }
}