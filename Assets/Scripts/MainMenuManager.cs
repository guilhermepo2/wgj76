using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip beepSound;
    public AudioClip beepFinalSound;

    void Start() {
        SoundManager.instance.ChangeSfxSourceVolume(0.15f);
        StartCoroutine(BeepRoutine());
    }

    private IEnumerator BeepRoutine() {
        yield return new WaitForSeconds(1.0f);
        SoundManager.instance.PlaySfx(beepSound);
        StartCoroutine(BeepRoutine());
    }

    private IEnumerator WaitToNextScene() {
        LevelManager.instance.FadeIn(2.0f);
        yield return new WaitForSeconds(3.0f);
        SoundManager.instance.ChangeSfxSourceVolume(1f);
        LevelManager.instance.LoadNextLevel();
    }

    public void Play() {
        StopAllCoroutines();
        SoundManager.instance.PlaySfx(beepFinalSound);
        StartCoroutine(WaitToNextScene());
    }
}
