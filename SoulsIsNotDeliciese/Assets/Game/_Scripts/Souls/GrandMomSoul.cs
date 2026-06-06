using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GrandMom", menuName = "Souls/GrandMom")]
public class GrandMomSoul : Souls
{
    public float tiltRecude;
    public override void Bonus(BoilerManager boilerManager)
    {
        boilerManager.capMaxTiltAngle *= tiltRecude;
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        boilerManager.capMaxTiltAngle /= tiltRecude;
    }
}