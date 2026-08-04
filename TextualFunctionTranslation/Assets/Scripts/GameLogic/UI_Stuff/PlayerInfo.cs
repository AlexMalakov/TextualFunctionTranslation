using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text cur_level;
    [SerializeField] private TMP_Text glory_stacks;
    [SerializeField] private TMP_Text dollarie_doos;

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
