using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndManager : MonoBehaviour {
    public Text deathCount;
    public Text jumpCount;
    public Text dashCount;

    void Start() {
        if(StatisticsCollector.instance) {
            deathCount.text = "deaths: " + StatisticsCollector.instance.deathCount;
            jumpCount.text = "jumps: " + StatisticsCollector.instance.jumpCount;
            dashCount.text = "dashes: " + StatisticsCollector.instance.dashCount;
            Destroy(StatisticsCollector.instance.gameObject);
        } else {
            deathCount.text = "no deaths";
            jumpCount.text = "no jumps";
            dashCount.text = "no dashes";
        }
    }
}
