using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Demon : MonoBehaviour
{
	[SerializeField] private TextPlayer textPlayer;
	[SerializeField] private float speed, waitTime;
	[SerializeField] private Replices[] replices;
	
	public bool IsPlayingReplices = false;
	public string DefaultPos;

	private Animator anim;
	private bool isDayEnd, isFoodEnd;

	private void Start()
	{
		DayManager.instance.OnNewDay += PlayDayStartReplices;
		DemonKvotaManager.instance.OnKvotaFilled += PlayFoodEndReplices;

		textPlayer.Init();
		anim = GetComponent<Animator>();
		PlayDayStartReplices();
	}

	private void PlayDayStartReplices()
	{
		if (isDayEnd) { return; }
		Debug.Log("Start Replice");
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

	private async UniTask PlayReplices(Speach[] replices)
	{
		if (IsPlayingReplices) { return; }

		Debug.Log("Replices");
		IsPlayingReplices = true;

		for (int i = 0; i < replices.Length; i++)
		{
			Debug.Log(replices[i].Replices);
			if (replices[i].AnimPos != String.Empty)
			{
				anim.CrossFade(replices[i].AnimPos, replices[i].animCrossFades);
			}
			await textPlayer.SkipOrPlay(replices[i].Replices, speed, waitTime, TextPlayer.TextingType.SecondsPerLetter);
		}

		anim.Play(DefaultPos);
		IsPlayingReplices = false;
	}


	[Serializable]
	public class Replices
	{
		public Speach[] DayStartReplices;
		public Speach[] FoodEndReplices;
	}
	[Serializable]
	public class Speach
	{
		public string Replices;
		public string AnimPos;
		public float animCrossFades;
	}
}
