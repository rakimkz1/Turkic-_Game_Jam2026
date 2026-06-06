using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dad", menuName = "Souls/Dad")]
public class Dad : Souls
{
    public float speedBonus = 1.2f;
    public float tiltMaxAngleBonus = 1.3f;
    public float tiltSpeedBonus = 1.2f;

    public override void Bonus(BoilerManager boilerManager)
    {
        boilerManager.capMoveSpeed *= speedBonus;
        boilerManager.capMaxTiltAngle *= tiltMaxAngleBonus;
        boilerManager.capTiltSpeed *= tiltSpeedBonus;
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        boilerManager.capMoveSpeed /= speedBonus;
        boilerManager.capMaxTiltAngle /= tiltMaxAngleBonus;
        boilerManager.capTiltSpeed /= tiltSpeedBonus;
    }   
}