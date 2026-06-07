using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Demon : MonoBehaviour
{
	[SerializeField] private TextPlayer textPlayer;
	[SerializeField] private float speed, waitTime;
	[SerializeField] private Replices[] replices;
	
	public bool IsPlayingReplices = false;

	private bool isDayEnd, isFoodEnd;

	private void Start()
	{
		DayManager.instance.OnNewDay += PlayDayStartReplices;
		DemonKvotaManager.instance.OnKvotaFilled += PlayFoodEndReplices;

		textPlayer.Init();
	}

	private void PlayDayStartReplices()
	{
		if (isDayEnd) { return; }
		isDayEnd = true;
		isFoodEnd = false;

		PlayReplices(replices[DayManager.instance.currentDay].DayStartReplices);
	}

	private void PlayFoodEndReplices()
	{
		if (isFoodEnd) { return; }
		isFoodEnd = true;
		isDayEnd = false;

		PlayReplices(replices[DayManager.instance.currentDay].FoodEndReplices);
	}

	private async UniTask PlayReplices(string[] replices)
	{
		if (IsPlayingReplices) { return; }

		IsPlayingReplices = true;

		for (int i = 0; i < replices.Length; i++)
		{
			await textPlayer.SkipOrPlay(replices[i], speed, waitTime, TextPlayer.TextingType.SecondsPerLetter);
		}

		IsPlayingReplices = false;
	}


	[Serializable]
	public class Replices
	{
		public string[] DayStartReplices;
		public string[] FoodEndReplices;
	}
}
