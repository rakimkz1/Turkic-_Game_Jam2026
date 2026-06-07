using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Demon : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private AnimationCurve WaitCurve;
	[SerializeField] private Replices[] replices;
	[SerializeField] private Vector2 padding;
	
	public bool IsPlayingReplices = false;

	private bool isDayEnd, isFoodEnd;
	private bool isSkipping = false;

	private RectTransform bgRect;

	private CancellationTokenSource cts;

	private void Start()
	{
		DayManager.instance.OnNewDay += PlayDayStartReplices;
		DemonKvotaManager.instance.OnKvotaFilled += PlayFoodEndReplices;

		bgRect = text.rectTransform.parent as RectTransform;
	}

	private void PlayDayStartReplices()
	{
		if (isDayEnd) { return; }
		isDayEnd = true;
		isFoodEnd = false;

		cts = new CancellationTokenSource();
		PlayReplices(replices[DayManager.instance.currentDay].DayStartReplices, cts.Token);
	}

	private void PlayFoodEndReplices()
	{
		if (isFoodEnd) { return; }
		isFoodEnd = true;
		isDayEnd = false;

		cts = new CancellationTokenSource();
		PlayReplices(replices[DayManager.instance.currentDay].FoodEndReplices, cts.Token);
	}

	private async UniTask PlayReplices(string[] replices, CancellationToken token)
	{
		if (IsPlayingReplices) { return; }

		IsPlayingReplices = true;

		for (int i = 0; i < replices.Length; i++)
		{
			SetText(replices[i]);
			ContentSizeFitter czf;

			float waitTime = Mathf.Clamp(replices[i].Length, 0, 100) * 0.1f;
			waitTime = WaitCurve.Evaluate(waitTime);
			await WaitReplic(waitTime*10);
		}

		IsPlayingReplices = false;
	}

	private void SetText(string text)
	{
		this.text.text = TextPlayer.WrapText(text, 40);
		bgRect.sizeDelta = this.text.GetPreferredValues() + padding;
	}

	private async UniTask WaitReplic(float duration)
	{
		float time = 0;
		while (time <= duration)
		{
			time += Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || isSkipping)
			{
				isSkipping = false;
				return;
			}

			await UniTask.Yield();
		}
	}

	public void Skip() => isSkipping = true;


	[Serializable]
	public class Replices
	{
		public string[] DayStartReplices;
		public string[] FoodEndReplices;
	}
}
