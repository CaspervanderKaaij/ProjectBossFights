using UnityEngine;
[CreateAssetMenu(fileName = "Whirlwind Potion", menuName = "Item/Buff")]
public class BuffItem : Item
{
    public float addedSpeed = 0.3f;
    public GameObject useParticle;
    public override void UseItem(){
        PlayerController player = FindObjectOfType<PlayerController>();
        player.speedMuliplier += addedSpeed;
        player.speedMuliplier = Mathf.Min(player.speedMuliplier,1.75f);
        Instantiate(useParticle,player.transform.position,Quaternion.identity);
    }
}
