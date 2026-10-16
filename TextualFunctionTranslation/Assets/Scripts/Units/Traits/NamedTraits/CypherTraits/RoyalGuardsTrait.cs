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
            if(!isLetter(cypherText[i])) {
                continue;
            }
            int caseBase = isUpper(cypherText[i]) ? 'A' : 'a';
            outputArr[i] = (char)((((int)cypherText[i] - caseBase + this.key) % 26) + caseBase);
        }
    }

    private bool isLetter(char c) {
        return ((int)c >= (int)'a' && (int)c <= (int)'z') || ((int)c >= (int)'A' && (int)c <= (int)'Z');
    }

    private bool isUpper(char c) {
        return (int)'A' <= (int)c && (int)c <= (int)'Z';
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
        float amt = this.getNumberActive() > 3 ? this.count * this.healthPerFullCompStack : this.count * this.healthPerStack;
        Operation buff = new Operation(OperationType.PreAdd, amt, "royal guard health", true);
        traitHaver.buffStat(BuffableStats.Health, buff, traitHaver);

        amt = this.getNumberActive() > 3 ? this.count * this.damagePerFullCompStack : this.count * this.damagePerStack;
        buff = new Operation(OperationType.PreAdd, amt, "royal guard damage", true);
        traitHaver.buffStat(BuffableStats.Damage, buff, traitHaver);

        this.startCount = this.count;
    }

    public override void onEnemyDefeat(Unit traitHaver, Unit target){
        this.log.logEvent(new CombatEvent(traitHaver, target, CombatEventType.Kill));
        this.count += (int) (this.getNumberActive() > 3 ? this.killStackGainLarge : this.killStackGainSmall);
        if(this.traitHolder is HumanPlayer) {
            this.pi.setGloryStacks(this.count);
        } else {
            this.pi.setEnemyGloryStacks(this.count);
        }
    }

    public override void onCombatSurvival(Unit traitHaver){

        this.log.logEvent(new CombatEvent(traitHaver, traitHaver, CombatEventType.Survival));
        this.count += (int) (this.getNumberActive() > 3 ? this.surviveStackGainLarge : this.surviveStackGainSmall);
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
        return "ceasarian legionaries";
    }

    public override string getTraitNarrative() {
        return "The elite personal guard of the monarchy, famed for their skill, experience, and code.";
    }

    public override string getTraitEffect() {
        return ("Gains " + this.killStackGainSmall+"/"+this.killStackGainLarge + " stacks of glory on kill, and " + surviveStackGainSmall+"/"+surviveStackGainLarge
                +" stacks of glory on battle survival. Glory stacks carry across succesful combats. Gain " + healthPerStack+"/"+healthPerFullCompStack 
                + " health and " +damagePerStack+"/"+damagePerFullCompStack + " damage per glory stack.");
    }

    public override int getMinimumForActive() {
        return 2;
    }


    public void resetStacks() {
        this.count = this.startCount;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2,4};
    }
}
