using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardenTrait : InformationTrait
{
    public WardenTrait() : base("warden", 2) {}


    public virtual void onCombatStart(Unit traitHaver);

}
