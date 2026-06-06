using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class BoilerManager : MonoBehaviour
{
    public bool isBoilderGamePlaying;
    public bool isCapControlable;
    public bool isSoulsRequires = true;
    public Action onBoiderGameStart;
    public Action onBoilerGameEnd;
    public GameObject boiderGamePanel;
    public GameObject caps;
    public float capMoveSpeed;
    public float capMoveLimits;
    public float capTiltSpeed;
    public float capMaxTiltAngle;
    public float damageFromCap;
    public SoulBall soulBall;
    [Header("SoulBallProperties")]
    public float soulBallSpeed;
    public float soulBallAccelerationPerHit;
    public float soulBallDamagePerHit;
    public float soulBallMaxHealth;
    public LayerMask capLayer;

    [SerializeField] private CameraMovement cameraMovement;
    public static BoilerManager Instance { get; private set; }
    private Souls _selectedSouls;
    private Camera mainCam;
    private Vector3 capInitialPos;
    private Vector3 capPrePos;
    private float _targetTilt;
    public BoilerGameView _view;

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
        _view = GetComponent<BoilerGameView>();
        _view.StartCapBoiling();
        mainCam = Camera.main;
        capPrePos = caps.transform.position;
        capInitialPos = caps.transform.position;
        soulBall.OnEscape += ExitBoilGame;
        soulBall.OnBoiled += ExitBoilGame;
        DayManager.instance.OnNewDay += () =>
        {
            isSoulsRequires = true;
        };
    }


    public async UniTask StartBoiderGame(Souls selectedSoul)
    {
        _selectedSouls = selectedSoul;
        cameraMovement.isMoveable = false;
        cameraMovement.ResetCamera();
        await _view.ShowBoilingGamePanel();
        isBoilderGamePlaying = true;
        onBoiderGameStart?.Invoke();
        SoulsManager.Instance.RemoveSouls(_selectedSouls);
        BonusApply();
        await ThrowSoul();
        isCapControlable = true;
    }
    private async void ExitBoilGame()
    {
        RemoveBonus();
        Debug.Log("GameExited");
        await _view.HideBoilingGamePanel();
        _view.CloseCap();
        cameraMovement.isMoveable = true;
        isBoilderGamePlaying = false;
        isCapControlable = false;
        caps.transform.position = capInitialPos;
        onBoilerGameEnd?.Invoke();
        if (DemonKvotaManager.instance.todaysKvota >= DemonKvotaManager.instance.maxKvota)
            Success();
        else
            LoseGame();
    }

    private void LoseGame()
    {
        Debug.Log("LoseGame");
    }

    private void Success()
    {
        isSoulsRequires = false;
        _view.StopCapBoiling();
    }

    private void BonusApply()
    {
        SoulsManager.Instance.activeSouls.ForEach(soul =>
        {
            soul.Bonus(this);
        });
    }
    private void RemoveBonus()
    {
        SoulsManager.Instance.activeSouls.ForEach(soul =>
        {
            soul.RemoveBonus(this);
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
        await UniTask.Delay(200);
        soulBall.gameObject.SetActive(true);
        soulBall.Init(soulBallSpeed, soulBallAccelerationPerHit, soulBallDamagePerHit, soulBallMaxHealth); 
        // Some Animation Here
        await UniTask.Delay(500);
        soulBall.isWorking = true;
    }
    private void ControlCap()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, capLayer);
        Vector3 mousePos = hit.point;
        mousePos.z = capInitialPos.z;
        mousePos.y = capInitialPos.y;
        //mousePos.x = Mathf.Clamp(mousePos.x, capInitialPos.x - capMoveLimits, capInitialPos.x + capMoveLimits);
        //caps.transform.position = Vector3.MoveTowards(caps.transform.position, mousePos, capMoveSpeed * Time.deltaTime);

        float clampedX = Mathf.Clamp(mousePos.x, -capMoveLimits + capInitialPos.x, capMoveLimits + capInitialPos.x);
        mousePos.x = clampedX;

        caps.transform.position = Vector3.Lerp(caps.transform.position, mousePos, capMoveSpeed * Time.deltaTime);

        // Скорость движения → наклон
        float speed = (caps.transform.position.x - capPrePos.x) / Time.deltaTime;
        _targetTilt = Mathf.Clamp(-speed * 3f, -capMaxTiltAngle, capMaxTiltAngle);
        capPrePos = caps.transform.position;

        float currentZ = caps.transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        float newZ = Mathf.Lerp(currentZ, _targetTilt,capTiltSpeed * Time.deltaTime);

        caps.transform.localEulerAngles = new Vector3(
            caps.transform.localEulerAngles.x,
            caps.transform.localEulerAngles.y,
            newZ);
    }
}