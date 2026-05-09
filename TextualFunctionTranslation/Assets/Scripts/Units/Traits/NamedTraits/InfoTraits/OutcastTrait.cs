using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcastTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    public OutcastTrait() : base("outcast", 2) {}


    public virtual void onCombatStart(Unit traitHaver) {

        AxialCoord center = traitHaver.getPosition().getCoords();
        AxialCoord[] possibleNeighbors = {
            new AxialCoord(center.q + 1, center.r - 1, center.s),
            new AxialCoord(center.q + 1, center.r, center.s - 1),
            new AxialCoord(center.q - 1, center.r + 1, center.s),
            new AxialCoord(center.q, center.r + 1, center.s - 1),
            new AxialCoord(center.q - 1, center.r, center.s + 1),
            new AxialCoord(center.q, center.r - 1, center.s + 1),
        };

        foreach(AxialCoord c in possibleNeighbors) {
            HexSquare sq = cm.getBoard().getHexAt(c);
            if(!sq.isSquare()) {
                return;
            }
        }

        //TODO: give unit it's stat boost
        foreach(AxialCoord c in possibleNeighbors) {
            this.logSquare(cm.getBoard().getHexAt(c));
        }
    }
  
}
