using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public struct DialogueLine
{
    public string text;
    public string speaker;
    public bool on_the_left;
    public string randomize;
}

[System.Serializable]
public struct DialogueIconMap
{
    public string name;
    public Sprite icon;
}


public class Dialogue : MonoBehaviour
{
    [SerializeField] private List<DialogueIconMap> nameToIcon;
    [SerializeField] private List<DialogueLine> dialogueLines;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Image talkerSprite;
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameManager gm;

    private ReviewData? reviewDataSave;

    public void Awake() {
        dialogueBox.SetActive(false);
    }

    public void setDialogueLines(List<DialogueLine> lines) {
        this.dialogueLines = lines;
    }

    private int curLine = 0;
    private bool inDialogue = false;


    public void startDialogue(ReviewData? rd) {
        this.reviewDataSave = rd;
        this.inDialogue = true;
        dialogueBox.SetActive(false);
        this.showLine();
    }

    public void Update() {
        if(inDialogue && Input.GetKeyDown(KeyCode.Space)) {
            this.curLine++;
            this.showLine();
        }
    }

    private void showLine() {
        if(this.curLine >= this.dialogueLines.Count) {
            this.endDialogue();
            return;
        }

        this.dialogue.text = this.dialogueLines[this.curLine].text;
        this.talkerSprite.sprite = this.nameToIcon.First(x => x.name == this.dialogueLines[this.curLine].speaker).icon;

        if(this.dialogueLines[this.curLine].on_the_left) {
            this.talkerSprite.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);
        } else {
            this.talkerSprite.GetComponent<RectTransform>().localScale  = new Vector3(-1, 1, 1);
        }
    }

    private void endDialogue() {
        this.curLine = 0;
        this.inDialogue = false;
        this.dialogueBox.SetActive(false);
        gm.doneYapping(this.reviewDataSave);
    }
}
