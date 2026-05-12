using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcastTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    public OutcastTrait() : base("outcast", 2) {}


    public virtual void onCombatStart(Unit traitHaver) {

        List<HexSquare> circle = this.getSquaresInRadius(traitHaver.getPosition(), 1, cm.getBoard());
        foreach(HexSquare s in circle) {
            if(s.getUnit() != null)
                return;
        }

        //TODO: give unit it's stat boost
        
        foreach(HexSquare s in circle) {
            this.logSquare(s);
        }
    }
  
}
