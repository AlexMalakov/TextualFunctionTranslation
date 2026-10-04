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
    [SerializeField] private int enemyGloryStacks;
    [SerializeField] private ShopRarity rarity; 
    [SerializeField] private ShopPool pool;
    private int seed;

    [Header ("hex alphabets")]
    [SerializeField] private string alphabetOrderingPlayer;
    [SerializeField] private string alphabetOrderingEnemy;

    [Header ("optional dialogue")]
    [SerializeField] private Dialogue preStageDialogue;
    [SerializeField] private Dialogue postStageDialogue;

    //this is fine it sets the random seed only
    public void Awake() {
        this.seed = System.DateTime.Now.Millisecond * (int)this.stageName[this.stageName.Length - 1];
        this.onAwake();
    }

    protected virtual void onAwake() {}

    public ShopRarity setShopOdds() {
        this.pool.initFromLevel(this.rarity, this.seed);
        return this.rarity;
    }

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

    public int getEnemyGloryStacks() {
        return this.enemyGloryStacks;
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
