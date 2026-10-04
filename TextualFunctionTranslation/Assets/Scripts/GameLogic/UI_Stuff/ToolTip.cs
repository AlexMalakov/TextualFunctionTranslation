using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private string tooltip;

    public string getToolTip(){
        return this.tooltip;
    }
}
