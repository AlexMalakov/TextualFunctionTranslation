using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public struct DialogueLine
{
    public string text;
    public string speaker;
    public bool on_the_left;
    public bool randomize;
    public bool permissionToScramble;
}

[System.Serializable]
public struct DialogueIconMap
{
    public string name;
    public Sprite icon;
}


public class Dialogue : MonoBehaviour
{
    [SerializeField] private List<DialogueLine> dialogueLines;
    [SerializeField] private GameManager gm;
    [SerializeField] private DialogueUiController dia;
    [SerializeField] private CameraRayCast camCast;

    private ReviewData? reviewDataSave;

    public void setDialogueLines(List<DialogueLine> lines) {
        this.dialogueLines = lines;
    }

    private int curLine = 0;
    private bool inDialogue = false;


    public void startDialogue(ReviewData? rd) {
        this.reviewDataSave = rd;
        this.inDialogue = true;
        this.camCast.diaLock(true);
        this.showLine();
    }

    public void Update() {
        if(inDialogue && Input.GetKeyDown(KeyCode.Space)) {
            this.nextLine();
        }
    }

    public void nextLine() {
        if(inDialogue) {
            this.curLine++;
            this.showLine();
        }
    }

    private void showLine() {
        if(this.curLine >= this.dialogueLines.Count) {
            this.endDialogue();
            return;
        }

        this.dia.displayLine(this.dialogueLines[this.curLine].on_the_left, this.dialogueLines[this.curLine].speaker, this.dialogueLines[this.curLine].text, this.dialogueLines[this.curLine].randomize, this.dialogueLines[this.curLine].permissionToScramble);
    }

    private void endDialogue() {
        this.curLine = 0;
        this.inDialogue = false;
        this.camCast.diaLock(false);
        this.dia.hideDialogue();
        gm.doneYapping(this.reviewDataSave);
    }
}
