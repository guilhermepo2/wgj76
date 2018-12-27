using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour, IInteract
{

    private IEnumerator LoadNextLevel() {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(1.5f);
        LevelManager.instance.LoadNextLevel();
    }
    void IInteract.Interact() {
        StartCoroutine(LoadNextLevel());
    }
}
