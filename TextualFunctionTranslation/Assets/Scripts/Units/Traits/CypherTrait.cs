using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTransformation {
    private HexSquare to;
    private HexSquare from;

    public void apply(char[] outputArr, char[] cypherText) { //this might need hashing? might not be efficient, but counter point do I care?
        char t = to.getCharValue();
        char f = from.getCharValue();
        for(int i = 0; i < cypherText.Length; i++) {
            if (t == cypherText[i]) {
                outputArr[i] = f;
            }
        }
    }
}

public abstract class CypherTrait : Trait
{
    private List<CharacterTransformation> cypher;

    public CypherTrait(string name, int minimumForActivate) : base(name, minimumForActivate) {
        this.cypher = new List<CharacterTransformation>();
    }

    protected void addToCypher(List<CharacterTransformation> tr) {
        this.cypher.AddRange(tr);
    }

    public void runCypher(char[] curCypherText) {
        char[] newCypherText = new char[curCypherText.Length];

        foreach(CharacterTransformation ct in this.cypher) {
            ct.apply(newCypherText, curCypherText);
        }

        for(int i = 0; i < curCypherText.Length; i++) {
            if (newCypherText[i] != '\0') {
                newCypherText[i] = curCypherText[i]; //if no transformation applied, then it's just identity
            }
        }

        this.cypher = new List<CharacterTransformation>();
    }
}

