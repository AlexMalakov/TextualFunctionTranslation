using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcastTrait : InformationTrait
{
    public OutcastTrait() : base("outcast", 2) {}


    public virtual void onCombatStart(Unit traitHaver);


  
}
