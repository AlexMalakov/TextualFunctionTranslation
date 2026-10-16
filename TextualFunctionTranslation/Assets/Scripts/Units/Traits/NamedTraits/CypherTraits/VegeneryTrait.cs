using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class VegenereCharacterTransformation : CharacterTransformation {
    private string key;

    public VegenereCharacterTransformation(string key) : base(null, null) {
        this.key = key;
    }

    public override void apply(char[] outputArr, char[] cypherText) {

        for(int i = 0; i < cypherText.Length; i++) {
            if(!isLetter(cypherText[i])) {
                continue;
            }
            //this is horrifying will debug the issues tomorrow byeeee :)
            //ok alex from literally 5 months after this comment checking in I did not in fact come back tomorrow also it was bugged kill me
            int caseBase = isUpper(cypherText[i]) ? 'A' : 'a';
            outputArr[i] = (char)((((int)cypherText[i] - caseBase + (int)this.key[i % key.Length] - (int)'a') % 26) + caseBase);
        }
    }

    private bool isLetter(char c) {
        return ((int)c >= (int)'a' && (int)c <= (int)'z') || ((int)c >= (int)'A' && (int)c <= (int)'Z');
    }

    private bool isUpper(char c) {
        return (int)'A' <= (int)c && (int)c <= (int)'Z';
    }
}

public class VegeneryTrait : CypherTrait
{
    [SerializeField] private Shop shop;

    public void buyTopUnit() { //TODO: combat event?
        string unitName = this.shop.getVigUnitName();
        Debug.Log("bought CYPHER WITH KEY " + unitName + " I AM " + gameObject.name);
        this.addToCypher(new List<CharacterTransformation>(){new VegenereCharacterTransformation(unitName)});
        this.shop.giftVigUnit();
    }

    public void trashTopUnit() {
        string unitName = this.shop.getVigUnitName();
        Debug.Log("sold CYPHER WITH KEY " + unitName + " I AM " + gameObject.name);
        this.addToCypher(new List<CharacterTransformation>(){new VegenereCharacterTransformation(unitName)});
        this.shop.trashVigUnit();
    }

    public override string getTraitName() {
        return "vigenery";
    }

    public override string getTraitNarrative() {
        return "Renowned inventors who attract others to witness their brilliant minds in action.";
    }

    public override string getTraitEffect() {
        return (
            "After a combat concludes, recruit the left most unit from the baracks onto the bench. If there is no space, the unit is lost. Only one unit can be purchased after a combat ends per player."
        );
    }

    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }
}
