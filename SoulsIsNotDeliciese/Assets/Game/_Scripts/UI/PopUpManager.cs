using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Unity.Hierarchy;
using UnityEngine;
using System.Collections.Generic;

public class PopUpManager : MonoBehaviour
{
	public static PopUpManager instance;

	[SerializeField] private GameObject PopupPrefab;
	[SerializeField] private Transform Container;

	[SerializeField] private Transform GrandMomPivot, AksakalPivot, BatirPivot, DadPivot, GrandFatherPivot, MinistrelPivot;

	private bool GrandMomPopUpIsActive, AksakalPopUpIsActive, BatirPopUpIsActive, DadPopUpIsActive, GrandFatherPopUpIsActive, MinistrelPopUpIsActive;
	public Souls GrandMomPop, AksakalPop, BatirPop, DadPop, GrandFatherPop, MinistrelPop;

	private int popupCounts = 0;

	private int idleCount = 2;
	private int hoverCount = 3;
	private int dragCount = 3;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		if (popupCounts < idleCount)
		{
			CreateIdleReplice(4);
		}
	}

	public void HoverCreate(string soulName, string content)
	{
		if (popupCounts >= hoverCount) return;
		Create(soulName, content);
	}

	public void GrabCreate(string soulName, string content)
	{
		if (popupCounts >= dragCount)
		{
			return;
		}

		Create(soulName, content);
	}

	public void Create(string soulName, string content)
	{
		if (!Check(soulName)) { return; }

		var go = Instantiate(PopupPrefab, Container);
		var logic = go.GetComponent<PopUpLogic>();
		var transform = GetThePivot(soulName, logic);
		go.transform.position = transform.position;
		popupCounts++;
		logic.OnTextClosed += () => popupCounts--;

		logic.Init(content, transform);
	}

	public async void CreateIdleReplice(float duration)
	{
		float time = 0;
		while (time < duration)
		{
			time += Time.deltaTime;
			await UniTask.Yield();
		}
		if (popupCounts < idleCount)
		{
			var soul = GetRandomSouls();
			Create(soul.name, soul.GetIdleReplices());
		}

	}

	public string[] GetAvailableNames()
	{
		List<string> result = new List<string>();
		
		if (!GrandMomPopUpIsActive && !GrandMomPop.SoulTalking)
		{
			result.Add("GrandMom");
		}
		if (!AksakalPopUpIsActive && !AksakalPop.SoulTalking)
		{
			result.Add("Aksakal");
		}
		if (!BatirPopUpIsActive && !BatirPop.SoulTalking)
		{
			result.Add("Batir");
		}
		if (!DadPopUpIsActive && !DadPop.SoulTalking)
		{
			result.Add("Dad");
		}
		if (!GrandFatherPopUpIsActive && !GrandFatherPop.SoulTalking)
		{
			result.Add("GrandFather");
		}
		if (!MinistrelPopUpIsActive && !MinistrelPop.SoulTalking)
		{
			result.Add("Minstrel");
		}

		return result.ToArray();
	}

	public Souls GetRandomSouls()
	{
		List<Souls> result = new List<Souls>();

		if (!GrandMomPopUpIsActive && !GrandMomPop.SoulTalking)
		{
			result.Add(GrandMomPop);
		}
		if (!AksakalPopUpIsActive && !AksakalPop.SoulTalking)
		{
			result.Add(AksakalPop);
		}
		if (!BatirPopUpIsActive && !BatirPop.SoulTalking)
		{
			result.Add(BatirPop);
		}
		if (!DadPopUpIsActive && !DadPop.SoulTalking)
		{
			result.Add(DadPop);
		}
		if (!GrandFatherPopUpIsActive && !GrandFatherPop.SoulTalking)
		{
			result.Add(GrandFatherPop);
		}
		if (!MinistrelPopUpIsActive && !MinistrelPop.SoulTalking)
		{
			result.Add(MinistrelPop);
		}
		if (result.Count == 0)
		{
			Debug.Log("SOUL IS ZERO");
			return null;
		}

		return result[UnityEngine.Random.Range(0, result.Count)];
	}

	private bool Check(string name)
	{
		switch (name)
		{
			case "GrandMom":
				return !GrandMomPopUpIsActive;

			case "Aksakal":
				return !AksakalPopUpIsActive;

			case "Batir":
				return !BatirPopUpIsActive;

			case "Dad":
				return !DadPopUpIsActive;

			case "GrandFather":
				return !GrandFatherPopUpIsActive;

			case "Minstrel":
				return !MinistrelPopUpIsActive;
		}
		return true;
	}
	private Souls GetSoul(string name)
	{
		switch (name)
		{
			case "GrandMom":
				return GrandMomPop;

			case "Aksakal":
				return AksakalPop;

			case "Batir":
				return BatirPop;

			case "Dad":
				return DadPop;

			case "GrandFather":
				return GrandFatherPop;

			case "Minstrel":
				return MinistrelPop;
		}
		return null;
	}

	private Transform GetThePivot(string name, PopUpLogic logic)
	{
		switch (name)
		{
			case "GrandMom":
				GrandMomPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => GrandMomPopUpIsActive = false;
				return GrandMomPivot;

			case "Aksakal":
				AksakalPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => AksakalPopUpIsActive = false;
				return AksakalPivot;

			case "Batir":
				BatirPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => BatirPopUpIsActive = false;
				return BatirPivot;

			case "Dad":
				DadPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => DadPopUpIsActive = false;
				return DadPivot;

			case "GrandFather":
				GrandFatherPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => GrandFatherPopUpIsActive = false;
				return GrandFatherPivot;

			case "Minstrel":
				MinistrelPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => MinistrelPopUpIsActive = false;
				return MinistrelPivot;
		}

		return null;
	}

}
