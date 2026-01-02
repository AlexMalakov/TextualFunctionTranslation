using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopSequence {
    public List<ShopEntry> shops;
}

[System.Serializable]
public class ShopEntry {
    public List<string> units;
}

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
