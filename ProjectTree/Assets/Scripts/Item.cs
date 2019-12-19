using UnityEngine;
public class Item : ScriptableObject {

    public string itemName = "Healing Herb";
    public string description = "Heals 10 HP.";

    public GameObject useParticle;
    public AudioClip clip;
    public float clipPitch = 1;

    [HideInInspector] public PlayerController player;
    public virtual void UseItem () {
        player = FindObjectOfType<PlayerController> ();
        if (useParticle != null) {
            Instantiate (useParticle, player.transform.position, Quaternion.identity);
        }
        if (clip != null) {
            SpawnAudio.AudioSpawn (clip, 0, clipPitch, 1);
        }

    }
}