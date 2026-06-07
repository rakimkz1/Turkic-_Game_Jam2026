using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


public class SoulItems : MonoBehaviour, IPointerEnterHandler
{
    public Souls InitialSoul;
    public float distanceFromCamera;
    public SoundPackage grabSound;
    public SoundPackage throwSound;
    public SoundPackage itemSound;

    private Vector3 intialPos;
    private Quaternion initialRot;
    private bool isGrabed;
    private Camera mainCam;

    private void Start()
    {
        initialRot = transform.rotation;
        intialPos = transform.position;
        mainCam = Camera.main;
        InitialSoul.Reset();

        int activeState = PlayerPrefs.GetInt(InitialSoul.name, 1);
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
	}
}