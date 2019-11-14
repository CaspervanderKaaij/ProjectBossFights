using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : Interact
{
    public DialogueHolder holder;
    public override void Activate(PlayerController player){
        base.Activate(player);
        player.StartUIDialogue(holder);
    }
}
