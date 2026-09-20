using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCover : MonoBehaviour
{
    private bool flashing = false;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color flashColor;
    [SerializeField] private Image imageToF; 
    [SerializeField] private Color colorToTrait;

    public void flashButton(bool isFlashing) {
        this.flashing = isFlashing;

        if(isFlashing) {
            this.imageToF.enabled = true;
            this.imageToF.color = flashColor;
            this.flash();
        } else {
            this.unflash();
        }
    }

    private void flash() {
        if(!this.flashing) {
            return;
        }
        this.imageToF.enabled = true;
        Invoke(nameof(unflash), .5f);
    }

    private void unflash() {
        this.imageToF.enabled = false;

        if(this.flashing) {
            Invoke(nameof(flash), .5f);
        }
    }

    public void coverButton(bool buttonIsEnabled) {
        this.imageToF.color = disabledColor;
        this.imageToF.enabled = !buttonIsEnabled;
    }

    public void flashTrait() {
        this.imageToF.enabled = true;
        this.imageToF.color = colorToTrait;

        Invoke(nameof(unflash), .5f); //TODO - i should be a setting
    }
}
