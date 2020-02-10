using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineHurtbox : MonoBehaviour {
    [SerializeField] LineRenderer line;
    [SerializeField] int team = 2;
    [SerializeField] float damage = 30;

    void Cast () {
        for (int i = 0; i < line.positionCount - 1; i++) {
            Debug.DrawRay (transform.position + line.GetPosition (i), line.GetPosition (i + 1) - line.GetPosition (i), Color.red, 0);
            RaycastHit hit;
            if (Physics.SphereCast (transform.position + line.GetPosition (i), line.startWidth, line.GetPosition (i + 1) - line.GetPosition (i), out hit, Vector3.Distance (line.GetPosition (i), line.GetPosition (i + 1)), LayerMask.GetMask ("Default","Player","Enemy"), QueryTriggerInteraction.Collide)) {
                Hitbox hBox = hit.transform.GetComponent<Hitbox>();
                if(hBox != null && hBox.team != team){
                    hBox.GetHit(damage);
                }
            }
        }
    }

    void LateUpdate () {
        Cast ();
    }
}