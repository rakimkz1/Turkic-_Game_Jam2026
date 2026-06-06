using System.Collections.Generic;
using UnityEngine;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager Instance { get; private set; }
    public List<Souls> soulsList = new();
    public List<Souls> activeSouls = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RemoveSouls(Souls souls)
    {
        activeSouls.Remove(souls);
    }
}