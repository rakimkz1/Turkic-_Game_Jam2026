using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;
    public int currentDay;      // start with 0 (0 is first day)
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

        currentDay = PlayerPrefs.GetInt("CurrentDay", 0);
    }

	private void Start()
	{
        LateStart();
	}

    private async UniTask LateStart()
    {
        await UniTask.Yield();

		OnNewDay.Invoke();
	}
	public void StartNewDay()
    {
        currentDay++;
        OnNewDay?.Invoke();
        Debug.Log("Starting day " + currentDay);
    }

	private void OnDestroy()
	{
        PlayerPrefs.SetInt("CurrentDay", currentDay);
	}
}
