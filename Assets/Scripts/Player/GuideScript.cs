using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideScript : MonoBehaviour
{
    public Transform toFollow;
    private float m_internalAngle;
    private const float followSpeed = 5f;
    public Transform overrideFollow = null;

    void Update() {
        Vector3 tempPosition = toFollow.position;

        if(overrideFollow != null) {
            tempPosition = overrideFollow.position;
        }

        /* dunno what was happening... */
        tempPosition.z = 0f;
        // /\ nice

        tempPosition.y += (Mathf.Sin(m_internalAngle * Mathf.Deg2Rad) / 4f);
        
        m_internalAngle = ((m_internalAngle + 1f) % 360);

        float t = Mathf.Clamp(followSpeed * Time.deltaTime, 0, 1);
        t = Interpolation.EaseIn(t);

        transform.position = Vector3.Lerp(transform.position, tempPosition, t);
    }

    void OnTriggerEnter2D() {
        Debug.Log("Guide is on Trigger");
    }
}
