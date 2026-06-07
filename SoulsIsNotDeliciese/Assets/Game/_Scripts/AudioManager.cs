using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	public AudioSource audioSource;
	public AudioSource[] otherSounds;
	private List<float> volumes = new();
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
		for (int i = 0; i < otherSounds.Length; i++)
			volumes.Add(otherSounds[i].volume);
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
	public void SetAllSoundVolume(float volume)
	{
		audioSource.volume = volume;
		for(int i = 0; i < otherSounds.Length;i++)
		{
			otherSounds[i].volume = volumes[i] * volume;
		}
	}
}
[Serializable]
public class SoundPackage
{
	public AudioClip audioClip;
	public float volume;
}