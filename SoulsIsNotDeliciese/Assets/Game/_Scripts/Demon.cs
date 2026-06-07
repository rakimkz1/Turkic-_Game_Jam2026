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
	public string DefaultPos;

	private Animator anim;
	private bool isDayEnd, isFoodEnd;
	private bool isSkipping = false;

	private RectTransform bgRect;

	private CancellationTokenSource cts;

	private void Start()
	{
		DayManager.instance.OnNewDay += PlayDayStartReplices;
		DemonKvotaManager.instance.OnKvotaFilled += PlayFoodEndReplices;

		anim = GetComponent<Animator>();
		bgRect = text.rectTransform.parent as RectTransform;
	}

	private void PlayDayStartReplices()
	{
		if (isDayEnd) { return; }
		Debug.Log("Start Replice");
		isDayEnd = true;
		isFoodEnd = false;

		cts = new CancellationTokenSource();
		PlayReplices(replices[DayManager.instance.currentDay].DayStartReplices);
	}

	private void PlayFoodEndReplices()
	{
		if (isFoodEnd) { return; }
		isFoodEnd = true;
		isDayEnd = false;

		cts = new CancellationTokenSource();
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
			SetText(replices[i].Replices);

			float waitTime = Mathf.Clamp(replices[i].Replices.Length, 0, 100) * 0.1f;
			waitTime = WaitCurve.Evaluate(waitTime);
			await WaitReplic(waitTime*10);
		}

		anim.Play(DefaultPos);
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
