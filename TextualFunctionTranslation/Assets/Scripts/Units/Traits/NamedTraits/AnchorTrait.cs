using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stub for the anchor trait -- this trait is not going to get used
public class AnchorTrait : Trait
{

    protected override string getTraitName() {
        return "outcast";
    }

    protected override int getMinimumForActive() {
        return 2;
    }
}
