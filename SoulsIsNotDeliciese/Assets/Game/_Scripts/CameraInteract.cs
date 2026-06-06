using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class CameraInteract : MonoBehaviour
{
    public bool isInteractable = true;
    public LayerMask interactMask;
    public static CameraInteract Instance { get; private set; }
    private Camera mainCamera;
    private bool isGrabingSoul;
    private SoulItems currentSouls;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(isInteractable && !isGrabingSoul && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, Mathf.Infinity, interactMask)){
            if (hit.collider != null && hit.collider.gameObject.tag == "Soul")
            {
                currentSouls = hit.collider.GetComponent<SoulItems>();
                currentSouls.Grab();
                if(BoilerManager.Instance.isSoulsRequires == true)
                    BoilerManager.Instance._view.OpenCap();
                isGrabingSoul = true;
            }
            if(hit.collider != null && hit.collider.gameObject.tag == "Kalendar")
            {
                hit.collider.GetComponent<Kalendar>().Activate();
            }
        }
        if (isGrabingSoul)
        {
            Vector3 soulPos = ray.direction * currentSouls.distanceFromCamera + mainCamera.transform.position;

            if (Physics.Raycast(ray, out hit))
            {
                currentSouls.transform.position = ((soulPos - currentSouls.transform.position).sqrMagnitude > (hit.point - currentSouls.transform.position).sqrMagnitude) ? hit.point : soulPos;
            }
            else
                currentSouls.transform.position = soulPos;

            if (Input.GetMouseButtonUp(0))
            {
                if (hit.collider != null && hit.collider.tag == "Boiler" && BoilerManager.Instance.isSoulsRequires)
                {
                    ThrowItem();
                }
                else
                {
                    currentSouls.Return();
                    isGrabingSoul = false;
                    BoilerManager.Instance._view.CloseCap();
                }
            }
        }
    }

    private async UniTask ThrowItem()
    {
        isGrabingSoul = false;
        await BoilerManager.Instance._view.ThrowItemIntoCap(currentSouls.transform);
        BoilerManager.Instance.StartBoiderGame(currentSouls.InitialSoul);
    }
}