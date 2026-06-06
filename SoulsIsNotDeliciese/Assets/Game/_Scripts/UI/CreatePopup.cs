using UnityEngine;

public class CreatePopup : MonoBehaviour
{
	[SerializeField] private GameObject PopupPrefab;
	[SerializeField] private Transform Container;
	public string content;

	[ContextMenu("Create Popup")]
	public void Create()
	{
		var go = Instantiate(PopupPrefab, Container);
		var logic = go.GetComponent<PopUpLogic>();

		logic.Init(content);
	}
}
