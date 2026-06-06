using UnityEngine;

public class PopUpManager : MonoBehaviour
{
	public static PopUpManager instance;

	[SerializeField] private GameObject PopupPrefab;
	[SerializeField] private Transform Container;

	[SerializeField] private Transform GrandMomPivot, AksakalPivot, BatirPivot, DadPivot, GrandFatherPivot, MinistrelPivot;
	//public string content;

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
		var go = Instantiate(PopupPrefab, Container);
		go.transform.position = GetThePivot(soulName);
		var logic = go.GetComponent<PopUpLogic>();

		logic.Init(content);
	}

	private Vector3 GetThePivot(string name)
	{
		switch (name)
		{
			case "GrandMom":
				return GrandMomPivot.position;

			case "Aksakal":
				return AksakalPivot.position;

			case "Batir":
				return BatirPivot.position;

			case "Dad":
				return DadPivot.position;

			case "GrandFather":
				return GrandFatherPivot.position;

			case "Minstrel":
				return MinistrelPivot.position;
		}

		return Vector3.zero;
	}

}
