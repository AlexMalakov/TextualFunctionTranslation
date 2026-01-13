using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitNames {
    Anchors
}

[System.Serializable]
public class TraitBlueprint {
    public TraitNames trait;
    public Trait traitObj;
}


public class TraitFactory : MonoBehaviour
{
    [SerializeField] private List<TraitBlueprint> traitBP;
    private Dictionary<TraitNames, Trait> traitToObj;

    public void Awake() {
        foreach(TraitBlueprint tbp in this.traitBP) {
            this.traitToObj.Add(tbp.trait, tbp.traitObj);
        }
    }

    public Trait getTrait(TraitNames t) {
        return this.traitToObj[t];
    }
}
