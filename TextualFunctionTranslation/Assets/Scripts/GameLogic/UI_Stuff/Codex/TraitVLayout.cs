using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitVLayout : MonoBehaviour
{
    [SerializeField] private GameObject traitPrefab;
    [SerializeField] private List<Trait> roleTraits;
    [SerializeField] private GameObject roleParent;
    [SerializeField] private List<Trait> classTraits;
    [SerializeField] private GameObject classParent;

    public void Start() {
        foreach(Trait t in this.roleTraits) {
            GameObject nt = Instantiate(this.traitPrefab, this.roleParent.transform);
            nt.GetComponent<TraitCodexButton>().init(t);
        }

        foreach(Trait t in this.classTraits) {
            GameObject nt = Instantiate(this.traitPrefab, this.classParent.transform);
            nt.GetComponent<TraitCodexButton>().init(t);
        }
    }
}
