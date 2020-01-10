using UnityEngine;
[CreateAssetMenu (fileName = "Spirit Orb", menuName = "Item/Invincible")]

public class InvinciblilityItem : Item
{
    public float time = 10;
    public override void UseItem () {
        base.UseItem();
        PlayerController player = FindObjectOfType<PlayerController> ();
       player.GetComponent<Hitbox> ().enabled = false;
        player.StopCoroutine("HitFlash");
        player.StartCoroutine("HitFlash");
        player.CancelInvoke ("StopInvincible");
        player.Invoke ("StopInvincible", time);
        
    }

    public override bool CanUse(){
        player = FindObjectOfType<PlayerController>();
        return player.GetComponent<Hitbox> ().enabled;
    }
}
