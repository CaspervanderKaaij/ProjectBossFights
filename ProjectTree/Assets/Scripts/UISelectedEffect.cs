using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectedEffect : MonoBehaviour
{
    [SerializeField] UISelect sel;
    RectTransform rect;
    [SerializeField] Vector3 unSelectedPos;
    [SerializeField] Vector3 selectedPos;
    [SerializeField] float speed = 1;
    [SerializeField] bool unscaled = true;
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if(sel.isOver == true){
            rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition,selectedPos,DeltaTime() * speed);
        } else {
            rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition,unSelectedPos,DeltaTime() * speed);
        }
    }

    float DeltaTime(){
        if(unscaled == true){
            return Time.unscaledDeltaTime;
        } else {
            return Time.deltaTime;
        }
    }
}
