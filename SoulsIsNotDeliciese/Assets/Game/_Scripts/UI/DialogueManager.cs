using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	public float TextingTime;
	public float SecondPerLetter;
	public float WaitTime;

	[SerializeField] private TextPlayer textPlayer;

	public string content;

	private void Awake()
	{
		textPlayer.Init(TextingTime, SecondPerLetter, WaitTime);
	}

	[ContextMenu("Play the Content with Texting Time")]
	public void PlayTextingTime()
	{
		textPlayer.SkipOrPlay(content, TextingTime, WaitTime, false);
	}

	[ContextMenu("Play the Content with Seconds per Letter")]
	public void PlaySecondsPerLetter()
	{
		textPlayer.SkipOrPlay(content, SecondPerLetter, WaitTime, true);
	}
}
