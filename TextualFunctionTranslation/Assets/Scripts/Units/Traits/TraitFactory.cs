using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitNames {
    Anchors,
    Darouidyse,
    Marksman,
    Outcast,
    Scout,
    Tracker,
    Warden
}

[System.Serializable]
public class TraitBlueprint {
    public TraitNames trait;
    public Trait traitObj;
}


public class TraitFactory : MonoBehaviour
{
    [SerializeField] private List<TraitBlueprint> traitBP;

    private static Dictionary<TraitNames, Trait> traitToObj = new Dictionary<TraitNames, Trait>();

    public void Awake() { //Awake because this needs to run first
        foreach(TraitBlueprint tbp in this.traitBP) {
            traitToObj.Add(tbp.trait, tbp.traitObj);
        }
    }

    public static Trait getTrait(TraitNames t) {
        return traitToObj[t];
    }
}
