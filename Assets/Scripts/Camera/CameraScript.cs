using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript instance;
    /* Camera Position */
    private Vector3 m_currentCameraPosition;

    /* Camera Transition */
    private const float transitionHorizontal = 25f;
    private const float transitionVertical = 14f;

    /* Screenshake */
    private Vector3 m_screenShake;
    private float m_screenShakeAmount;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        m_currentCameraPosition = transform.position;
    }

    private IEnumerator ShakeRoutine() {
        float timeElapsed = 0f;
        while(timeElapsed < m_screenShakeAmount) {
            m_screenShake = new Vector3(Random.Range(-m_screenShakeAmount, m_screenShakeAmount),
                                        Random.Range(-m_screenShakeAmount, m_screenShakeAmount),
                                        0f);
            transform.position += m_screenShake;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = m_currentCameraPosition;
    }

    public void ShakeScreen(float toShake) {
        m_screenShakeAmount = toShake;
        StartCoroutine(ShakeRoutine());
    }

    public void TransitCamera(Vector2 direction) {
        Vector3 tempPosition = transform.position;
        
        if(direction == Vector2.right) {
            tempPosition.x += transitionHorizontal;
        } else if(direction == Vector2.left) {
            tempPosition.x -= transitionHorizontal;
        } else if(direction == Vector2.up) {
            tempPosition.y += transitionVertical;
        } else if(direction == Vector2.down) {
            tempPosition.y -= transitionVertical;
        }

        transform.position = tempPosition;
        m_currentCameraPosition = transform.position;
    }
}
