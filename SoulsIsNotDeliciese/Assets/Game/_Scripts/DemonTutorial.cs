using Unity.Cinemachine;
using UnityEngine;

public class DemonTutorial : MonoBehaviour
{
    public CinemachineCamera cameraSpirites;
    public CinemachineCamera cameraCandles;

    public void ShowSpirits()
    {
        cameraSpirites.Priority = 10;
        cameraCandles.Priority =  -10;
    }
    public void ReturnToNormal()
    {
        cameraCandles.Priority = -10;
        cameraCandles.Priority = -10;
    }
    public void ShowCandles()
    {
        cameraSpirites.Priority = -10;
        cameraCandles.Priority = 10;
    }
}
