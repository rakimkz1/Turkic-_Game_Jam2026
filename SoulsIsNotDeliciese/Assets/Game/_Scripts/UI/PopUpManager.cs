using UnityEngine;

public class PopUpManager : MonoBehaviour
{
	public static PopUpManager instance;

	[SerializeField] private GameObject PopupPrefab;
	[SerializeField] private Transform Container;

	[SerializeField] private Transform GrandMomPivot, AksakalPivot, BatirPivot, DadPivot, GrandFatherPivot, MinistrelPivot;

	private bool GrandMomPopUpIsActive, AksakalPopUpIsActive, BatirPopUpIsActive, DadPopUpIsActive, GrandFatherPopUpIsActive, MinistrelPopUpIsActive;

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

	public void Create(string soulName, string content)
	{
		if (!Check(soulName)) { return; }

		var go = Instantiate(PopupPrefab, Container);
		var logic = go.GetComponent<PopUpLogic>();
		go.transform.position = GetThePivot(soulName, logic);

		logic.Init(content);
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

	private Vector3 GetThePivot(string name, PopUpLogic logic)
	{
		switch (name)
		{
			case "GrandMom":
				GrandMomPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => GrandMomPopUpIsActive = false;
				return GrandMomPivot.position;

			case "Aksakal":
				AksakalPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => AksakalPopUpIsActive = false;
				return AksakalPivot.position;

			case "Batir":
				BatirPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => BatirPopUpIsActive = false;
				return BatirPivot.position;

			case "Dad":
				DadPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => DadPopUpIsActive = false;
				return DadPivot.position;

			case "GrandFather":
				GrandFatherPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => GrandFatherPopUpIsActive = false;
				return GrandFatherPivot.position;

			case "Minstrel":
				MinistrelPopUpIsActive = true;
				logic.TextPlayer.OnTextClosed += () => MinistrelPopUpIsActive = false;
				return MinistrelPivot.position;
		}

		return Vector3.zero;
	}

}
