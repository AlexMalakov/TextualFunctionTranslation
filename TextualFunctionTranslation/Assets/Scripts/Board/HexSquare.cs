using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[System.Serializable]
public struct AxialCoord {
    public int q, r, s;

    public AxialCoord(int q, int r) {
        this.q = q;
        this.r = r;
        this.s = -q - r;
    }

    public AxialCoord(int q, int r, int s) {
        if(q + r + s != 0) {
            throw new Exception("INVALID Q R S VALUES!");
        }
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public OffsetCoord toOffset() {
        int parity = r&1;
        int x = q + (int)Mathf.Floor((r + parity) * .5f);
        int y = r;
        return new OffsetCoord(x, y);
    }

    public AxialCoord getMirroredCoord() {
        OffsetCoord preM = this.toOffset();
        if(preM.y <= 3) {
            return new OffsetCoord(preM.x, preM.y + 4).toAxial();
        } else {
            return new OffsetCoord(preM.x, preM.y - 4).toAxial();
        }
    }

    public bool equal(AxialCoord o) {
        return this.q == o.q && this.r == o.r;
    }
}

[Serializable]
public struct OffsetCoord {
    public int x, y;

    public OffsetCoord(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public AxialCoord toAxial() {
        int parity = y&1;
        int q = x - (int)Mathf.Floor((y + parity) * .5f);
        int r = y;
        return new AxialCoord(q, r);
    }
}

public class HexSquare : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject flashMe;
    private AxialCoord axc;
    private Unit unit;

    private char characterValue;
    private bool showing = false;
    private bool tutorialFlashOn = false;
    
    public virtual void init(int x, int y, Vector3 hexPos) {
        this.text.gameObject.SetActive(false);
        if(y >= 4) {
            this.text.color = new Color(0, 255, 0);
        } else {
            this.text.color = new Color(255, 0, 0);
        }
        this.transform.position = hexPos;
        this.axc = new OffsetCoord(x, y).toAxial();
    }

    public virtual List<AxialCoord> getNeighborCoords() {
        return new List<AxialCoord>(){
            new AxialCoord(this.axc.q+1, this.axc.r-1, this.axc.s),
            new AxialCoord(this.axc.q+1, this.axc.r, this.axc.s-1),
            new AxialCoord(this.axc.q-1, this.axc.r+1, this.axc.s),
            new AxialCoord(this.axc.q-1, this.axc.r, this.axc.s+1),
            new AxialCoord(this.axc.q, this.axc.r+1, this.axc.s-1),
            new AxialCoord(this.axc.q, this.axc.r-1, this.axc.s+1)};
    }

    public char getCharValue() {
        return characterValue;
    }

    public virtual AxialCoord getCoords() {
        return this.axc;
    }

    public virtual bool isSquare() {
        return true;
    }

    //returns distance between two hexSquares
    public static int hexDist(HexSquare s1, HexSquare s2) {
        AxialCoord s1c = s1.getCoords();
        AxialCoord s2c = s2.getCoords(); 
        return (Mathf.Abs(s1c.q - s2c.q) + Mathf.Abs(s1c.r - s2c.r) + Mathf.Abs(s1c.s - s2c.s))/2;
    }

    public virtual Unit getUnit() {
        return this.unit;
    }

    public virtual void claimSquare(Unit u, bool status) {
        if(status) {
            // Debug.Log("CLAIMING SQUARE " + this.toString());
            if(this.unit != null) {
                throw new Exception("cannot claim an already claimed hex square! " + u.gameObject.name + " on square " + this.toString() + " trying to take from " + this.unit.gameObject.name);
            } 
            this.unit = u;
        } else {
            // Debug.Log("UNCLAIMING SQUARE " + this.toString());
            if(this.unit == null) {
                throw new Exception("cannot leave an unoccupied hex square! " + u.gameObject.name + " on square " + this.toString());
            }
            this.unit = null;
        }
    }

    public void assignChar(char c) {
        this.characterValue = c;
        this.text.text = c + "";
        this.showing = false;
    }

    public void displayValue(bool toggle) {
        this.showing = toggle;
        this.text.gameObject.SetActive(toggle);
    }

    public bool isShowingChar() {
        return this.showing;
    }

    public void highlightSquare(Color color, float duration) {
        this.flashMe.SetActive(true);
        this.flashMe.GetComponent<Renderer>().material.color = color;
        if(duration > 0) {
            Invoke(nameof(unhighlight), duration);
        }
    }

    public void tutorialFlash(bool flashActive) {
        this.tutorialFlashOn = flashActive;
        if(flashActive) {
            this.doFlash();
        } else {
            this.unhighlight();
        }
    }

    private void doFlash() {
        if(!this.tutorialFlashOn) {
            this.unhighlight();
            return;
        }
        this.highlightSquare(new Color(255, 0, 0), .5f);
        Invoke(nameof(doFlash), 1f);

    }
    
    private void unhighlight() {
        this.flashMe.SetActive(false);
    }

    public string toString() {
        // return this.getCoords().toOffset().x + ", " + this.getCoords().toOffset().y;
        return "OFFEST: " + this.getCoords().toOffset().x + ", " + this.getCoords().toOffset().y + " CUBIC: " + this.getCoords().q+ ", " + this.getCoords().r+ ", " + this.getCoords().s;
    }

    public void showDruidInfluence(GameObject particle) {
        particle.transform.position = this.transform.position + Vector3.up * .5f;
    }
}
