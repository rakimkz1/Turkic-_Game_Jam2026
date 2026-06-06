using System;
using UnityEngine;


public class Kalendar : MonoBehaviour
{
    public void Activate()
    {
        if (DemonKvotaManager.instance.todaysKvota >= DemonKvotaManager.instance.maxKvota)
            DayManager.instance.StartNewDay();
        else
            Reject();
    }

    private void Reject()
    {
        throw new NotImplementedException();
    }
}