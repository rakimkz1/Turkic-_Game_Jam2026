using UnityEngine;


[CreateAssetMenu(fileName = "Aksacal", menuName = "Souls/Aksacal")]
public class Aksacal : Souls
{
    public float wallSizeBonus = 1.3f;
    public override void Bonus(BoilerManager boilerManager)
    {
        Vector3 size = boilerManager.caps.transform.localScale;
        size.x *= wallSizeBonus;
        boilerManager.caps.transform.localScale = size;
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        Vector3 size = boilerManager.caps.transform.localScale;
        size.x /= wallSizeBonus;
        boilerManager.caps.transform.localScale = size;
    }
}