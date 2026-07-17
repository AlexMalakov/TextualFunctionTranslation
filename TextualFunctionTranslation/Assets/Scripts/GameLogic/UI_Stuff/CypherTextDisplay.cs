using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CypherTextDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text cyphertext;


    public void setCypherText(string msg) {
        this.cyphertext.text = msg;
    }

    public bool toggleDisplay() {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        return this.gameObject.activeSelf;
    }

    public void setDisplay(bool setMe) {
        this.gameObject.SetActive(setMe);
    }
}
