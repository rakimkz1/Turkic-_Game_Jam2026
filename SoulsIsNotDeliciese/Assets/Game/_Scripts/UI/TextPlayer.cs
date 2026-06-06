using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

[Serializable]
public class TextPlayer
{
	public TextMeshProUGUI Text;
	public State CurrentState {  get => currentState; }

	private float speedPlaying = 10f;
	private float keepDuration = 10f;

	private State currentState;
	private string currentFullContent;

	private CancellationTokenSource cts;

	private bool isCtsWorking = false;

	public void Init(float SpeedPlaying = 10f, float KeepDuration = 10f)
	{
		Text.text = "";
		currentState = State.Nothing;
		speedPlaying = SpeedPlaying;
		keepDuration = KeepDuration;
	}

	public void SkipOrPlay(string content, bool ShowImmediatelyAfterSkip = false)
	{
		switch (currentState)
		{
			case State.Nothing:
				{
					currentState = State.Playing;
					currentFullContent = content;

					if (isCtsWorking)
					{
						cts.Cancel();
					}
					cts = new CancellationTokenSource();
					_ = PlayTheText(cts);
				}
				break;

			case State.Playing:
				{
					currentState = State.Showing;
					cts.Cancel();
					Text.text = currentFullContent;

					if (ShowImmediatelyAfterSkip)
					{
						currentState = State.Nothing;
						SkipOrPlay(content);
					}
				}
				break;

			case State.Showing:
				{
					currentState = State.Nothing;
					SkipOrPlay(content);
				}
				break;
		}
	}

	public void ClosePlaying()
	{
		currentState = State.Nothing;
		currentFullContent = "";

	}


	private async UniTask PlayTheText(CancellationTokenSource cts)
	{
		try
		{
			isCtsWorking = true;

			await UniTask.Yield(cts.Token);

			float secondsPerLetter = speedPlaying / currentFullContent.Length;
			float time = 0;

			while (time < speedPlaying)
			{
				time += Time.deltaTime;
				int currentLength = (int)(time / secondsPerLetter);
				string content = currentFullContent.Substring(0, currentLength);
				Text.text = content;

				await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
			}
			Text.text = currentFullContent;
			currentState = State.Showing;
			
			cts = new CancellationTokenSource();
			_ = WaitTillEnd(cts);
		}
		catch (Exception ex)
		{
			Debug.Log("PlayTheText has been Cancelled");
		}
	}

	private async UniTask WaitTillEnd(CancellationTokenSource cts)
	{
		try
		{
			await UniTask.WaitForSeconds(keepDuration, cancellationToken: cts.Token);
			isCtsWorking = false;
		}
		catch (Exception ex)
		{
			Debug.Log("WaitTillEnd has been Cancelled");
		}
	}


	public enum State
	{
		Nothing,			// Пустое текстовое поле
		Showing,			// Уже написанное текстовое поле
		Playing				// Еще в процессе написания
	}
}
