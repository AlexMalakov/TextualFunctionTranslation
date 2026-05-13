using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class VegenereCharacterTransformation : CharacterTransformation {
    private string key;

    public VegenereCharacterTransformation(HexSquare to, HexSquare from, string key) : base(to, from) {
        this.key = key;
    }

    public override void apply(char[] outputArr, char[] cypherText) {

        for(int i = 0; i < cypherText.Length; i++) {

            outputArr[i] = (char)((((int)cypherText[i] + (int)this.key[i % key.Length]) % 26) + (int)'a'); //this is horrifying will debug the issues tomorrow byeeee :)
        }
    }
}

public class VegeneryTrait : CypherTrait
{
    public VegeneryTrait() : base("vegenery", 2) {}


    public override void onCombatEnd(Unit traitHaver){
        string unitName = "placeholder"; //todo: give this string a value

        //TODO: buy the placeholder unit

        this.addToCypher(new List<CharacterTransformation>(){new VegenereCharacterTransformation(traitHaver.getPosition(), traitHaver.getPosition(), unitName)});
    }
}
