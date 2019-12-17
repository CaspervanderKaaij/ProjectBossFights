using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {
    public DiaVars[] dialogue;
}

[System.Serializable]
public class DiaVars {
    [TextArea] public string dia;
    public enum NextDiaMethod {
        Press,
        Wait
    }
    public NextDiaMethod method;
    public int talkerID;

}