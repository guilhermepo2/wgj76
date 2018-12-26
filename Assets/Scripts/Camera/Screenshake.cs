using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour
{
    public static Screenshake instance;
    public float idleAmplitude = 0f;
    public float shakeAmplitude = 3f;

    Cinemachine.CinemachineBasicMultiChannelPerlin m_perlin;
    Cinemachine.CinemachineVirtualCamera m_virtualCamera;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        m_virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        m_perlin = FindObjectOfType<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        ResetCamera();
    }

    private IEnumerator ShakeRoutine(float duration, float shakeAmplitude) {
        Vector3 cameraPositionBeforeShake = transform.position;
        yield return null;
        m_perlin.m_AmplitudeGain = shakeAmplitude;
        yield return new WaitForSeconds(duration);
        ResetCamera();
        transform.position = cameraPositionBeforeShake;
    }

    public void ShakeCamera(float duration) {
        StartCoroutine(ShakeRoutine(duration, shakeAmplitude));
    }

    public void ResetCamera() {
        m_perlin.m_AmplitudeGain = idleAmplitude;
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }
}
