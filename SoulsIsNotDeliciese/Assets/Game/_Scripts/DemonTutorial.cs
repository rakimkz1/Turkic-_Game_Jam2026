using Unity.Cinemachine;
using UnityEngine;

public class DemonTutorial : MonoBehaviour
{
    public CinemachineCamera cameraSpirites;
    public CinemachineCamera cameraCandles;

    public void ShowSpirits()
    {
        cameraSpirites.Priority = 10;
        cameraCandles.Priority = -10;
        Debug.Log("Spirites");
    }
    public void ReturnToNormal()
    {
        cameraCandles.Priority = -10;
        cameraCandles.Priority = -10;
        Debug.Log("Return");
    }
    public void ShowCandles()
    {
        Debug.Log("Candles");
        cameraSpirites.Priority = -10;
        cameraCandles.Priority = 10;
    }
}
