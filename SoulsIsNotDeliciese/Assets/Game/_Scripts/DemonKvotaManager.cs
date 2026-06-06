using UnityEngine;
using UnityEngine.UI;

public class DemonKvotaManager : MonoBehaviour
{
    public static DemonKvotaManager instance;

    public float todaysKvota = 0f;
    public float maxKvota = 100f;

    public Image kvotaBar;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        DayManager.instance.OnNewDay += () =>
        {
            todaysKvota = 0f;
        };
    }

    public void AddKvota(float kvota)
    {
        todaysKvota += kvota;
        kvotaBar.fillAmount = todaysKvota / maxKvota;
    }
}