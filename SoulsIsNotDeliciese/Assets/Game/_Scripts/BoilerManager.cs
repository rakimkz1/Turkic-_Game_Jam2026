using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class BoilerManager : MonoBehaviour
{
    public bool isBoilderGamePlaying;
    public bool isCapControlable;
    public Action onBoiderGameStart;
    public Action onBoilerGameEnd;
    public GameObject boiderGamePanel;
    public GameObject caps;
    public float capMoveSpeed;
    public float capMoveLimits;
    public SoulBall soulBall;
    [Header("SoulBallProperties")]
    public float soulBallSpeed;
    public float soulBallAccelerationPerHit;
    public float soulBallDamagePerHit;

    public static BoilerManager Instance { get; private set; }
    private Souls _selectedSouls;
    private Camera mainCam;
    private Vector3 capInitialPos;

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
    private void Start()
    {
        mainCam = Camera.main;
        capInitialPos = caps.transform.position;
        soulBall.OnEscape += ExitBoilGame;
    }


    public async UniTask StartBoiderGame(Souls selectedSoul)
    {
        _selectedSouls = selectedSoul;
        await ShowBoiderGamePanel();
        isBoilderGamePlaying = true;
        onBoiderGameStart?.Invoke();
        SoulsManager.Instance.RemoveSouls(_selectedSouls);
        BonusApply();
        await ThrowSoul();
        isCapControlable = true;
    }
    private async void ExitBoilGame()
    {
        await HideBoildGamePanel();
        isBoilderGamePlaying = false;
        isCapControlable = false;
        caps.transform.position = capInitialPos;
        onBoilerGameEnd?.Invoke();
    }

    private async UniTask HideBoildGamePanel()
    {
        await UniTask.Delay(1000);
        //Here Some Animation
        boiderGamePanel.SetActive(false);
    }

    private void BonusApply()
    {
        SoulsManager.Instance.activeSouls.ForEach(soul =>
        {
            soul.Bonus();
        });
    }

    private void Update()
    {
        if (isCapControlable)
        {
            ControlCap();
        }
    }

    private async UniTask ThrowSoul()
    {
        soulBall.gameObject.SetActive(true);
        soulBall.Init(soulBallSpeed, soulBallAccelerationPerHit, soulBallDamagePerHit); 
        // Some Animation Here
        await UniTask.Delay(1000);
    }

    private async UniTask ShowBoiderGamePanel()
    {
        //Some Visual Effect Here
        boiderGamePanel.SetActive(true);
        await UniTask.Delay(1000);
    }
    private void ControlCap()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.y = capInitialPos.y;
        mousePos.x = Mathf.Clamp(mousePos.x, capInitialPos.x - capMoveLimits, capInitialPos.x + capMoveLimits);
        caps.transform.position = Vector3.MoveTowards(caps.transform.position, mousePos, capMoveSpeed * Time.deltaTime);
    }
}
