using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct DialogueLine
{
    public string dialogue;
    public string speaker;
    public bool onTheLeft;
}


public class Dialogue : MonoBehaviour
{
    [SerializeField] private Dictionary<string, Sprite> nameToIcon;
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

        this.dialogue.text = this.dialogueLines[this.curLine].dialogue;
        this.talkerSprite.sprite = this.nameToIcon[this.dialogueLines[this.curLine].speaker];

        if(this.dialogueLines[this.curLine].onTheLeft) {
            this.dialogueBox.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);
        } else {
            this.dialogueBox.GetComponent<RectTransform>().localScale  = new Vector3(-1, 1, 1);
        }
    }

    private void endDialogue() {
        this.curLine = 0;
        this.inDialogue = false;
        this.dialogueBox.SetActive(false);
        gm.doneYapping(this.reviewDataSave);
    }
}
