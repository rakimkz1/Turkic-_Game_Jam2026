using TMPro;
using UnityEngine;

public class PopUpLogic : MonoBehaviour
{
	[SerializeField] private TextPlayer textPlayer;
	[SerializeField] private float TextSpeed = 1.0f;
	[SerializeField] private float WaitTime = 5f;
	[SerializeField] private TextPlayer.TextingType TextingType = TextPlayer.TextingType.SecondsPerLetter;

	private RectTransform rectTransform;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="target">Либо Курсор, Либо объект на котором будет попап</param>
	public void Init(string content) 
	{
		rectTransform = GetComponent<RectTransform>();

		textPlayer.Init();
		textPlayer.SkipOrPlay(content, TextSpeed, WaitTime, TextingType);
		textPlayer.OnTextClosed += Destroy;
	}

	private void Update()
	{
		Vector3 Direction = transform.position - Camera.main.transform.position;
		transform.rotation = Quaternion.LookRotation(Direction);
	}

	private void Destroy()
	{
		Destroy(gameObject);
	}
}
