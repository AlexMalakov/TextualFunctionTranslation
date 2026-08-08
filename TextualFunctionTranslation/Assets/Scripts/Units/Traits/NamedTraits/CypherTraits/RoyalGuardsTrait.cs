using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalGuardsTrait : CypherTrait
{
    [SerializeField] private PlayerInfo pi;
    [SerializeField] private CombatManager cm;
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
    //TODO: how does this work for the enemy player...
    //TODO: on combat loss reset glory stacks

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
        this.pi.setGloryStacks(this.count);
    }

    public override void onCombatSurvival(Unit traitHaver){
        this.count += (int) (this.getNumberActive() > 2 ? this.surviveStackGainLarge : this.surviveStackGainSmall);
        this.pi.setGloryStacks(this.count);
    }

    public override void onCombatEnd(Unit traitHaver){
        List<HexSquare> ps = this.cm.getBoard().getPlayerSquares();
        Dictionary<char, HexSquare> charToSq = new Dictionary<char, HexSquare>();

        foreach(HexSquare s in ps) {
            charToSq.Add(s.getCharValue(), s);
        }

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        foreach(char c in charToSq.Keys) {
            char toC = (char)((((int)c + this.count) % 26 ) + (int)'a');
            cypher.Add(new CharacterTransformation(charToSq[toC], charToSq[c]));
        }

        this.addToCypher(cypher);
    }

    public override string getTraitName() {
        return "royal guards";
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
