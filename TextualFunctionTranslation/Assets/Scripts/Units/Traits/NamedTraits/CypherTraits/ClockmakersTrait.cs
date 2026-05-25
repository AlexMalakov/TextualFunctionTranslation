using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockmakersTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float healthBoostAmount;
    //this traits buffs stack :)

    public override void onCombatStart(Unit traitHaver){
        this.applyInRadius(1);
        if(this.getNumberActive() > 2) {
            this.applyInRadius(2);
        }
    }

    private void applyInRadius(Unit traitHaver, int radius) {
        List<HexSquare> radCircle = this.getSquaresInRadius(traitHaver.getPosition(), radius, this.cm.getBoard());
        
        Operation buff = new Operation(OperationType.PreAdd, this.healthBoostAmount, "clockmaker health " + traitHaver.getName());
        foreach(HexSquare s in radcircle) {
            if(s.getUnit() != null) {
                s.getUnit(BuffableStats.Health, buff)
            }
        }

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        HexSquare prev = null;
        foreach(HexSquare s in radcircle) {
            if(prev != null) {
                cypher.Add(new CharacterTransformation(s, prev));
            }

            prev = s;
        }
        cypher.Add(new CharacterTransformation(radcircle[0], prev));

        this.addToCypher(cypher);
    }

    protected override string getTraitName() {
        return "clockmaker";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

}
