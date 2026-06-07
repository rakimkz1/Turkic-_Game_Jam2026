using UnityEngine;
using UnityEngine.EventSystems;

public class SkipDemonText : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private Demon demon;

	public void OnPointerClick(PointerEventData eventData)
	{
		demon.Skip();
	}
}
