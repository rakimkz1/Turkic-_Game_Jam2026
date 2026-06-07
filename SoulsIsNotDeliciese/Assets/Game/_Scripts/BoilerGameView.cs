using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BoilerGameView : MonoBehaviour
{
    public Transform capTransform;
    public Vector3 capShakeStreight;
    public Vector3 capShakeRotate;
    public Transform capRemovePoint;
    public Transform soulItemThrowPoint;
    public SpriteRenderer blackBackGround;
    public Transform BoilGamePanel;
    public float throwPower;
    public AudioSource boilingSound;

    private Vector3 capInitialPos;
    private Quaternion capInitalRot;
    private Vector3 boilGamePanelScale;
    private Sequence capBoilingTween;
    private Sequence capOpenCloseTween;

    private void Start()
    {
        boilGamePanelScale = BoilGamePanel.transform.localScale;
        capInitalRot = capTransform.rotation;
        capInitialPos = capTransform.position;
    }
    public async UniTask ShowBoilingGamePanel()
    {
        DOTween.Complete("HideBoilingGamePanel");
        Sequence sequence = DOTween.Sequence();
        BoilGamePanel.gameObject.SetActive(true);
        sequence.Append(blackBackGround.DOFade(0.85f, 0.5f));
        sequence.Append(BoilGamePanel.DOScale(boilGamePanelScale, 1f).From(0f));
        await sequence.Play().SetId("ShowBoilingGamePanel").AsyncWaitForCompletion();
    }
    public async UniTask HideBoilingGamePanel()
    {
        Debug.Log("HideViewPanel");
        DOTween.Complete("ShowBoilingGamePanel");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(BoilGamePanel.DOScale(0f, 1f));
        sequence.Append(blackBackGround.DOFade(0f, 0.5f));
        await sequence.Play().SetId("HideBoilingGamePanel").OnComplete(() =>
        {
            BoilGamePanel.transform.localScale = boilGamePanelScale;
            BoilGamePanel.gameObject.SetActive(false);
        }).AsyncWaitForCompletion();
    }
    public void StartCapBoiling()
    {
        boilingSound.gameObject.SetActive(true);
        capBoilingTween?.Kill();
        capBoilingTween = DOTween.Sequence();
        capBoilingTween.Append(capTransform.DOShakePosition(0.5f, capShakeStreight, 10, 90, false).SetLoops(int.MaxValue).SetEase(Ease.Linear));
        capBoilingTween.Join(capTransform.DOShakeRotation(0.5f, capShakeRotate, 10, 90, false).SetLoops(int.MaxValue).SetEase(Ease.Linear));
        capBoilingTween.Play();
    }
    public void StopCapBoiling()
    {
        boilingSound.gameObject.SetActive(false);
        capBoilingTween?.Kill();
    }
    public async UniTask OpenCap()
    {
        capOpenCloseTween?.Kill();
        StopCapBoiling();
        Debug.Log("OpenCap");
        capOpenCloseTween = DOTween.Sequence();
        capOpenCloseTween.Append(capTransform.DOMove(capRemovePoint.position, 1f));
        capOpenCloseTween.Join(capTransform.DORotateQuaternion(capRemovePoint.rotation, 1f));
        await capOpenCloseTween.AsyncWaitForCompletion();
    }
    public async UniTask CloseCap()
    {
        Debug.Log("CloseCap");
        capOpenCloseTween?.Kill();
        capOpenCloseTween = DOTween.Sequence();
        capOpenCloseTween.Append(capTransform.DOMove(capInitialPos, 1f));
        capOpenCloseTween.Join(capTransform.DORotateQuaternion(capInitalRot, 1f));
        await capOpenCloseTween.AsyncWaitForCompletion();
        StartCapBoiling();
    }
    public async UniTask ThrowItemIntoCap(Transform soulsItem)
    {
        await soulsItem.DOJump(soulItemThrowPoint.position, throwPower, 1, 1f).OnComplete(() =>
        {
            soulsItem.gameObject.SetActive(false);
        }).AsyncWaitForCompletion();
    }
}