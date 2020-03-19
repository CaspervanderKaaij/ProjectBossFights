using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class DialogueHolder : MonoBehaviour {
    public DiaVars[] dialogue;
}

[System.Serializable]
public class DiaVars {
    [TextArea] public string dia;
    public string talker = "";
    public float letterSpeed = 0.05f;
    public enum NextDiaMethod {
        Press,
        Wait,
        NoBackPress,
        NoBackWait
    }
    public NextDiaMethod method;
    public int talkerID;
    [ConditionalField("method",false,NextDiaMethod.Wait)] public float waitTime = 1;
    [ConditionalField("method",false,NextDiaMethod.NoBackWait)] public float _waitTime = 1;

}