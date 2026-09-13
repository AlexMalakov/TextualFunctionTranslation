using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct ReviewData {
    public bool playerWon;
    public bool randomizerSuvived;

    public ReviewData(bool playerWon, bool randomizerSuvived) {
        this.playerWon = playerWon;
        this.randomizerSuvived = randomizerSuvived;
    }
}

public class ReviewManager : MonoBehaviour
{

    [SerializeField] private GameStateButton gameStateButton;
    [SerializeField] private CypherPopUpButton cypherButton;
    [SerializeField] private CypherTextDisplay display;
    [SerializeField] private ResultsRaycast moviePlayer;
    [SerializeField] private LenseButton lb;
    [SerializeField] private List<TMP_Text> scramblableItems;
    [SerializeField] private LenseItem li;

    private bool isScrambling = false;
    private int randCounter = 0;
    private int FRAMES_TILL_RAND = 5;

    public void onResultsStart(string txt, ReviewData data) {
        this.gameStateButton.runResults(true, data.playerWon);
        this.display.setCypherText(txt);
        this.lb.showButton(true);
        this.isScrambling = data.randomizerSuvived;

        if(this.isScrambling) {
            this.scrambleAllTMP();
        }
    }


    public void handleResults() {
        this.moviePlayer.checkUserInteraction();
        this.li.checkUserInteraction();

        if(this.isScrambling) {
            if(this.randCounter % this.FRAMES_TILL_RAND == 0) {
                this.scrambleAllTMP();
            }
            
            randCounter = (randCounter + 1) % this.FRAMES_TILL_RAND;
        }
    }

    private void scrambleAllTMP() {
        foreach(TMP_Text t in this.scramblableItems) {
            t.text = this.scrambleString(t.text);
        }
    }

    private string scrambleString(string input) {
        char[] chars = input.ToCharArray();

        for (int i = 0; i < chars.Length; i++) {
            if (chars[i] >= 'a' && chars[i] <= 'z') {
                chars[i] = (char)Random.Range('a', 'z' + 1);
            } else if (chars[i] >= 'A' && chars[i] <= 'Z') {
                chars[i] = (char)Random.Range('A', 'Z' + 1);
            }
        }

        return new string(chars);
    }

    public void onResultsEnd() {
        this.gameStateButton.runResults(false, true);
        this.cypherButton.setDisplay(false);
        this.display.setDisplay(false);
        this.moviePlayer.runResults(false);
        this.lb.showButton(false);
    }

    public void displayReview() {
        this.cypherButton.setDisplay(true);
        this.display.setDisplay(true);
        this.moviePlayer.runResults(true);
    }

    public List<TMP_Text> getRandomizerText() {
        return this.scramblableItems;
    }
}
