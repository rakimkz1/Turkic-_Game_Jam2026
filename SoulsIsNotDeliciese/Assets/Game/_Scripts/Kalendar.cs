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
    public GameObject gameOverPanel;
    public SoundPackage reject;
    public AudioSource GameOverSound;
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
        else if(DemonKvotaManager.instance.todaysKvota >= DemonKvotaManager.instance.maxKvota && DayManager.instance.currentDay == 3 && !_isAnim)
        {
            gameOverPanel.SetActive(true);
            AudioManager.instance.SetAllSoundVolume(0f);
            GameOverSound.Play();
        }
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
        DOTween.To(() => AudioManager.instance.audioSource.volume, x => AudioManager.instance.SetAllSoundVolume(x), 0f, candleBreath.audioClip.length).SetEase(Ease.InQuad);

        blackBackgroundPanel.color = Color.black;
        candleCam.Priority = -10;
        candles[DayManager.instance.currentDay].SetActive(false);
        await UniTask.Delay(1000);
        nextDayText.text = $"Days {DayManager.instance.currentDay + 2}";
        sequence = DOTween.Sequence();
        sequence.Append(nextDayText.DOFade(1f, 1f).From(0f));
        sequence.Insert(3f, nextDayText.DOFade(0f, 1f));
        sequence.Join(blackBackgroundPanel.DOFade(0f, 1f));
        sequence.Join(DOTween.To(() => AudioManager.instance.audioSource.volume, x => AudioManager.instance.SetAllSoundVolume(x), 1f, 3f));
        sequence.Play().OnComplete(() =>
        {
            Debug.Log("Complete");
            DayManager.instance.StartNewDay();
            _isAnim = false;
        });
    }
    private void Reject()
    {
        AudioManager.instance.PlayOneShot(reject);
    }
}