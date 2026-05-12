using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockmakersTrait : CypherTrait
{
    public ClockmakersTrait() : base("clockmaker", 2) {

    }

    public virtual void onCombatStart(Unit traitHaver){
        
    }

    private List<HexSquare> getSquaresInRadius(HexSquare center, int radius) {
        
    }
}
