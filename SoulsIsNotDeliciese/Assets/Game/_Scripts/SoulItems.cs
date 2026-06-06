using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class SoulItems : MonoBehaviour, IPointerEnterHandler
{
    public Souls InitialSoul;
    public float distanceFromCamera;

    private Vector3 intialPos;
    private Quaternion initialRot;
    private bool isGrabed;
    private Camera mainCam;

    private void Start()
    {
        initialRot = transform.rotation;
        intialPos = transform.position;
        mainCam = Camera.main;
        InitialSoul.Reset();
    }

    public void Init(Souls soul)
    {
        InitialSoul = soul;
    }

    public void Grab()
    {
        isGrabed = true;
        GetComponent<Collider>().enabled = false;
    }

    public void Return()
    {
        isGrabed = false;
        transform.position = intialPos;
        transform.rotation = initialRot;
        GetComponent<Collider>().enabled = true;
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
        PopUpManager.instance?.Create(InitialSoul.name, InitialSoul.GetHoverReplices());
	}
}