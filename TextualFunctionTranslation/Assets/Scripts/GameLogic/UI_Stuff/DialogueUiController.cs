using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DialogueUiController : MonoBehaviour
{
    [SerializeField] private List<DialogueIconMap> nameToIcon;
    [SerializeField] private GameObject leftObject;
    [SerializeField] private GameObject rightObject;
    [SerializeField] private GameObject belowBar;

    [SerializeField] private Image rightImage;
    [SerializeField] private Image leftImage;

    [SerializeField] private RandDialogueTMP rightText;
    [SerializeField] private RandDialogueTMP leftText;

    private bool randomize = false;
    private int FRAMES_TILL_RAND = 15;
    private int randCounter = 0;

    public void displayLine(bool left, string iconToShow, string text, bool rnd, bool permissionToScramble) {
        this.randomize = rnd;
        this.leftText.allowedToScramble(permissionToScramble);
        this.rightText.allowedToScramble(permissionToScramble);
        this.leftText.setPermaScramble(rnd);
        this.rightText.setPermaScramble(rnd);

        if(left) {
            this.leftObject.SetActive(true);
            this.rightObject.SetActive(false);
            this.belowBar.SetActive(true);

            this.leftImage.sprite = this.nameToIcon.First(x => x.name == iconToShow).icon;
            this.leftText.setText(text);
        } else {
            this.leftObject.SetActive(false);
            this.rightObject.SetActive(true);
            this.belowBar.SetActive(true);

            Debug.Log("MATCH FOR icon :" + iconToShow);
            this.rightImage.sprite = this.nameToIcon.First(x => x.name == iconToShow).icon;
            this.rightText.setText(text);
        }
    }

    public void hideDialogue() {
        this.leftObject.SetActive(false);
        this.rightObject.SetActive(false);
        this.belowBar.SetActive(false);
    }

    public void Update() {
        if(this.belowBar.activeInHierarchy && this.randomize) {
            if(this.randCounter % this.FRAMES_TILL_RAND == 0) {
                this.scrambleAllTMP();
            }
            
            randCounter = (randCounter + 1) % this.FRAMES_TILL_RAND;
        }
    }

    private void scrambleAllTMP() {
        rightText.scramble();
        leftText.scramble();
    }
}

    