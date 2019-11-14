using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour
{
    [TextArea] public string[] dialogue;
    public enum NextDiaMethod
    {
        Press,
        Wait
    }
}
