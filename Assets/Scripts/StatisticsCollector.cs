using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsCollector : MonoBehaviour
{
    public static StatisticsCollector instance;

    /* Statistics */
    public int deathCount;
    public int jumpCount;
    public int dashCount;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            deathCount = 0;
            jumpCount = 0;
            dashCount = 0;
        } else {
            Destroy(gameObject);
        }
    }
}
