using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Batir", menuName = "Souls/Batir")]
public class Batir : Souls
{
    public float damageFromCap;
    public override void Bonus(BoilerManager boilerManager)
    {
        boilerManager.damageFromCap = damageFromCap;
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        boilerManager.damageFromCap = 0;
    }
}