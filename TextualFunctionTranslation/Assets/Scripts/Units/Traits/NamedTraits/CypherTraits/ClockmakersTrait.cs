using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockmakersTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;


    public override void onCombatStart(Unit traitHaver){

        //TODO: determine if radius 1 or 1 and 2

        List<HexSquare> rad1circle = this.getSquaresInRadius(traitHaver.getPosition(), 1, this.cm.getBoard());
        
        // foreach(HexSquare s in rad1circle) {
        //     //TODO: buff every unit in circle
        // }

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        HexSquare prev = null;
        foreach(HexSquare s in rad1circle) {
            if(prev != null) {
                cypher.Add(new CharacterTransformation(s, prev));
            }

            prev = s;
        }
        cypher.Add(new CharacterTransformation(rad1circle[0], prev));

        this.addToCypher(cypher);
    }

    protected override string getTraitName() {
        return "clockmaker";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

}
