using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "GrandFather", menuName = "Souls/GrandFather")]
public class GrandFather : Souls
{
    public float chanceToExecute;
    public float slowDownNumber; 
    public override void Bonus(BoilerManager boilerManager)
    {
        boilerManager.soulBall.OnHitBoiler += SlowDownBall;
    }
    public void SlowDownBall()
    {
        if (Random.value < chanceToExecute)
        {
            BoilerManager.Instance.soulBall.SlowDownBall(slowDownNumber);
        }
    }
    public override void RemoveBonus(BoilerManager boilerManager)
    {
        boilerManager.soulBall.OnHitBoiler -= SlowDownBall;
    }
}