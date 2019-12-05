using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RepeatEvent : MonoBehaviour
{
    [SerializeField] UnityEvent ev;
    [SerializeField] float rate = 1;

    void Start()
    {
        InvokeRepeating("Ev",0,rate);
    }

    void Ev(){
        ev.Invoke();
    }
}
