using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandTMP : MonoBehaviour
{
    public string text;
    [SerializeField] TMP_Text tmp;

    public void setText(string txt) {
        this.text = txt;
        this.tmp.text = txt;
    }

    public void scramble() {
        this.tmp.text = GameManager.scrambleString(this.text);
    }

    public void reset() {
        this.tmp.text = this.text;
    }
}
