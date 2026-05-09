using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    public ScoutTrait() : base("scout", 2) {}



    public virtual void onFirstAttackAgainstUnit(Unit traitHaver, Unit target);

}
