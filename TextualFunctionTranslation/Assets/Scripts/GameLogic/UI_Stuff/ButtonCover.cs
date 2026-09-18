using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCover : MonoBehaviour
{
    private bool flashing = false;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color flashColor;
    [SerializableField] private Image image; 

    public void flashButton(bool isFlashing) {
        this.flashing = isFlashing;

        if(isFlashing) {
            this.image.color = flashColor;
            this.flash();
        } else {
            this.unflash();
        }
    }

    private void flash() {
        if(!this.flashing) {
            return;
        }
        this.image.enabled = false;
        Invoke(nameof(unflash), .5f);
    }

    private void unflash() {

        this.image.enabled = false;

        if(this.flashing) {
            Invoke(nameof(flash), .5f);
        }
    }

    public bool coverButton(bool buttonIsEnabled) {
        this.image.color = disabledColor;
        this.image.enabled = !buttonIsEnabled;
    }
}
