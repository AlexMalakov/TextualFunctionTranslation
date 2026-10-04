using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CypherTextDisplay : MonoBehaviour
{
    [SerializeField] private RandTMP cyphertext;


    public void setCypherText(string msg) {
        this.cyphertext.setText(msg);
    }

    public bool toggleDisplay() {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        return this.gameObject.activeSelf;
    }

    public void setDisplay(bool setMe) {
        this.gameObject.SetActive(setMe);
    }
}
