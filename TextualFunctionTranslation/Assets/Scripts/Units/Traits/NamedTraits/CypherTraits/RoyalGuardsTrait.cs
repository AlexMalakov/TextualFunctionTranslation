using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalGuardsTrait : CypherTrait
{
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
    //TODO: shouldnt persist across different runs

    public override void onCombatStart(Unit traitHaver){
        float amt = this.getNumberActive() > 2 ? this.count * this.healthPerFullCompStack : this.count * this.healthPerStack;
        Operation buff = new Operation(OperationType.PreAdd, amt, "royal guard health");
        traitHaver.buffStat(BuffableStats.Health, buff);

        amt = this.getNumberActive() > 2 ? this.count * this.damagePerFullCompStack : this.count * this.damagePerStack;
        buff = new Operation(OperationType.PreAdd, amt, "royal guard damage");
        traitHaver.buffStat(BuffableStats.Damage, buff);
    }

    public override void onEnemyDefeat(Unit traitHaver, Unit target){
        this.count += (int) (this.getNumberActive() > 2 ? this.killStackGainLarge : this.killStackGainSmall);
    }

    public override void onCombatSurvival(Unit traitHaver){
        this.count += (int) (this.getNumberActive() > 2 ? this.surviveStackGainLarge : this.surviveStackGainSmall);
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

    protected override int getMinimumForActive() {
        return 2;
    }
}
