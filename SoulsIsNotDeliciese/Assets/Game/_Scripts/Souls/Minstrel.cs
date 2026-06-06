using UnityEngine;

[CreateAssetMenu(fileName = "Minstrel", menuName = "Souls/Minstrel")]
public class Minstrel : Souls
{
    public float returnChance;
    public override void Bonus(BoilerManager boilerManager)
    {
        boilerManager.soulBall.isMinistalAllowEscape = ReturnBall;
    }
    public bool ReturnBall()
    {
        if(Random.value < returnChance)
        {
            return false;
        }
        return true;
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        boilerManager.soulBall.isMinistalAllowEscape = null;
    }
}