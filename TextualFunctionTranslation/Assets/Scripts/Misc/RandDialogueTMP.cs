using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandDialogueTMP : RandTMP
{
    [SerializeField] private bool permissionToScramble;
    private bool permaS;

    public void setPermaScramble(bool perma) {
        this.permaS = perma;
    }

    public override void scramble() {
        if(this.permissionToScramble || this.permaS) {
            base.scramble();
        }
    }

    public void allowedToScramble(bool allowed) {
        this.permissionToScramble = allowed;
    }

    public override void reset() {
        if(!this.permaS) base.reset();
    }
}
