using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioSource itemAudio;
	public AudioSource grabAudio;
	public AudioSource throwAudio;

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

	public void PlayItemAudio()
	{
		PlayAudio(itemAudio);
	}

	public void PlayGrabAudio()
	{
		PlayAudio(grabAudio);
	}

	public void PlayThrowAudio()
	{
		PlayAudio(throwAudio);
	}

	private void PlayAudio(AudioSource audio)
	{
		if (!audio.isPlaying)
		{
			audio.Stop();
		}
		audio.Play();
	}
}
