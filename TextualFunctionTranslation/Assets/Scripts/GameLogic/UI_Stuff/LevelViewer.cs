using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text levelMoney;
    [SerializeField] private TMP_Text unitCap;


    //TODO shop odds

    public void onLevelStart(int reward, int curUnits, int unitCap) {
        this.levelMoney.text = reward + "";
        this.unitCap.text = curUnits + "/" + unitCap;
    }

    public void onUnitLayoutUpdate(int curUnits, int unitCap) {
        this.unitCap.text = curUnits + "/" + unitCap;
    }
}
