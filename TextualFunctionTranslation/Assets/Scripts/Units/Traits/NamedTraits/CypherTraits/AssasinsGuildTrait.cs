using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssasinsGuildTrait : CypherTrait
{

    //on combat start, and on unit defeat?

    protected override string getTraitName() {
        return "assasins";
    }

    protected override int getMinimumForActive() {
        return 2;
    }
}
