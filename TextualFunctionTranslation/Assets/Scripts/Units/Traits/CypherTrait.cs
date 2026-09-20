using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTransformation {
    private HexSquare to;
    private HexSquare from;

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
    private List<CharacterTransformation> cypher = new List<CharacterTransformation>();

    protected void addToCypher(List<CharacterTransformation> tr) {
        this.cypher.AddRange(tr);
    }

    public override char[] runCypher(char[] curCypherText) {
        char[] newCypherText = new char[curCypherText.Length];

        foreach(CharacterTransformation ct in this.cypher) {
            ct.apply(newCypherText, curCypherText);
        }

        int copy_count = 0;
        int preserved = 0;
        for(int i = 0; i < curCypherText.Length; i++) {
            
            if (newCypherText[i] == '\0') {
                newCypherText[i] = curCypherText[i]; //if no transformation applied, then it's just identity
                copy_count++;
            } else {
                preserved++;
            }
        }
        
        return newCypherText;
    }

    public override void resetCypher() {
        this.cypher = new List<CharacterTransformation>();
    }
}

