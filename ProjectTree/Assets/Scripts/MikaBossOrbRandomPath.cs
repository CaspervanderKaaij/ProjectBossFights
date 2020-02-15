using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikaBossOrbRandomPath : MonoBehaviour
{
    [SerializeField] Transform[] posses;
    [SerializeField] float speed = 5;
    int curGoal = 0;
    void Start() 
    {
        curGoal = Random.Range(0,posses.Length);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,posses[curGoal].position,Time.deltaTime * speed);
        if(Vector3.Distance(transform.position, posses[curGoal].position) < 1){
            curGoal = Random.Range(0,posses.Length);
        }
    }
}
