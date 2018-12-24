using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager instance;
    [Header("UI Elements")]
    public GameObject dialogueObject;
    public Text dialogueText;
    [Header("Config")]
    public bool isShowingDialogue;

    private Queue<string> m_sentences;
    private string m_currentSentenceBeingTyped;
    private bool m_isTypingSentence;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        m_sentences = new Queue<string>();
        dialogueObject.SetActive(false);
    }

    void Update() {
        if(Input.GetButtonDown("Submit")) {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue) {
        dialogueObject.SetActive(true);
        m_sentences.Clear();
        if(dialogue.dialogueClip) SoundManager.instance.PlaySfx(dialogue.dialogueClip);

        foreach(string sentence in dialogue.sentences) {
            m_sentences.Enqueue(sentence);
        }

        isShowingDialogue = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if(m_isTypingSentence) {
            StopAllCoroutines();
            dialogueText.text = m_currentSentenceBeingTyped;
            m_isTypingSentence = false;
            return;
        }

        if(m_sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = m_sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence) {
        m_isTypingSentence = true;
        m_currentSentenceBeingTyped = sentence;
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
        }
        
        m_isTypingSentence = false;
    }

    public void EndDialogue() {
        isShowingDialogue = false;
        dialogueObject.SetActive(false);
    }

}
