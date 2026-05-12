using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalGuardsTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    private int count;

    public RoyalGuardsTrait() : base("royal guards", 2) {
        this.count = 0;
    }

    public virtual void onCombatStart(Unit traitHaver){
        //TODO: BUFF UNITS BASED ON COUNT
    }

    public virtual void onEnemyDefeat(Unit traitHaver, Unit target){
        this.count += 2; //TODO: give bigger buff if more traits
    }

    public virtual void onCombatSurvival(Unit traitHaver){
        this.count += 5; //TODO: give bigger buff if more traits
    }

    public virtual void onCombatEnd(Unit traitHaver){
        List<HexSquare> ps = this.cm.getBoard().getPlayerSquares();
        Dictionary<char, HexSquare> charToSq = new Dictionary<char, HexSquare>();

        foreach(HexSquare s in ps) {
            charToSq.Add(s.getCharValue(), s);
        }

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        foreach(char c in charToSq.Keys()) {
            char toC = (char)((((int)c + this.count) % 26 ) + (int)'a');
            cypher.Add(new CharacterTransformation(charToSq[toC], charToSq[c]));
        }

        this.addToCypher(cypher);
    }

    /*
Vegenary - on combat end
Rotator - on combat start 
royal knights - on unit defeat, on combat survive
plug - on unit defeat
assasin - on combat end
flank guard - on combat start
    */
}
