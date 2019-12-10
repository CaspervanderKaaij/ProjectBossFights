using UnityEngine;
[CreateAssetMenu(fileName = "Whirlwind Potion", menuName = "Item/Buff")]
public class BuffItem : Item
{
    public override void UseItem(){
        Debug.Log("Speed buff!!");
    }
}
