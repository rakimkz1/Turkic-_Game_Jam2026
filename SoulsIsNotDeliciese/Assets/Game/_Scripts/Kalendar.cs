using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;


public class Kalendar : MonoBehaviour
{
    public GameObject[] candles;
    public CinemachineCamera candleCam;
    public Image blackBackgroundPanel;
    public TextMeshProUGUI nextDayText;
    public SoundPackage candleBreath;
    public SoundPackage reject;
    private Sequence sequence;
    private bool _isAnim;

    private void Start()
    {
        sequence = DOTween.Sequence();
    }
    public void Activate()
    {
        if (DemonKvotaManager.instance.todaysKvota >= DemonKvotaManager.instance.maxKvota && DayManager.instance.currentDay < 3 && !_isAnim)
            StartAnim();
        else
            Reject();
    }

    private async UniTask StartAnim()
    {
        Debug.Log("Kalendar work");
        _isAnim = true;
        candleCam.Priority = 10;
        await UniTask.Delay(2000);
        AudioManager.instance.PlayOneShot(candleBreath);
        blackBackgroundPanel.color = Color.black;
        candleCam.Priority = -10;
        candles[DayManager.instance.currentDay].SetActive(false);
        await UniTask.Delay(1000);
        nextDayText.text = $"Days {DayManager.instance.currentDay + 2}";
        sequence = DOTween.Sequence();
        sequence.Append(nextDayText.DOFade(1f, 1f).From(0f));
        sequence.SetDelay(500);
        sequence.Append(nextDayText.DOFade(0f, 1f));
        sequence.Join(blackBackgroundPanel.DOFade(0f, 1f));
        sequence.Play().OnComplete(() =>
        {
            DayManager.instance.StartNewDay();
            _isAnim = false;
        });
    }
    private void Reject()
    {
        
        AudioManager.instance.PlayOneShot(reject);
    }
}