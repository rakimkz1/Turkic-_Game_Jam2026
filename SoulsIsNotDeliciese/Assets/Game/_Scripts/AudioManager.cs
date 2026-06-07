using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	public AudioSource audioSource;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(instance);
		}
	}
	public void PlayOneShot(SoundPackage sound)
	{
		audioSource.PlayOneShot(sound.audioClip, sound.volume);
	}
	public async void PlayOneShotDelay(SoundPackage sound, float delay)
	{
		await UniTask.Delay((int)(delay * 1000));
		audioSource.PlayOneShot(sound.audioClip, sound.volume);
	}
}
[Serializable]
public class SoundPackage
{
	public AudioClip audioClip;
	public float volume;
}