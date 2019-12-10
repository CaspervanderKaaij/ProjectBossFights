using UnityEngine;
public class Item : ScriptableObject
{

    public string itemName = "Healing Herb";
    public string description = "Heals 10 HP.";
    public virtual void UseItem(){
        Debug.Log("I used an item :D");
    }
}
