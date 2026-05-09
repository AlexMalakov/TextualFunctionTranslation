using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardenTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    public WardenTrait() : base("warden", 2) {}


    public virtual void onCombatStart(Unit traitHaver) {
        AxialCoord pos = traitHaver.getPosition().getCoords();
        HexSquare leftSquare;
        HexSquare rightSquare;
        
        if(cm.isPlayerUnit(traitHaver)) { //just dont use r
            leftSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q - 1, pos.r + 1, pos.s));
            rightSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q, pos.r + 1, pos.s - 1));
        } else {
            leftSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q + 1, pos.r - 1, pos.s));
            rightSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q, pos.r - 1, pos.s + 1));
        }

        if(leftSquare.isSquare()) {
            //todo boost unit if it's there
            this.logSquare(leftSquare);
        }

        if(rightSquare.isSquare()) {
            //todo boost unit if it's there
            this.logSquare(rightSquare);
        }
    }

}
