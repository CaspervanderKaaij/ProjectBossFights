using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour {
    public Item[] itemTypes;
    public int[] myItems;
    void Update () {
        if (Input.GetKeyDown (KeyCode.Alpha1)) {
            UseItem (0);
        }
    }

    public void UseItem (int inventorySlot) {
        itemTypes[myItems[inventorySlot]].UseItem ();
    }

}