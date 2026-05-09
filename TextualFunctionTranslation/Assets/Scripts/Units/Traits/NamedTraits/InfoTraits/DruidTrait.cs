using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidTrait : InformationTrait
{
    public DruidTrait() : base("darouidyse", 2) {}



    public override void onPrecombatStart(Unit traitHaver);

    public override void onCombatStart(Unit traitHaver);


}
