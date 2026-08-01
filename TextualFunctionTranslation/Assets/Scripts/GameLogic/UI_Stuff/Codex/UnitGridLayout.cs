using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGridLayout : MonoBehaviour
{

    [SerializeField] private GameObject unitHolder;

    public void Start()
    {
        foreach(UnitBlueprint bp in UnitFactory.getAllBlueprints()) {
            GameObject newUnit = Instantiate(unitHolder, this.transform);
            newUnit.GetComponent<UnitCodexButton>().init(bp);
        }
    }
}
