using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public Text nameText;
	public Text dialogueText;

	public GameObject dialogBox;


	private Queue<string> sentences;

	private bool hasNext = false;

	public string nextScene;

	public float time_increment;

	// Use this for initialization
	void Start () {
		sentences = new Queue<string>();
		StartCoroutine("gameClock");

	}

	    IEnumerator gameClock() {
        GameObject progressBar = GameObject.Find("progressbar");
        while (progressBar.GetComponent<Image>().fillAmount < 1f) {

          	progressBar.GetComponent<Image>().fillAmount += time_increment;
            yield return new WaitForSeconds(1f);
			Debug.Log("One second");
			// Debug.Log("Prog bar: " + progressBar.GetComponent<Image>().fillAmount);
        }

    }

	public void StartDialogue (Dialogue dialogue)
	{
		dialogBox.SetActive(true);
		hasNext = true;

		nameText.text = dialogue.name;

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		// StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	public bool hasMore() {
		return hasNext;
	}

	void EndDialogue()
	{
		hasNext = false;
		dialogBox.SetActive(false);
		StartCoroutine("gameClock");
	}

}
