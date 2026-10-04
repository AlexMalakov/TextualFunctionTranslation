using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitVLayout : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TraitCodexInfo tci;
    [SerializeField] private GameObject traitPrefab;
    [SerializeField] private GameObject traitParent;
    [SerializeField] private GameObject traitCodex;

    public void Start() {
        foreach(Trait t in this.player.getAllTraits()) {
            GameObject nt = Instantiate(this.traitPrefab, this.traitParent.transform);
            nt.GetComponent<TraitCodexButton>().init(t, tci, traitCodex);
        }

        this.tci.showTrait(this.player.getAllTraits()[0]);
    }
}
