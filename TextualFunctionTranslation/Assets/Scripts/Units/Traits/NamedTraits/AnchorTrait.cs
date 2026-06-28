using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stub for the anchor trait -- this trait is not going to get used
public class AnchorTrait : Trait
{

    public override string getTraitName() {
        return "outcast";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1,2,3,4};
    }
}
