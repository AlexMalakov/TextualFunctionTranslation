using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text levelMoney;
    [SerializeField] private TMP_Text unitCap;

    [SerializeField] private TMP_Text odds2;
    [SerializeField] private TMP_Text odds3;
    [SerializeField] private TMP_Text odds4;
    [SerializeField] private TMP_Text odds5;


    //TODO shop odds

    public void onLevelStart(int reward, int curUnits, int unitCap, ShopRarity rarity) {
        this.levelMoney.text = "+" + reward;
        this.unitCap.text = curUnits + "/" + unitCap;

        this.odds2.text = "<color=#CF9F07>2</color> - " + rarity.chance2 + "%";
        this.odds3.text = "<color=#CF9F07>3</color> - " + rarity.chance3 + "%";
        this.odds4.text = "<color=#CF9F07>4</color> - " + rarity.chance4 + "%";
        this.odds5.text = "<color=#CF9F07>5</color> - " + rarity.chance5 + "%";

        //text = text + "" + prevTier + "</color> / ";
    }

    public void onUnitLayoutUpdate(int curUnits, int unitCap) {
        this.unitCap.text = curUnits + "/" + unitCap;
    }
}
