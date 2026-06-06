using System;
using System.Collections;
using UnityEngine;


public class SoulItems : MonoBehaviour
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
    }

    public void Init(Souls soul)
    {
        InitialSoul = soul;
    }

    public void Grab()
    {
        isGrabed = true;
        GetComponent<Collider>().isTrigger = true;
    }

    public void Return()
    {
        isGrabed = false;
        transform.position = intialPos;
        transform.rotation = initialRot;
    }
}