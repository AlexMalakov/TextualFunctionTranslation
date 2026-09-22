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
            //this is horrifying will debug the issues tomorrow byeeee :)
            //ok alex from literally 5 months after this comment checking in I did not in fact come back tomorrow also it was bugged kill me
            outputArr[i] = (char)((((int)cypherText[i] + (int)this.key[i % key.Length] - (int)'a') % 26) + (int)'a'); 
        }
    }
}

public class VegeneryTrait : CypherTrait
{
    [SerializeField] private Shop shop;

    public void buyTopUnit() {
        string unitName = this.shop.getVigUnitName();
        this.addToCypher(new List<CharacterTransformation>(){new VegenereCharacterTransformation(unitName)});
        this.shop.giftVigUnit();
    }

    public void trashTopUnit() {
        string unitName = this.shop.getVigUnitName();
        this.addToCypher(new List<CharacterTransformation>(){new VegenereCharacterTransformation(unitName)});
        this.shop.trashVigUnit();
    }

    public override string getTraitName() {
        return "vegenere";
    }

    public override string getTraitDescription() {
        return ("Brilliant inventors that have revolutionized the technology of the kingdom."
            + " At the end of a succesfull combat recruit the left most unit in the shop free of cost." +
            " Unit is only recruited if there is space on your bench, otherwise the unit is lost.");
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }
}
