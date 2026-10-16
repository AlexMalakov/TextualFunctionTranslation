using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryStage : CombatStage
{
    [SerializeField] TMP_Text victory_text;
    [SerializeField] GameObject victoryPopUp;

    public void displayVictory() {
        victory_text.text = "stand proud, you are strong";
        victoryPopUp.SetActive(true);
    }
}
