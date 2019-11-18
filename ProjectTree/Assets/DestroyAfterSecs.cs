using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSecs : MonoBehaviour
{
    public float after = 0;
    public GameObject toDestroy;
    void Start()
    {
        Destroy(toDestroy,after);
    }
}
