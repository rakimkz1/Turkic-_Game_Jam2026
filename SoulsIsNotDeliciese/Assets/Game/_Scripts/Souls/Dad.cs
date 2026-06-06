using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dad", menuName = "Souls/Dad")]
public class Dad : Souls
{
    public float speedBonus = 1.2f;

    public override void Bonus(BoilerManager boilerManager)
    {
        Debug.Log("Dad Bonus Activated");
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        Debug.Log("Dad Bonus Removed");
    }   
}