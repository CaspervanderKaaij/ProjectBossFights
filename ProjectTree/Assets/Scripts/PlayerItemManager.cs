using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemManager : MonoBehaviour {
    public Item[] itemTypes;
    public InventoryValues[] myItems;
    public Text[] uiOwned;
    [Header ("Input")]
    public string item1Button = "Item1";
    public string item2Button = "Item2";
    public string item3Button = "Item3";
    public string item4Button = "Item4";

    void Start () {
        UpdateUIOwnedText ();
    }
    void Update () {
        GetInput ();
        GetAxisInput ();
    }

    void GetInput () {
        for (int i = 0; i < 4; i++) {
            if (Input.GetButtonDown (GetValueByString.GetStringValue ("item" + (i + 1) + "Button", this)) == true) {
                UseItem (i);
                UpdateUIOwnedText ();
            }
        }
    }

    bool canAxisInput = true;
    void GetAxisInput () {
        if (canAxisInput == true) {

            if (Input.GetAxis (item1Button) != 0) {
                canAxisInput = false;
                if (Input.GetAxis (item1Button) > 0) {
                    //print("right");
                    UseItem (2);
                } else {
                   // print("left");
                    UseItem (1);
                }
                UpdateUIOwnedText ();
            }

            if (Input.GetAxis (item3Button) != 0) {
                canAxisInput = false;
                if (Input.GetAxis (item3Button) > 0) {
                 //   print("up");
                    UseItem (0);
                } else {
                  //  print("down");
                    UseItem (3);
                }
                UpdateUIOwnedText ();
            }
        }

        if (Input.GetAxis (item1Button) == 0 && Input.GetAxis (item3Button) == 0) {
            canAxisInput = true;
        }
    }

    void UpdateUIOwnedText () {
        for (int i = 0; i < 4; i++) {
            uiOwned[i].text = "" + myItems[i].owned;
        }
    }

    public void UseItem (int inventorySlot) {
        if (myItems[inventorySlot].owned > 0) {
            if (itemTypes[myItems[inventorySlot].itemID].CanUse () == true) {
                itemTypes[myItems[inventorySlot].itemID].UseItem ();
                myItems[inventorySlot].owned--;
            }
        }
    }

}

[System.Serializable]
public class InventoryValues {
    public int itemID = 0;
    public int owned = 5;
}