using UnityEngine;
[CreateAssetMenu(fileName = "Whirlwind Potion", menuName = "Item/Buff")]
public class BuffItem : Item
{
    public float addedSpeed = 0.3f;
    public override void UseItem(){
        base.UseItem();
        player.speedMuliplier += addedSpeed;
        player.speedMuliplier = Mathf.Min(player.speedMuliplier,1.75f);
    }

    public override bool CanUse(){
        player = FindObjectOfType<PlayerController>();
        return (player.speedMuliplier - addedSpeed < 1.75f);
    }
}
