using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    public TrackerTrait() : base("tracker", 1) {}


    public virtual void onCombatEnd(Unit traitHaver);

}
