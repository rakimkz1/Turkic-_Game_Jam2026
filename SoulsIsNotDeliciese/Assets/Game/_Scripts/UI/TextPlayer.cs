using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

[Serializable]
public class TextPlayer
{
	public TextMeshProUGUI Text;
	public State CurrentState => currentState;

	private float playingTime = 1f;
	private float secondsPerLetter = 0.1f;
	private float keepDuration = 10f;

	private State currentState = State.Nothing;

	private string currentFullContent = string.Empty;

	private CancellationTokenSource cts;

	public void Init(
		float speedPlaying = 1f,
		float secondsPerLetter = 0.1f,
		float keepDuration = 10f)
	{
		this.playingTime = speedPlaying;
		this.secondsPerLetter = secondsPerLetter;
		this.keepDuration = keepDuration;

		currentState = State.Nothing;
		currentFullContent = string.Empty;

		Text.text = string.Empty;
	}

	public void SkipOrPlay(
		string content,
		float speed,
		float keepDuration,
		bool useSecondsPerLetter = false,
		bool showImmediatelyAfterSkip = false)
	{
		this.keepDuration = keepDuration;

		if (useSecondsPerLetter)
			secondsPerLetter = speed;
		else
			playingTime = speed;

		SkipOrPlay(content, useSecondsPerLetter, showImmediatelyAfterSkip);
	}

	public void SkipOrPlay(
		string content,
		bool useSecondsPerLetter = false,
		bool showImmediatelyAfterSkip = false)
	{
		switch (currentState)
		{
			case State.Nothing:
				StartPlaying(content, useSecondsPerLetter);
				break;

			case State.Playing:
				SkipCurrentText();

				if (showImmediatelyAfterSkip)
				{
					StartPlaying(content, useSecondsPerLetter);
				}
				break;

			case State.Showing:
				StartPlaying(content, useSecondsPerLetter);
				break;
		}
	}

	public void ClosePlaying()
	{
		CancelCurrentTask();

		currentState = State.Nothing;
		currentFullContent = string.Empty;

		if (Text != null)
			Text.text = string.Empty;
	}

	private void StartPlaying(string content, bool useSecondsPerLetter)
	{
		CancelCurrentTask();

		currentFullContent = content;
		currentState = State.Playing;

		cts = new CancellationTokenSource();

		_ = PlayAsync(
			content,
			useSecondsPerLetter,
			cts.Token);
	}

	private void SkipCurrentText()
	{
		CancelCurrentTask();

		currentState = State.Showing;
		Text.text = currentFullContent;
	}

	private async UniTaskVoid PlayAsync(
		string fullContent,
		bool useSecondsPerLetter,
		CancellationToken token)
	{
		try
		{
			await UniTask.Yield(token);

			if (string.IsNullOrEmpty(fullContent))
			{
				currentState = State.Showing;
				return;
			}

			float duration = useSecondsPerLetter
				? secondsPerLetter * fullContent.Length
				: playingTime;

			float currentSecondsPerLetter = duration / fullContent.Length;

			float elapsed = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;

				int length =
					Mathf.Min(
						(int)(elapsed / currentSecondsPerLetter),
						fullContent.Length);

				Text.text = fullContent.Substring(0, length);

				await UniTask.Yield(token);
			}

			Text.text = fullContent;
			currentState = State.Showing;

			await UniTask.WaitForSeconds(
				keepDuration,
				cancellationToken: token);

			if (currentState == State.Showing)
			{
				ClosePlaying();
			}
		}
		catch (OperationCanceledException)
		{
			// Нормальное завершение.
		}
	}

	private void CancelCurrentTask()
	{
		if (cts == null)
			return;

		cts.Cancel();
		cts.Dispose();
		cts = null;
	}

	public enum State
	{
		Nothing,
		Showing,
		Playing
	}
}