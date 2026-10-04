using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTransformation {
    public HexSquare to;
    public HexSquare from; //TODO: make me private again

    public CharacterTransformation(HexSquare to, HexSquare from) {
        this.to = to;
        this.from = from;
    }

    public virtual void apply(char[] outputArr, char[] cypherText) { //this might need hashing? might not be efficient, but counter point do I care?
        char t = to.getCharValue();
        char f = from.getCharValue();
        for(int i = 0; i < cypherText.Length; i++) {
            if (char.ToLower(t) == char.ToLower(cypherText[i])) {
                outputArr[i] = f;
            }
        }
    }
}

public abstract class CypherTrait : Trait
{
    //new design desision - cypher's don't stack within a trait, so we're going to treat each trait's cypher submission as a single closed loop addition
    private List<List<CharacterTransformation>> cyphers = new List<List<CharacterTransformation>>();

    protected void addToCypher(List<CharacterTransformation> tr) { //adds a completed loop transformation onto the cypher
        this.cyphers.Add(tr);
    }

    public override char[] runCypher(char[] curCypherText) {
        foreach(List<CharacterTransformation> completeTransformation in this.cyphers) {
            char[] newCypherText = new char[curCypherText.Length];

            foreach(CharacterTransformation ct in completeTransformation) {
                ct.apply(newCypherText, curCypherText);
            }

            for(int i = 0; i < curCypherText.Length; i++) {
                if (newCypherText[i] == '\0') {
                    newCypherText[i] = curCypherText[i]; //if no transformation applied, then it's just identity
                }
            }

            curCypherText = newCypherText;
        }
        
        return curCypherText;
    }

    public override void resetCypher() {
        this.cyphers = new List<List<CharacterTransformation>>();

    }
}

