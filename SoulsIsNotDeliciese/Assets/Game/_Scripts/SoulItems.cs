using DG.Tweening;
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
        transform.DOMove(intialPos, 1f);;
        transform.DORotateQuaternion(initialRot, 1f);
        GetComponent<Collider>().enabled = true;
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
        PopUpManager.instance?.Create(InitialSoul.name, InitialSoul.GetHoverReplices());
	}
}