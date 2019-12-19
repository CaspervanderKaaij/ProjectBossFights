using UnityEngine;
[CreateAssetMenu (fileName = "Healing Herb", menuName = "Item/Heal")]
public class HealItem : Item {
    public float amount = 10;
    public float clipPitch = 1;
    public override void UseItem () {
        PlayerController player = FindObjectOfType<PlayerController> ();
        player.hitbox.hp = Mathf.Min (player.maxHP, player.hitbox.hp + amount);
        
    }
}