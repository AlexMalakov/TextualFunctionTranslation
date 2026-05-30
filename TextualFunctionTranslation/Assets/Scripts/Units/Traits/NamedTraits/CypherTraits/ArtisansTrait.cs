using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtisansTrait : CypherTrait
{


    public override void onEnemyDefeat(Unit traitHaver, Unit target){
        //I need to think this trait through better
    }

    public override string getTraitName() {
        return "artisans";
    }

    protected override int getMinimumForActive() {
        return 1;
    }

}

