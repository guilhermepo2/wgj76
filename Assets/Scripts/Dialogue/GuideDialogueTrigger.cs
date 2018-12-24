using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideDialogueTrigger : MonoBehaviour, IStartDialogue, IEndDialogue {
    public Dialogue dialogue;
    public Transform toPositionDuringDialogue;

    private bool m_isFirstTimeDialogue;

    void Start() {
        m_isFirstTimeDialogue = true;
    }

    public void TriggerDialogue() {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    void IStartDialogue.StartDialogue() {
        if(m_isFirstTimeDialogue) {
            m_isFirstTimeDialogue = false;
        }

        TriggerDialogue();
        GuideScript guide = GameObject.FindObjectOfType<GuideScript>();
        if(guide != null && toPositionDuringDialogue != null) {
            guide.overrideFollow = toPositionDuringDialogue;
        }
    }

    void IEndDialogue.EndDialogue() {
        GuideScript guide = GameObject.FindObjectOfType<GuideScript>();
        if(guide != null) {
            guide.overrideFollow = null;
        }

        DialogueManager.instance.EndDialogue();
    }
}
