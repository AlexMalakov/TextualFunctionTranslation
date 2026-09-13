using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelDialogue
{
    public List<SectionDialogue> levels;
}


[System.Serializable]
public struct SectionDialogue
{
    public string level_name;
    public List<DialogueLine> dialogue;
}

public class CombatStage : MonoBehaviour
{
    [SerializeField] private string plainTextCipher; //if you decompile this you are a poopoo head and also it won't help much brochacho
    [SerializeField] private PlayerUnitLayout opponentUnitLayout;
    [SerializeField] private int playerUnitCap;
    [SerializeField] private string stageName;
    [SerializeField] private int rewardMoney;

    [Header ("hex alphabets")]
    [SerializeField] private string alphabetOrderingPlayer;
    [SerializeField] private string alphabetOrderingEnemy;

    [Header ("optional dialogue")]
    [SerializeField] private Dialogue preStageDialogue;
    [SerializeField] private Dialogue postStageDialogue;

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

    public int getRewardMoney() {
        return this.rewardMoney;
    }

    public virtual bool hasDialogue(bool combatHappened) {
        return (combatHappened && this.postStageDialogue != null) || (!combatHappened && this.preStageDialogue != null);
    }

    public virtual Dialogue getDialogue(bool combatHappened) {
        return combatHappened ? this.postStageDialogue : this.preStageDialogue;
    }

    public void setDialogue(List<DialogueLine> dialogue) {
        this.preStageDialogue.setDialogueLines(dialogue);
    }
}
