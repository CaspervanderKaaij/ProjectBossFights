using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
   public void Instant(GameObject g){
       Instantiate(g,transform.position,transform.rotation);
   }
}
