using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;

	public void TriggerDialogue ()
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}

	public void next() {
		FindObjectOfType<DialogueManager>().DisplayNextSentence();
	}

	public bool hasNext() {
		return FindObjectOfType<DialogueManager>().hasMore();
	}

}
