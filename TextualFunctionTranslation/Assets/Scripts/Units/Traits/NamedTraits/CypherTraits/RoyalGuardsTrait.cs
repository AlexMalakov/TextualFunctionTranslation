using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//faking another cypher can't be bothered doing it legit is super ass
class CaesarCharacterTransformation : CharacterTransformation {
    private int key;

    public CaesarCharacterTransformation(int key) : base(null, null) {
        this.key = key;
    }

    public override void apply(char[] outputArr, char[] cypherText) {
        Debug.Log("Caesar encryption: key" + this.key + " | input: " + new string(cypherText));
        for(int i = 0; i < cypherText.Length; i++) {
            outputArr[i] = (char)((((int)cypherText[i] + this.key - (int)'a') % 26) + (int)'a');
        }
    }
}

public class RoyalGuardsTrait : CypherTrait
{
    [SerializeField] private PlayerInfo pi;
    [SerializeField] private CombatManager cm;
    [SerializeField] private Player traitHolder;

    [SerializeField] private float healthPerStack;
    [SerializeField] private float healthPerFullCompStack;
    [SerializeField] private float damagePerStack;
    [SerializeField] private float damagePerFullCompStack;

    [SerializeField] private float killStackGainSmall = 2f;
    [SerializeField] private float killStackGainLarge = 5f;
    [SerializeField] private float surviveStackGainSmall = 5f;
    [SerializeField] private float surviveStackGainLarge = 8f;
    private int count = 0;
    private int startCount = 0;
    //TODO: shouldnt persist across different runs
    //TODO: on combat loss reset glory stacks

    public void loadLevelStacks(int loadStacks) {
        this.count = loadStacks;
        this.startCount = this.count;
    }

    public override void onCombatStart(Unit traitHaver){
        float amt = this.getNumberActive() > 2 ? this.count * this.healthPerFullCompStack : this.count * this.healthPerStack;
        Operation buff = new Operation(OperationType.PreAdd, amt, "royal guard health");
        traitHaver.buffStat(BuffableStats.Health, buff, traitHaver);

        amt = this.getNumberActive() > 2 ? this.count * this.damagePerFullCompStack : this.count * this.damagePerStack;
        buff = new Operation(OperationType.PreAdd, amt, "royal guard damage");
        traitHaver.buffStat(BuffableStats.Damage, buff, traitHaver);

        this.startCount = this.count;
    }

    public override void onEnemyDefeat(Unit traitHaver, Unit target){
        this.count += (int) (this.getNumberActive() > 2 ? this.killStackGainLarge : this.killStackGainSmall);
        if(this.traitHolder is HumanPlayer) {
            this.pi.setGloryStacks(this.count);
        } else {
            this.pi.setEnemyGloryStacks(this.count);
        }
    }

    public override void onCombatSurvival(Unit traitHaver){
        this.count += (int) (this.getNumberActive() > 2 ? this.surviveStackGainLarge : this.surviveStackGainSmall);
        if(this.traitHolder is HumanPlayer) {
            this.pi.setGloryStacks(this.count);
        } else {
            this.pi.setEnemyGloryStacks(this.count);
        }
    }

    public override void onCombatEnd() {
        CharacterTransformation salad = new CaesarCharacterTransformation(this.count);
        this.addToCypher(new List<CharacterTransformation>(){salad});
    }

    public override string getTraitName() {
        return "royal guard";
    }

    public override string getTraitDescription() {
        return ("An elite regiment of the kings own guards, and veterans of many of the kingdoms combats. Gains "
            + healthPerStack + "/" + healthPerFullCompStack + " health and " + damagePerStack + "/" + damagePerFullCompStack
            + " damage per stack of glory. For each succesfull combat, gain stacks carrying over into all future combats in the run. "
            + " Gain " + this.killStackGainSmall + "/" + this.killStackGainLarge + " glory for each royal guard final blow, and "
            + surviveStackGainSmall + "/" + surviveStackGainLarge + " glory for each surviving royal guard after a combat.");

    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2,4};
    }
}
