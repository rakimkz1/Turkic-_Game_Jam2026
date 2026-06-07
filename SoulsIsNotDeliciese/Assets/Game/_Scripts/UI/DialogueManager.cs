using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	public float TextingTime;
	public float SecondPerLetter;
	public float WaitTime;
	public AudioSource sound;

	[SerializeField] private TextPlayer textPlayer;

	public string content;

	private void Awake()
	{
		textPlayer.Init(TextingTime, SecondPerLetter, WaitTime, sound);
	}

	[ContextMenu("Play the Content with Texting Time")]
	public void PlayTextingTime()
	{
		textPlayer.SkipOrPlay(content, TextingTime, WaitTime, TextPlayer.TextingType.SecondsForWholeText);
	}

	public void PlayTextingTime(string Contents)
	{
		content = Contents;
		PlayTextingTime();
    }

    [ContextMenu("Play the Content with Seconds per Letter")]
	public void PlaySecondsPerLetter()
	{
		textPlayer.SkipOrPlay(content, SecondPerLetter, WaitTime, TextPlayer.TextingType.SecondsPerLetter);
	}
	public void PlaySecondsPerLetter(string Contents)
	{
		content = Contents;
		PlaySecondsPerLetter();
    }
}
