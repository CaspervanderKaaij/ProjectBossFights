using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEvent : MonoBehaviour
{
    public UnityEvent ev;
    public float waitTime = 0.1f;
    void Start()
    {
        Invoke("Activate",waitTime);
    }

    void Activate(){
        ev.Invoke();
        Destroy(this);
    }

}
