using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStage : MonoBehaviour
{
    [SerializeField] private string plainTextCipher; //if you decompile this you are a poopoo head and also it won't help much brochacho
    [SerializeField] private PlayerUnitLayout opponentUnitLayout;
    [SerializeField] private int playerUnitCap;
    [SerializeField] private string stageName;

    [SerializeField] string alphabetOrderingPlayer;
    [SerializeField] string alphabetOrderingEnemy;


    //TODO: each stage could have dialogue?
    //TODO: when saving is implemented, each stage shoudl also have an identifier for ezpzlemonsqueezy

    public PlayerUnitLayout getCombatLayout() {
        return this.opponentUnitLayout;
    }

    public int getMaxUnits() {
        return this.playerUnitCap;
    }

    public string getLevelCipher() {
        return this.plainTextCipher;
    }

    public string getAlphabet() {
        return this.alphabetOrderingEnemy + this.alphabetOrderingPlayer;
    }

    public string getStageName() {
        return this.stageName;
    }
}
