using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtisansTrait : CypherTrait
{
    public ArtisansTrait() : base("artisans", 1) {}


    public virtual void onEnemyDefeat(Unit traitHaver, Unit target){
        //I need to think this trait through better
    }

}

