using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DruidTrait : InformationTrait
{
    [SerializeField] private CombatManager combatManager;

    private List<HexSquare> vowelSquares;
    private HexSquare ySquare;

    //NOTE: THIS TRAIT IS ONLY USABLE FOR THE PLAYER
    public DruidTrait() : base("darouidyse", 2) {}
    


    public override void onPrecombatStart() { //this might run multiple times which may be annoying or slow | solution enemy wont have this trait ever
        vowelSquares = new List<HexSquare>();
        char[] vowels = {'a', 'e', 'i', 'o', 'u'};

        foreach (HexSquare s in this.combatManager.getBoard().getPlayerSquares()) {
            if (vowels.Contains(s.getCharValue())) {
                vowelSquares.Add(s);
            } else if(s.getCharValue() == 'y') {
                ySquare = s;
            }
        }

        //TODO: highlight squares
    }

    public override void onCombatStart(Unit traitHaver) {
        if (vowelSquares.Contains(traitHaver.getPosition())) {
            //TODO: apply stat boost
            return;
        } else if(traitHaver.getPosition() == ySquare) {
            //TODO: apply stat boost
            return;
        }
    }


}
