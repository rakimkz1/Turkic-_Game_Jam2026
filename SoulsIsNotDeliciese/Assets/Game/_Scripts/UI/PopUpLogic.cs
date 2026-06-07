using System;
using TMPro;
using UnityEngine;

public class PopUpLogic : MonoBehaviour
{
	[SerializeField] private TextPlayer textPlayer;
	[SerializeField] private float TextSpeed = 1.0f;
	[SerializeField] private float WaitTime = 5f;
	[SerializeField] private TextPlayer.TextingType TextingType = TextPlayer.TextingType.SecondsPerLetter;

	private RectTransform rectTransform;
	private Transform FollowObject;

	public TextPlayer TextPlayer => textPlayer;

	public event Action OnTextClosed;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="target">Либо Курсор, Либо объект на котором будет попап</param>
	public async void Init(string content, Transform followObject = null) 
	{
		FollowObject = followObject;
		rectTransform = GetComponent<RectTransform>();

		textPlayer.Init();
		textPlayer.OnTextClosed += Destroy;
		await textPlayer.SkipOrPlay(content, TextSpeed, WaitTime, TextingType);
		
	}

	private void Update()
	{
		Vector3 Direction = transform.position - Camera.main.transform.position;
		if (FollowObject != null)
		{
			transform.position = FollowObject.position;
		}
		transform.rotation = Quaternion.LookRotation(Direction);
	}

	private void Destroy()
	{
		OnTextClosed?.Invoke();
		Destroy(gameObject);
	}
}
