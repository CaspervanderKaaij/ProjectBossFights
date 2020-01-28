using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUIBar : MonoBehaviour
{
    [SerializeField] UIBar bar;
    [SerializeField] Hitbox hp;
    [SerializeField] float maxHP = 100;

    void Update()
    {
        bar.curPercent = (hp.hp / maxHP) * 100;
    }

     void OnDestroy() {
        bar.curPercent = 0;
    }
}
