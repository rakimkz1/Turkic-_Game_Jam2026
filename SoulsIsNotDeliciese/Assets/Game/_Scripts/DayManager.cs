using System;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;
    public int currentDay;
    public Action OnNewDay;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartNewDay()
    {
        currentDay++;
        OnNewDay?.Invoke();
        Debug.Log("Starting day " + currentDay);
    }
}
