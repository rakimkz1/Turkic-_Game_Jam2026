using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	public Transform target;
	public Transform image;

    void Update()
    {
		Vector2 targetPosition = Camera.main.WorldToScreenPoint(target.position);
		image.position = targetPosition;
		Debug.Log(targetPosition);
	}
}
