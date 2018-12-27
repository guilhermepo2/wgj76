using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioSource musicSource;
	public AudioSource sfxSource;
	public AudioClip defaultMusic;
	
	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}
	public void ChangeMusicWithoutFade(AudioClip newMusic) {
		if(instance == null || musicSource == null) {
			Debug.LogError("There's no sound manager or no music source!");
			return;
		}

		musicSource.Stop();
		musicSource.clip = newMusic;
		musicSource.Play();
	}

	public void ReturnToDefaultMusic() {
		ChangeMusicWithoutFade(defaultMusic);
	}

	// SFX

	public void PlaySfx(AudioClip clip) {
		if(instance == null || musicSource == null) {
			Debug.LogError("There's no sound manager or no SFX source!");
			return;
		}
		
		sfxSource.PlayOneShot(clip);
	}

	public void ChangeMusicSourceVolume(float volume) {
		musicSource.volume = volume;
	}

	public void ChangeSfxSourceVolume(float volume) {
		sfxSource.volume = volume;
	}
}
