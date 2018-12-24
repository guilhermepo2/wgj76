using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideDialogueTrigger : MonoBehaviour {
    public Dialogue dialogue;

    public void TriggerDialogue() {
        DialogueManager.instance.StartDialogue(dialogue);
    }
}
