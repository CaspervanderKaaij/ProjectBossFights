using UnityEngine;
[CreateAssetMenu (fileName = "Healing Herb", menuName = "Item/Heal")]
public class HealItem : Item {
    public float amount = 10;
    public GameObject useParticle;
    public AudioClip clip;
    public float clipPitch = 1;
    public override void UseItem () {
        PlayerController player = FindObjectOfType<PlayerController> ();
        player.hitbox.hp = Mathf.Min (player.maxHP, player.hitbox.hp + amount);
        if (useParticle != null) {
            Instantiate (useParticle, player.transform.position, useParticle.transform.rotation);
        }
        if(clip != null){
            SpawnAudio.AudioSpawn(clip,0,clipPitch,1);
        }
    }
}