using Unity.Cinemachine;
using UnityEngine;

public class DemonTutorial : MonoBehaviour
{
    public CinemachineCamera cameraSpirites;
    public CinemachineCamera cameraCandles;

    public void ShowSpirits()
    {
        Debug.Log("ShowSpirits");
        cameraSpirites.Priority = 10;
        cameraCandles.Priority =  -10;
    }
    public void ReturnToNormal()
	{
		Debug.Log("ReturnToNormal");
		cameraSpirites.Priority = -10;
        cameraCandles.Priority = -10;
    }
    public void ShowCandles()
	{
		Debug.Log("ShowCandles");
		cameraSpirites.Priority = -10;
        cameraCandles.Priority = 10;
    }
}
