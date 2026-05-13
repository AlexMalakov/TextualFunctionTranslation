using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityWatchTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    public CityWatchTrait() : base("city watch", 2) {}

    
    public override void onCombatStart(Unit traitHaver){
        HexSquare start = traitHaver.getPosition();
        List<Unit> leftBuff = this.getAllUnitsInDirection(start, -1);
        List<Unit> rightBuff = this.getAllUnitsInDirection(start, 1);

        //TODO: give left units their buff, give right units their buff
        
        List<CharacterTransformation> loopdeloop = new List<CharacterTransformation>();
        HexSquare rightEdge;

        while(true) {
            AxialCoord sCords = start.getCoords();
            HexSquare next = this.cm.getBoard().getHexAt(new AxialCoord(sCords.q + 1, sCords.r, sCords.s - 1));

            if(!next.isSquare()) {
                rightEdge = start;
                break;
            }

            loopdeloop.Add(new CharacterTransformation(next, start));
            start = next;
        }

        HexSquare leftEdge;
        while(true) {
            AxialCoord sCords = start.getCoords();
            HexSquare next = this.cm.getBoard().getHexAt(new AxialCoord(sCords.q - 1, sCords.r, sCords.s + 1));

            if(!next.isSquare()) {
                leftEdge = start;
                break;
            }

            loopdeloop.Add(new CharacterTransformation(start, next));
            start = next;
        }

        loopdeloop.Add(new CharacterTransformation(leftEdge, rightEdge));
        this.addToCypher(loopdeloop);
    }

    private List<Unit> getAllUnitsInDirection(HexSquare start, int direction) {

        List<Unit> allUnits = new List<Unit>();
        while(true) {
            AxialCoord sCords = start.getCoords();
            start = this.cm.getBoard().getHexAt(new AxialCoord(sCords.q + direction, sCords.r, sCords.s - direction));

            if(!start.isSquare())
                return allUnits;

            if(start.getUnit() != null)
                allUnits.Add(start.getUnit());
        }
    }
}
