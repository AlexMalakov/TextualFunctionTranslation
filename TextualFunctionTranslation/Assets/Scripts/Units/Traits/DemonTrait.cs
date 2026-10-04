using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonTrait : Trait
{
    public override string getTraitName() {
        return "demon";
    }

    public override int getMinimumForActive() {
        return 100;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{100};
    }

    public override string getTraitNarrative() {
        return "The demon lord, and his trusted few.";
    }

    public override string getTraitEffect() {
        return (
            "Differs based on the foe you face."
        );
    }
}

