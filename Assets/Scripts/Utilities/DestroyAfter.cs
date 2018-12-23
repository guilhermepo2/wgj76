using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float destroyAfter = 1.0f;

    void Start() {
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine() {
        yield return new WaitForSeconds(destroyAfter);
        Destroy(gameObject);
    }
}
