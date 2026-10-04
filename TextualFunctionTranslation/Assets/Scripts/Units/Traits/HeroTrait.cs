using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTrait : Trait
{
    public override string getTraitName() {
        return "hero";
    }

    public override int getMinimumForActive() {
        return 100;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{100};
    }

    public override string getTraitNarrative() {
        return "Savior of the kingdom.";
    }

    public override string getTraitEffect() {
        return (
            "Saves us all."
        );
    }
}
