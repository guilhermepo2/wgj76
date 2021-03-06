﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
		public float fadeDuration = .1f;
		public int MaxFrameRate = 60;

		public MaskableGraphic blackScreenToFade;

		void Awake() {
			QualitySettings.vSyncCount = 0;
			StartCoroutine(changeFrameRate());

			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}
		}

		void Start() {
			FadeOut(fadeDuration);
		}

		public void ReloadLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

		public void LoadNextLevel() {
			int sceneCount = SceneManager.sceneCountInBuildSettings;
			int sceneToLoad = (SceneManager.GetActiveScene().buildIndex + 1) % sceneCount;
			SceneManager.LoadScene(sceneToLoad);
		}

		public void LoadLevel(string levelName) {
			if(Application.CanStreamedLevelBeLoaded(levelName)) {
				SceneManager.LoadScene(levelName);
			} else {
				Debug.LogError("LevelManager Error: invalid scene specified (" + levelName + " )");
			}
		}

		public void LoadLevel(int levelIndex) {
			Debug.Log("Load Level: Int");
			if(levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) {
				SceneManager.LoadScene(levelIndex);
			} else {
				Debug.LogError("Level Manager Error: invalid scene index specified (" + levelIndex + ")");
			}
		}

		// =============================================================================
		// =============================================================================
		// 							FADE IN / FADE OUT
		// =============================================================================
		// =============================================================================

		private void Fade(float targetAlpha, float duration) {
			blackScreenToFade.CrossFadeAlpha(targetAlpha, duration, true);
		}

		public void FadeOut(float duration) {
			if(blackScreenToFade == null) return;
			
			blackScreenToFade.canvasRenderer.SetAlpha(1f);
			Fade(0f, duration);
		}

		public void FadeIn(float duration) {
			if(blackScreenToFade == null) return;

			blackScreenToFade.canvasRenderer.SetAlpha(0f);
			Fade(1f, duration);
		}

		public IEnumerator changeFrameRate() {
			Application.targetFrameRate = MaxFrameRate;
			yield return new WaitForSeconds(1f);
		}
}
