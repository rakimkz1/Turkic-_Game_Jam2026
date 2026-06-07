using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


public class SoulItems : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Souls InitialSoul;
    public float distanceFromCamera;
    public SoundPackage grabSound;
    public SoundPackage throwSound;
    public SoundPackage itemSound;
    public GameObject discription;

    private Vector3 intialPos;
    private Quaternion initialRot;
    private bool isGrabed;
    private Camera mainCam;

    private void Start()
	{
		discription.SetActive(false);
		initialRot = transform.rotation;
        intialPos = transform.position;
        mainCam = Camera.main;
        InitialSoul.Reset();

        int activeState = 1;//PlayerPrefs.GetInt(InitialSoul.name, 1);
        if (activeState != 1)
        {
            gameObject.SetActive(false);
        }
    }

	private void OnDestroy()
	{
		int activeState = gameObject.activeSelf ? 1 : 0;
        PlayerPrefs.SetInt(InitialSoul.name, activeState);
	}

	public void Init(Souls soul)
    {
        InitialSoul = soul;
    }

    public void Grab()
    {
        isGrabed = true;
        GetComponent<Collider>().enabled = false;
        AudioManager.instance.PlayOneShot(grabSound);
		discription.SetActive(false); 
	}

    public void Return()
    {
        isGrabed = false;
        transform.DOMove(intialPos, 1f);;
        transform.DORotateQuaternion(initialRot, 1f);
        GetComponent<Collider>().enabled = true;
        AudioManager.instance.PlayOneShot(throwSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
	{
        PopUpManager.instance?.Create(InitialSoul.name, InitialSoul.GetHoverReplices());
        AudioManager.instance.PlayOneShot(itemSound);
        if (!isGrabed) { discription.SetActive(true); }
        
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		discription.SetActive(false);
	}
}