using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockmakersTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float healthBoostAmount;
    [SerializeField] private Player traitPlayer; //continue where you left off this.combatManager.getBoard().getPlayerSquares()
    //this traits buffs stack :)

    public override void onCombatStart(Unit traitHaver){
        this.log.logEvent(new CombatEvent(traitHaver, traitHaver, CombatEventType.Start));
        this.applyInRadius(traitHaver, 1);
        // if(this.getNumberActive() > 2) {
        //     this.applyInRadius(traitHaver, 2);
        // }
    }

    private void applyInRadius(Unit traitHaver, int radius) {
        List<HexSquare> radCircle = this.getSquaresInRadius(traitHaver.getPosition(), radius, this.cm.getBoard());
        
        Operation buff = new Operation(OperationType.PreAdd, this.healthBoostAmount, "clockmaker health " + traitHaver.getName(), false);
        foreach(HexSquare s in radCircle) {
            if(s.getUnit() != null && this.cm.sameOwner(s.getUnit(), traitHaver)) {
                s.getUnit().buffStat(BuffableStats.Health, buff, traitHaver);
            }
        }

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        HexSquare prev = null;
        foreach(HexSquare s in radCircle) {
            if(prev != null) {
                cypher.Add(new CharacterTransformation(prev, s));
            }

            prev = s;
        }
        cypher.Add(new CharacterTransformation(prev, radCircle[0]));

        this.addToCypher(cypher);
    }

    public override string getTraitName() {
        return "clockmaker";
    }
    public override string getTraitNarrative() {
        return "Armorers, toolsmiths, and craftsman recruited famed for their strength and attention to detail.";
    }

    public override string getTraitEffect() {
        return ("Grants " + healthBoostAmount + "/TODO health and TODO/TODO attack speed to surrounding units.");
    }

    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }

}
