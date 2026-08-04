using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    [SerailizeField] private TMP_Text cur_level;
    [SerailizeField] private TMP_Text glory_stacks;
    [SerailizeField] private TMP_Text dollarie_doos;




    public void setLevel(string level_name) {
        this.cur_level.text = level_name;
    }

    public void setGloryStacks(int stackCount) {
        this.glory_stacks.text = stackCount + "";
    }

    public void setDollars(int dollary_doos) {
        this.dollarie_doos.text = dollary_doos + "";
    }
}
