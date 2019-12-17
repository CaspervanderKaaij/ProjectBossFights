﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelInfo curInfo = null;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Activate(LevelInfo info){
        curInfo = info;
        curInfo.Load();
    }

    public void Activate(string levelInfoName){
        curInfo = Resources.Load(levelInfoName) as LevelInfo;
        print(curInfo);
        curInfo.Load();
    }

    public void ActivateWithCurInfo(){
        curInfo.Load();
    }
}
