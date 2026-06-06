using TMPro;
using UnityEngine;


public class InfoPanel : MonoBehaviour
{
    public TextMeshProUGUI currentDayText;

    private void Start()
    {
        DayManager.instance.OnNewDay += ShowInfo;
    }

    public void ShowInfo()
    {
        currentDayText.text = $"Day {DayManager.instance.currentDay + 1}";
    }
}
