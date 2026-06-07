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

	public event Action OnTextClosed, OnTextingEnd;

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

	public async UniTask SkipOrPlay(
		string content,
		float speed,
		float keepDuration,
		TextingType textingType,
		bool showImmediatelyAfterSkip = false)
	{
		this.keepDuration = keepDuration;

		if (textingType == TextingType.SecondsPerLetter)
			secondsPerLetter = speed;
		else
			playingTime = speed;

		await SkipOrPlay(content, textingType, showImmediatelyAfterSkip);
	}

	public async UniTask SkipOrPlay(
		string content,
		TextingType textingType,
		bool showImmediatelyAfterSkip = false)
	{
		switch (currentState)
		{
			case State.Nothing:
				StartPlaying(content, textingType);
				break;

			case State.Playing:
				SkipCurrentText();

				if (showImmediatelyAfterSkip)
				{
					StartPlaying(content, textingType);
				}
				break;

			case State.Showing:
				StartPlaying(content, textingType);
				break;
		}
	}

	public void ClosePlaying()
	{
		CancelCurrentTask();

		currentState = State.Nothing;
		currentFullContent = string.Empty;
		OnTextClosed?.Invoke();

		if (Text != null)
			Text.text = string.Empty;
	}

	private async UniTask StartPlaying(string content, TextingType textingType)
	{
		CancelCurrentTask();

		currentFullContent = content;
		currentState = State.Playing;

		cts = new CancellationTokenSource();

		await PlayAsync(
			content,
			textingType,
			cts.Token);
	}

	private void SkipCurrentText()
	{
		CancelCurrentTask();

		currentState = State.Showing;
		Text.text = currentFullContent;
	}

	private async UniTask PlayAsync(
		string fullContent,
		TextingType textingType,
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

			float duration = textingType == TextingType.SecondsPerLetter
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
			OnTextingEnd?.Invoke();

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

	public enum TextingType
	{
		SecondsPerLetter,
		SecondsForWholeText
	}
}