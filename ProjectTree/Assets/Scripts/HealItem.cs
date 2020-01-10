using UnityEngine;
[CreateAssetMenu (fileName = "Healing Herb", menuName = "Item/Heal")]
public class HealItem : Item {
    public float amount = 10;
    public override void UseItem () {
        base.UseItem();
        player.hitbox.hp = Mathf.Min (player.maxHP, player.hitbox.hp + amount);
        
    }

    public override bool CanUse(){
        player = FindObjectOfType<PlayerController>();
        return (player.hitbox.hp < player.maxHP);
    }
}